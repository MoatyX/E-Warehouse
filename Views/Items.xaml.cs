using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using E_Warehouse.Data;
using E_Warehouse.Models;
using E_Warehouse.Utils;

namespace E_Warehouse.Views
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : UserControl
    {
        private readonly ItemModel _dataContextModel;
        private readonly ObservableCollection<Item> _itemsViewSource;

        private bool _newItemMode;

        private Brush _defaultColorBrush;
        private readonly DataTable _itemSourceCompanies;

        public Item SelectedItem => itemListView.SelectedItem is Item item ? item : null;
        private bool _rowInitPass;
        private bool _noMatch;


        /// <summary>
        /// Constructor that takes 2 parameters, defining the behaviour of the view
        /// </summary>
        /// <param name="viewMode">whether to display a single item, batch or all of them</param>
        /// <param name="partNumber">in the case of single item, partnumber is given to get that item</param>
        public Items(params string[] partNumber)
        {
            InitializeComponent();

            _dataContextModel = new ItemModel();

            //alter the collection based on the ItemDisplayMode
            switch (partNumber.Length)
            {
                case 0:
                    _dataContextModel.Items.Include(x => x.SourceCompanies).Load();
                    _itemsViewSource = _dataContextModel.Items.Local;
                    break;
                case 1:
                    string itemToFind = partNumber[0];
                    var item = _dataContextModel.Items.Include(x => x.SourceCompanies)
                        .FirstOrDefault(x => x.PartNumber.Equals(itemToFind, StringComparison.CurrentCultureIgnoreCase));
                    if (item == null)
                    {
                        _noMatch = true;
                    }
                        
                    _itemsViewSource = new ObservableCollection<Item> {item};
                    break;
                default:
                    var items = _dataContextModel.Items.Where(x =>
                        partNumber.Contains(x.PartNumber, StringComparer.CurrentCultureIgnoreCase)).ToList();
                    if (items.Count == 0)
                    {
                        _noMatch = true;
                    }
                    _itemsViewSource = new ObservableCollection<Item>(items);
                    break;
            }

            _itemSourceCompanies = new DataTable();
            _itemSourceCompanies.Columns.Add(new DataColumn("CompanyName"));
            RefreshSourceCompanies((Item)itemListView.SelectedItem);

            _itemSourceCompanies.RowDeleting += ItemSourceCompaniesOnRowDelete;
            _itemSourceCompanies.RowChanged += ItemSourceCompaniesOnRowChanged;
        }

        /// <summary>
        /// shows an error dialog that the search was unsuccessful and returns to the search window
        /// </summary>
        private void NoMatchFound()
        {
            MessageBox.Show("Could not find a match", "Not found", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            Application.Current.MainWindow.DataContext = new SearchItem();
        }

        private void ItemSourceCompaniesOnRowDelete(object sender, DataRowChangeEventArgs e)
        {
            if(_rowInitPass) return;
            if (e.Action != DataRowAction.Delete) return;

            var sourceCompanies = _itemsViewSource[itemListView.SelectedIndex].SourceCompanies;
            sourceCompanies.Remove(sourceCompanies.First(x => x.Name.Equals((string)e.Row.ItemArray[0])));
        }

        private void ItemSourceCompaniesOnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if(_rowInitPass) return;
            switch (e.Action)
            {
                case DataRowAction.Change:
                    NewSourceCompanyEntryProcess(e.Row.ItemArray[0] as string);
                    break;
                case DataRowAction.Add:
                    NewSourceCompanyEntryProcess(e.Row.ItemArray[0] as string);
                    break;
            }
        }

        /// <summary>
        /// the process of adding a new company entry to an item
        /// </summary>
        private void NewSourceCompanyEntryProcess(string companyName)
        {
            if (_newItemMode)
            {
                throw new Exception("Not yet supported");
            }

            //check if the added company name already exists or if we should create a new company
            var addedCompany = companyName;
            var potentialCompany = _dataContextModel.Companies.FirstOrDefault(x =>
                x.Name.Equals(addedCompany, StringComparison.CurrentCultureIgnoreCase));
            if (potentialCompany != null)
            {
                potentialCompany.IsSourceCompany = true;
                SelectedItem.SourceCompanies.Add(potentialCompany);
                _itemSourceCompanies.AcceptChanges();
                return;
            }

            //no item was found in the database, create a new company then
            var result =
                MessageBox.Show(
                    "The Entered Company name was not found in the database, should a new company with this name be created ?",
                    "Company Not found", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                _itemsViewSource[itemListView.SelectedIndex].SourceCompanies.Add(new Company()
                {
                    IsSourceCompany = true,
                    Name = addedCompany
                });
                _itemSourceCompanies.AcceptChanges();
            }
            else
                _itemSourceCompanies.RejectChanges();

        }

        private void RefreshSourceCompanies(Item item)
        {
            _rowInitPass = true;
            _itemSourceCompanies.Rows.Clear();
            if (item == null) return;
            foreach (var company in item.SourceCompanies)
            {
                if (company != null)
                    _itemSourceCompanies.Rows.Add(company.Name);
            }

            _itemSourceCompanies.AcceptChanges();

            _rowInitPass = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_noMatch)
            {
                NoMatchFound();
                return;
            }

            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (FindResource("itemViewSource") is CollectionViewSource itemsView)
            {
                itemsView.Source = _itemsViewSource;
                sourceCompaniesDataGrid.ItemsSource = _itemSourceCompanies.DefaultView;
            }

            _defaultColorBrush = itemListView.Background;

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SetupNewEntryMode();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this item ?", "Item Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _dataContextModel.Items.Remove(SelectedItem);
            }
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            var changes = 0;
            if (_newItemMode)
            {
                var newItem = new Item
                {
                    Name = ItemName.Text,
                    BuyPrice = Convert.ToInt32(buyPriceTextBox.Text),
                    SellPrice = Convert.ToInt32(sellPriceTextBox.Text),
                    PartNumber = ItemPartNumber.Text,
                    Quantity = Convert.ToInt32(quantityTextBox.Text)
                };

                _dataContextModel.Items.Add(newItem);
                changes = _dataContextModel.SaveChanges();
            }

            changes += _dataContextModel.SaveChanges();
            _itemSourceCompanies.AcceptChanges();
            itemListView.UpdateLayout();
            SetupEntryDisplayMode();

            if (changes > 0)
                MessageBox.Show($"{changes} Changes have been applied", "Info", MessageBoxButton.OK);
            else
                MessageBox.Show("No Changes have been detected to apply", "info", MessageBoxButton.OK);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            SetupEntryDisplayMode();
        }

        private void SetupNewEntryMode()
        {
            BtnCancel.Visibility = Visibility.Visible;

            ItemDetailView.Background = Brushes.Aquamarine;
            _newItemMode = true;
            DataContext = null;

            idTextBox.Visibility = Visibility.Collapsed;
        }

        private void SetupEntryDisplayMode()
        {
            BtnCancel.Visibility = Visibility.Collapsed;
            idTextBox.Visibility = Visibility.Visible;

            ItemDetailView.Background = _defaultColorBrush;
            _newItemMode = false;
            DataContext = itemListView;
        }

        private void ItemListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSourceCompanies((Item)itemListView.SelectedItem);
        }
    }
}

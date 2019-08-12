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
using E_Warehouse.Views.Modals;

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

        public Item SelectedItem => itemListView.SelectedItem is Item item ? item : null;
        private bool _rowInitPass;
        private readonly bool _noMatch;


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
                    _dataContextModel.Items.Include(x => x.Transactions.Select(y => y.Company)).Load();
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
                //transactionsDataGrid.ItemsSource = _itemTransactionsTable.DefaultView;
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

        private void BtnNewCompany_Click(object sender, RoutedEventArgs e)
        {
            ItemCompanyEntryWindow companyEntry =
                new ItemCompanyEntryWindow(SelectedItem.Id, _dataContextModel.Companies.ToList())
            {
                Owner = Application.Current.MainWindow
            };
            var result = companyEntry.ShowDialog();

            //as soon as we get a Dialog result, we continue executing here
            if (result.HasValue && result.Value)
            {
                _itemsViewSource[itemListView.SelectedIndex].Transactions.Add(companyEntry.NewTransaction);
                transactionsDataGrid.UpdateLayout();
            }
        }
    }
}

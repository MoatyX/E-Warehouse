using E_Warehouse.Data;
using E_Warehouse.Models;
using E_Warehouse.Views.Modals;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace E_Warehouse.Views
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : UserControl
    {
        private DataModelRepo DataModel => DataModelRepo.Instance;

        private readonly ObservableCollection<Item> _itemsViewSource;

        private bool _newItemMode;
        private bool _importMode;
        private Brush _defaultColorBrush;

        public Item SelectedItem => itemListView?.SelectedItem is Item item ? item : null;
        private readonly bool _noMatch;


        /// <summary>
        /// Constructor that takes partNumber strings as an input to look them up from the database and display them in the UI
        /// </summary>
        /// <param name="viewMode">whether to display a single item, batch or all of them</param>
        /// <param name="partNumber">in the case of single item, partnumber is given to get that item</param>
        public Items(params string[] partNumber)
        {
            InitializeComponent();

            //alter the collection based on the ItemDisplayMode
            switch (partNumber.Length)
            {
                case 0:
                    MessageBox.Show("Please Provide Item Numbers to preform the search", "No Item Numbers were given",
                        MessageBoxButton.OK);
                    break;
                case 1:
                    string itemToFind = partNumber[0];
                    var item = DataModel.Items.FirstOrDefault(x => x.PartNumber.Equals(itemToFind, StringComparison.CurrentCultureIgnoreCase));
                    if (item == null)
                    {
                        _noMatch = true;
                    }

                    _itemsViewSource = new ObservableCollection<Item> { item };
                    break;
                default:
                    var items = DataModel.Items.Where(x =>
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
        /// Constructor that takes Item objects to display them independent from the database
        /// </summary>
        /// <param name="items"></param>
        public Items(params Item[] items)
        {
            InitializeComponent();

            _itemsViewSource = new ObservableCollection<Item>(items);
            _importMode = true;
        }

        public Items()
        {
            InitializeComponent();

            _itemsViewSource = DataModel.Items;
        }

        /// <summary>
        /// shows an error dialog that the search was unsuccessful and returns to the search window
        /// </summary>
        private static void NoMatchFound()
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
            }

            _defaultColorBrush = itemListView.Background;
            Console.WriteLine(SelectedItem);

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_newItemMode)
            {
                //we are actually in the process of Adding a new Entry, notify the user and return
                MessageBox.Show("A new Item has been Added but not applied, Apply Changes first", "Unsaved Changes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SetupNewEntryMode();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_newItemMode)
            {
                //if we are in this mode, then the delete button will act as a cancel button
                BtnCancel_OnClick(sender, e);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this item ?", "Item Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DataModel.RemoveItem(SelectedItem);
            }
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Commit Changes ?", "Confirmation", MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
                return;

            if (_newItemMode)
            {
                var newItem = SelectedItem;
                newItem.Name = nameTextBox.Text;
                newItem.SellPrice = Convert.ToDouble(sellPriceTextBox.Text);
                newItem.PartNumber = partNumberTextBox.Text;
                newItem.Quantity = Convert.ToInt32(quantityTextBox.Text);

                DataModel.AddItem(newItem);

                _newItemMode = false;
            }

            if (_importMode)
            {
                foreach (var item in _itemsViewSource)
                {
                    DataModel.AddItem(item);
                }

                _importMode = false;
            }

            int changes = DataModel.SaveChanges();
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

            _itemsViewSource.Add(new Item("New Item"));
            itemListView.SelectedIndex = _itemsViewSource.Count - 1;
            itemListView.IsEnabled = false;
        }

        private void SetupEntryDisplayMode()
        {
            if (_newItemMode)
            {
                /*
                 * if we are in this mode, this definitely mean that:
                 * the itemListView.SelectedIndex = _itemsViewSource.Count - 1;
                 * and that item is the new item we've added but never committed,
                 * so remove it and continue normally
                 */

                _itemsViewSource.RemoveAt(_itemsViewSource.Count - 1);
            }

            BtnCancel.Visibility = Visibility.Collapsed;

            ItemDetailView.Background = _defaultColorBrush;
            _newItemMode = false;

            itemListView.IsEnabled = true;
        }

        private void BtnNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            var targetItem = _itemsViewSource[itemListView.SelectedIndex];

            ItemCompanyEntryWindow companyEntry =
                new ItemCompanyEntryWindow(targetItem, DataModel.Companies.ToList())
                {
                    Owner = Application.Current.MainWindow
                };
            var result = companyEntry.ShowDialog();

            //as soon as we get a Dialog result, we continue executing here
            if (result.HasValue && result.Value)
            {

                switch (companyEntry.NewTransaction.ItemTransactionType)
                {
                    case ItemTransaction.TransactionType.Buy:
                        SelectedItem.Quantity += companyEntry.NewTransaction.Quantity;
                        break;
                    case ItemTransaction.TransactionType.Sell:
                        if (SelectedItem.Quantity - companyEntry.NewTransaction.Quantity < 0)
                        {
                            MessageBox.Show("The Required Quantity in the Transaction is higher than stock!\nif you dont wish to continue, dont Apply Changes and restart",
                                "Quantity Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        targetItem.Quantity -= companyEntry.NewTransaction.Quantity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                quantityTextBox.Text = targetItem.Quantity.ToString();
                targetItem.Transactions.Add(companyEntry.NewTransaction);
                DataModel.UpdateItemStats(targetItem);
                transactionsDataGrid.Items.Refresh();
            }
        }
    }
}

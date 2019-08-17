using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using E_Warehouse.Data;
using E_Warehouse.Models;

namespace E_Warehouse.Views.Modals
{
    /// <summary>
    /// Interaction logic for ItemCompanyEntryWindow.xaml
    /// </summary>
    public partial class ItemCompanyEntryWindow : Window
    {
        public IEnumerable<string> CompanyNames { get; } //used solely by the textbox to do auto complete
        private readonly Item _item;
        private readonly IEnumerable<Company> _companies;

        public ItemTransaction NewTransaction { get; private set; }

        private double Price
        {
            get
            {
                bool success = double.TryParse(PriceTextbox.Text, NumberStyles.Any, CultureInfo.CurrentCulture,
                    out var output);

                if (success) return output;

                MessageBox.Show("Please Enter Numbers Only!", "Invalid Input Type", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return 0;

            }
        }

        private int Quantity
        {
            get
            {
                bool success = int.TryParse(QuantityTextbox.Text, NumberStyles.Any, CultureInfo.CurrentCulture,
                    out var output);

                if (success) return Math.Abs(output);

                MessageBox.Show("Please Enter Numbers Only!", "Invalid Input Type", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return 0;
            }
        }

        public ItemCompanyEntryWindow(Item item, IEnumerable<Company> companies = null)
        {
            InitializeComponent();

            _item = item;
            _companies = companies;

            if (_companies != null) CompanyNames = _companies.Select(x => x.Name).ToArray();

            DataContext = this;
            TransactionType.ItemsSource = Enum.GetValues(typeof(ItemTransaction.TransactionType))
                .Cast<ItemTransaction.TransactionType>();
        }

        private void OkBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var autoCompletedCompany = _companies.FirstOrDefault(x =>
                x.Name.Equals(CompanyNameTextbox.Text, StringComparison.CurrentCultureIgnoreCase));
            if (autoCompletedCompany == null)
            {
                var result =
                    MessageBox.Show("Company is not found in the database and thus will be automatically created",
                        "Company Name Not found", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    autoCompletedCompany = new Company
                    {
                        Name = CompanyNameTextbox.Text,
                        Items = new List<Item>(),
                    };
                }
            }

            if (autoCompletedCompany != null)
            {
                CreateTransaction(autoCompletedCompany);
                DialogResult = true;
            }
            else DialogResult = false;
        }

        private void CreateTransaction(Company company)
        {
            var transType = (ItemTransaction.TransactionType)TransactionType.SelectedIndex;
            NewTransaction = new ItemTransaction
            {
                Company = company,
                Price = Price,
                ItemTransactionType = transType,
                TransactionDate = TransactionDate.DisplayDate,
                Quantity = Quantity
            };

            if (transType == ItemTransaction.TransactionType.Buy)
            {
                company.IsSourceCompany = true;
                company.Items.Add(_item);
            }
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

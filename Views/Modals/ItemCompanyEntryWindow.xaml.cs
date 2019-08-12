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
        private readonly int _itemId;
        private readonly IEnumerable<Company> _companies;

        public ItemTransaction NewTransaction { get; private set; }

        private float Price
        {
            get
            {
                bool success = float.TryParse(PriceTextbox.Text, NumberStyles.Any, CultureInfo.CurrentCulture,
                    out var output);

                if (success) return output;

                MessageBox.Show("Please Enter Numbers Only!", "Invalid Input Type", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return 0;

            }
        }

        public ItemCompanyEntryWindow(int itemId, IEnumerable<Company> companies = null)
        {
            InitializeComponent();

            _itemId = itemId;
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
            if (autoCompletedCompany != null)
            {
                var transType = (ItemTransaction.TransactionType) TransactionType.SelectedIndex;
                NewTransaction = new ItemTransaction
                {
                    CompanyId = autoCompletedCompany.Id,
                    ItemId = _itemId,
                    Price = Price,
                    ItemTransactionType = transType
                };

                if (transType == ItemTransaction.TransactionType.Buy)
                {
                    autoCompletedCompany.IsSourceCompany = true;
                }
            }
                

            this.DialogResult = autoCompletedCompany != null;
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

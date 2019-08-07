using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using E_Warehouse.Data;
using E_Warehouse.Models;

namespace E_Warehouse.Views
{
    /// <summary>
    /// Interaction logic for AllCompanies.xaml
    /// </summary>
    public partial class AllCompanies : UserControl
    {
        private Brush _defaultColorBrush;

        private readonly ItemModel _dataContextModel;

        public ObservableCollection<Company> Companies => _dataContextModel.Companies.Local;
        public bool NewCompanyState;


        public AllCompanies()
        {
            InitializeComponent();

            _dataContextModel = new ItemModel();
            _dataContextModel.Companies.Load();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(this)) return;

            if (FindResource("companyViewSource") is CollectionViewSource companiesList)
            {
                companiesList.Source = Companies;
            }

            _defaultColorBrush = companyDetailGrid.Background;
        }

        /// <summary>
        /// Add new company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SetupNewEntryMode();
        }

        /// <summary>
        /// setup the UI form to take a new entry instead of displaying entries
        /// (New Company Mode)
        /// </summary>
        private void SetupNewEntryMode()
        {
            BtnCancel.Visibility = Visibility.Visible;
            Label_CompanyDetails.Visibility = Visibility.Collapsed;

            NewCompanyState = true;
            companyDetailGrid.Background = Brushes.Aquamarine;
            companyDetailGrid.DataContext = null;

            id.Visibility = Visibility.Collapsed;
            foreach (UIElement child in companyDetailGrid.Children)
            {
                switch (child)
                {
                    case TextBox textBox:
                        textBox.Text = "";
                        break;
                    case CheckBox checkBox:
                        checkBox.IsChecked = false;
                        break;
                }
            }
        }

        private void SetupEntryDisplayMode()
        {
            Label_CompanyDetails.Visibility = Visibility.Visible;
            BtnCancel.Visibility = Visibility.Collapsed;

            NewCompanyState = false;
            companyDetailGrid.Background = _defaultColorBrush;
            companyDetailGrid.DataContext = companyListView.DataContext;

            id.Visibility = Visibility.Visible;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            SetupEntryDisplayMode();
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            if (NewCompanyState)
            {
                Company company = new Company
                {
                    Name = CompanyNameTextBox.Text,
                    IsSourceCompany = isSourceCompanyCheckBox.IsChecked.Value
                };

                Companies.Add(company);
                _dataContextModel.SaveChanges();
            }

            SetupEntryDisplayMode();
            _dataContextModel.SaveChanges();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Entry ? \n this cannot be undone",
                "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                    break;
                case MessageBoxResult.Cancel:
                    break;
                case MessageBoxResult.Yes:
                    DeleteCompany(Convert.ToInt32(id.Content));
                    break;
                case MessageBoxResult.No:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DeleteCompany(int c_id)
        {
            var c = _dataContextModel.Companies.FirstOrDefault(x => x.Id == c_id);
            if (c != null) _dataContextModel.Companies.Remove(c);

            _dataContextModel.SaveChanges();
        }
    }
}

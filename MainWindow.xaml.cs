using System.Data.Entity;
using System.Linq;
using System.Windows;
using E_Warehouse.Data;
using E_Warehouse.Models;
using E_Warehouse.Utils;
using E_Warehouse.Views;

namespace E_Warehouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //on start, load the search view
            MenuItem_Search_OnClick(null, null);
        }

        /// <summary>
        /// Exit the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Open a search window where the user enters a part number to get its info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Search_OnClick(object sender, RoutedEventArgs e)
        {
            OpenItemSearchView();
        }

        public void OpenItemSearchView(params string[] preparedItems)
        {
            var context = new SearchItem();
            context.PutItem(preparedItems);

            DataContext = context;
        }

        private void MenuItem_DB_AllCompanies_OnClick(object sender, RoutedEventArgs e)
        {
            DataContext = new AllCompanies();
        }

        private void MenuItem_DB_Items_OnClick(object sender, RoutedEventArgs e)
        {
            OpenItemsView();
        }

        private void OpenItemsView(params Item[] preparedItems)
        {
            if(preparedItems != null && preparedItems.Length > 0) DataContext = new Items(preparedItems);
            else DataContext = new Items();
        }

        /// <summary>
        /// Extract items from an excel sheet and search for them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelHelper.OpenProcessFile(SearchDoneCallback);
        }

        private void SearchDoneCallback(Item[] obj)
        {
            OpenItemSearchView(obj.Select(x => x.PartNumber).ToArray());
        }

        /// <summary>
        /// Import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportExcel_OnClick(object sender, RoutedEventArgs e)
        {
            ExcelHelper.OpenProcessFile(ImportDoneCallback);
        }

        private void ImportDoneCallback(Item[] obj)
        {
            OpenItemsView(obj);
        }
    }
}

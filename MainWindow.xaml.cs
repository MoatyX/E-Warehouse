using System.Data.Entity;
using System.Windows;
using E_Warehouse.Data;
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
            DataContext = new SearchItem();
        }

        private void MenuItem_DB_AllCompanies_OnClick(object sender, RoutedEventArgs e)
        {
            DataContext = new AllCompanies();
        }

        private void MenuItem_DB_Items_OnClick(object sender, RoutedEventArgs e)
        {
            DataContext = new Items();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExcelHelper.OpenProcessFile();
        }
    }
}

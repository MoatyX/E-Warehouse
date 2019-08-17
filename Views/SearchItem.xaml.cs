using System;
using System.Collections.Generic;
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
using E_Warehouse.Utils;

namespace E_Warehouse.Views
{
    /// <summary>
    /// Interaction logic for SearchItem.xaml
    /// </summary>
    public partial class SearchItem : UserControl
    {
        public SearchItem()
        {
            InitializeComponent();
        }

        public void PutItem(params string[] items)
        {
            SearchTextbox.Text = string.Join(",", items);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = SearchTextbox.Text;
            text.RemoveWhitespace();

            string[] subStrings = text.Split(',');
            Application.Current.MainWindow.DataContext = new Items(subStrings);

        }
    }
}

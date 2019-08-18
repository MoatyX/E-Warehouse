using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using E_Warehouse.Models;

namespace E_Warehouse.Data
{
    public sealed class DataModelRepo
    {
        private static DataModelRepo _instance;
        private static readonly object PadLock = new object();

        public static DataModelRepo Instance
        {
            get
            {
                lock (PadLock)
                {
                    return _instance ?? (_instance = new DataModelRepo());
                }
            }
        }

        private DataModelRepo()
        {
            ItemModel = new ItemModel();
        }

        public ItemModel ItemModel { get; set; }

        private bool _loadedCompanies;
        public ObservableCollection<Company> Companies
        {
            get
            {
                if (!_loadedCompanies)
                {
                    ItemModel.Companies.Include(x => x.Items).Load();
                    _loadedCompanies = true;
                }
                return ItemModel.Companies.Local;
            }
        }

        private bool _loadedItems;
        public ObservableCollection<Item> Items
        {
            get
            {
                if (!_loadedItems)
                {
                    ItemModel.Items.
                        Include(x => x.Transactions.Select(y => y.Company)).Load();
                    _loadedItems = true;
                }

                var xx = ItemModel.Items.ToList();
                Console.WriteLine(xx);
                return ItemModel.Items.Local;
            }
        }

        private bool _loadedItemStats;
        public ObservableCollection<ItemStatistic> ItemStatistics
        {
            get
            {
                if (!_loadedItemStats)
                {
                    _loadedItemStats = true;
                    ItemModel.ItemStatistics.Load();
                }
                return ItemModel.ItemStatistics.Local;
            }
        }

        public void RemoveItem(Item item)
        {
            ItemModel.Items.Remove(item);
        }

        public void AddItem(Item item)
        {
            ItemModel.Items.Add(item);
        }

        public void UpdateItemStats(Item item)
        {
            var stat = item.ItemStatistic;

            var sellCollection = item.Transactions
                .Where(x => x.ItemTransactionType == ItemTransaction.TransactionType.Sell).ToArray();
            var buyCollection = item.Transactions
                .Where(x => x.ItemTransactionType == ItemTransaction.TransactionType.Buy).ToArray();

            if (buyCollection.Any())
            {
                stat.AvgBuyingPrice = buyCollection.Average(x => x.Price);
                stat.LowestBuyingPrice = buyCollection.Min(x => x.Price);
                stat.LowestBuyingCompany = item.Transactions.First(x => x.Price.Equals(stat.LowestBuyingPrice)).Company;
            }

            if(sellCollection.Any())
                stat.AvgSellingPrice = sellCollection.Average(x => x.Price);
        }

        public int SaveChanges()
        {
            try
            {
                return ItemModel.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(e.Message, "Saving Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("Changes Will not be saved", "Saving Error", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            return 0;
        }
    }
}

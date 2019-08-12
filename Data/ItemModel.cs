using E_Warehouse.Models;

namespace E_Warehouse.Data
{
    using System.Data.Entity;

    public class ItemModel : DbContext
    {
        public ItemModel() : base("name=ItemModel")
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ItemTransaction> ItemTransactions { get; set; }
    }
}
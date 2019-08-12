using System;
using System.Collections.Generic;

namespace E_Warehouse.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PartNumber { get; set; }
        public float BuyPrice { get; set; }
        public float SellPrice { get; set; }
        public int Quantity { get; set; }

        public ICollection<Company> SourceCompanies { get; set; }

        public ICollection<ItemTransaction> Transactions { get; set; }
    }
}

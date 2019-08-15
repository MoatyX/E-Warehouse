using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Warehouse.Models
{
    public class Item
    {
        public Item()
        {
            Transactions = new List<ItemTransaction>();
            SourceCompanies = new List<Company>();
        }

        /// <summary>
        /// A constructor used solely by the UI when creating a new Item
        /// </summary>
        /// <param name="name"></param>
        public Item(string name) : this()
        {
            Name = name;
            ItemStatistic = new ItemStatistic
            {
                Item = this,
            };
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public double SellPrice { get; set; }
        public int Quantity { get; set; }

        public virtual ItemStatistic ItemStatistic { get; set; }

        public ICollection<Company> SourceCompanies { get; set; }

        public ICollection<ItemTransaction> Transactions { get; set; }
    }
}

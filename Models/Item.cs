using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public string Name { get; set; }

        [Required(AllowEmptyStrings = false), Key]
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public double SellPrice { get; set; }
        public int Quantity { get; set; }

        public virtual ItemStatistic ItemStatistic { get; set; }

        public ICollection<Company> SourceCompanies { get; set; }

        public ICollection<ItemTransaction> Transactions { get; set; }

        public override string ToString()
        {
            string output = $"(Item: {PartNumber}, Sell Price: {SellPrice}, Quantity: {Quantity})";
            return output;
        }
    }
}

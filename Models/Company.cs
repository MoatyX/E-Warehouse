using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Warehouse.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSourceCompany { get; set; }

        /// <summary>
        /// the total amount of items that a company has
        /// </summary>
        public ICollection<Item> Items { get; set; }
    }
}

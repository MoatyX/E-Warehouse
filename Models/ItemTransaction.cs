using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Warehouse.Models
{
    public class ItemTransaction
    {
        public enum TransactionType
        {
            Sell,
            Buy
        }

        public int Id { get; set; }

        public Item Item { get; set; }
        public int ItemId { get; set; }

        public Company Company { get; set; }
        public int CompanyId { get; set; }

        public TransactionType ItemTransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
    }
}

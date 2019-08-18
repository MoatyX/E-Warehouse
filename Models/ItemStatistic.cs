using System.ComponentModel.DataAnnotations.Schema;

namespace E_Warehouse.Models
{
    public class ItemStatistic
    {
        [ForeignKey("Item")]
        public string Id { get; set; }

        public int ItemStatisticId { get; set; }

        public virtual Item Item { get; set; }

        public double AvgSellingPrice { get; set; }
        public double AvgBuyingPrice { get; set; }
        public double LowestBuyingPrice { get; set; }

        public virtual Company LowestBuyingCompany { get; set; }
    }
}

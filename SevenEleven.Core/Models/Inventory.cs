
namespace SevenEleven.Core.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int Quantity { get; set; }
        public int MinimumStock { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool IsBelowMinimum()
        {
            return Quantity < MinimumStock;
        }
    }
}

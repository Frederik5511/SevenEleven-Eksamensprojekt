
namespace SevenEleven.Core.Models
{
    public class InventoryOverview
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Barcode { get; set; } = "";
        public int Quantity { get; set; }
        public int MinimumStock { get; set; }
        public DateTime LastUpdated { get; set; }

        public bool IsBelowMinimum =>
            Quantity < MinimumStock;
    }
}

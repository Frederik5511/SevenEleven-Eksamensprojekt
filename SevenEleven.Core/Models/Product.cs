
namespace SevenEleven.Core.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public string Barcode { get; set; } = "";
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int ShelfLife { get; set; }
        public bool IsSeasonal { get; set; }

        public int? ProductCategoryId { get; set; }
    }
}

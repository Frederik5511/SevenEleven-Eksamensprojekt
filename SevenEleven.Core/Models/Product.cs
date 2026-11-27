
namespace SevenEleven.Core.Models
{
    public class Product
    {
        public int ProductId { get; private set; }
        public string Name { get; private set; } = "";
        public string Barcode { get; private set; } = "";
        public decimal PurchasePrice { get; private set; }
        public decimal SellingPrice { get; private set; }
        public int ShelfLife { get; private set; }
        public bool IsSeasonal { get; private set; }

        public int? ProductCategoryId { get; private set; }

        public Product (string name, string barcode, decimal purchasePrice, decimal sellingPrice, int shelfLife, bool isSeasonal, int? productCategoryId = null)
        {
            Name = name;
            Barcode = barcode;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
            ShelfLife = shelfLife;
            IsSeasonal = isSeasonal;
            ProductCategoryId = productCategoryId;
        }
    }
}

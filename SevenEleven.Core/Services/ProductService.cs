using System;
using SevenEleven.Core.Repositories;
using SevenEleven.Core.Models;

namespace SevenEleven.Core.Services
{

    public class ProductService
    {
        private readonly ProductRepository _repository;


        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        public Product CreateProduct(string name, string barcode, decimal purchasePrice, decimal sellingPrice, int shelfLife, bool isSeasonal, int? productCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            {
                throw new ArgumentException("Produktnavnet skal være udfyldt og må højst være 100 tegn");
            }

            if (purchasePrice < 0)
            {
                throw new ArgumentOutOfRangeException(null, message: "Indkøbspris må ikke være negativ");
            }
            
            if (sellingPrice < 0)
            {
                throw new ArgumentOutOfRangeException(null, message: "Salgspris må ikke være negativ");
            }
            
            if (shelfLife < 1)
            {
                throw new ArgumentOutOfRangeException(null, message: "Holdbarheden skal være mindst 1 dag");
            }

            if (string.IsNullOrWhiteSpace(barcode) || barcode.Length > 50)
            {
                throw new ArgumentException("Stregkoden skal være udfyldt og må højest være 50 tegn");
            } 
            if (_repository.ExistsByBarcode(barcode))
            {
                throw new ArgumentException("Stregkoden er allerede i brug");
            }

            Product product = new Product(name, barcode, purchasePrice, sellingPrice, shelfLife, isSeasonal, productCategoryId);
            _repository.Add(product);
            return product;
        }
    }
}

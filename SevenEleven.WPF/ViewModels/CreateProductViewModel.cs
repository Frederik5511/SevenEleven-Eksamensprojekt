using System;
using System.Collections.Generic;
using System.Text;
using SevenEleven.Core.Models;
using SevenEleven.Core.Services;
using SevenEleven.WPF.Commands;
using System.Windows.Input;
using System.ComponentModel;

namespace SevenEleven.WPF.ViewModels
{
    public class CreateProductViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public string Barcode { get; set; } = "";
        public bool IsSeasonal { get; set; }
        public string PurchasePriceText { get; set; } = "";
        public string SellingPriceText { get; set; } = "";
        public string ShelfLifeText { get; set; } = "";
        public List<ProductCategory> Categories { get; } = new();
        public int? ProductCategoryId { get; set; }
        public ICommand SaveCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public string ErrorMessage { get; private set; } = "";
        public string SuccessMessage { get; private set; } = "";


        private readonly ProductService _productService;

        public CreateProductViewModel(ProductService productService)
        {
            _productService = productService;
            SaveCommand = new RelayCommand(Save);
        }
        private void SetErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }

        private void SetSuccessMessage(string successMessage)
        {
            SuccessMessage = successMessage;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SuccessMessage)));
        }

        private void Save()
        {
            SetErrorMessage("");
            SetSuccessMessage("");
            if (!decimal.TryParse(PurchasePriceText, out decimal purchasePrice))
            {
                SetErrorMessage("Indtast en gyldig indkøbspris.");
                return;
            }

            if (!decimal.TryParse(SellingPriceText, out decimal sellingPrice))
            {
                SetErrorMessage("Indtast en gyldig salgspris.");
                return;
            }

            if (!int.TryParse(ShelfLifeText, out int shelfLife))
            {
                SetErrorMessage("Indtast gyldig holdbarhed.");
                return;
            }
            try
            {
                _productService.CreateProduct(
                    Name, Barcode, purchasePrice, sellingPrice,
                    shelfLife, IsSeasonal, ProductCategoryId);
                SetSuccessMessage($"Produktet {Name} er oprettet. Du kan nu lukke vinduet eller indtaste en ny vare.");
            }
            catch (ArgumentException ex)
            {
                SetErrorMessage(ex.Message);
            }
        }
    }
}

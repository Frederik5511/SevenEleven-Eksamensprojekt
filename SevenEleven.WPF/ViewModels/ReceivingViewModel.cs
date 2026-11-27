using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;
using SevenEleven.WPF.Commands;

namespace SevenEleven.WPF.ViewModels;

public class ReceivingViewModel : INotifyPropertyChanged
{
    private const int StoreId = 1;
    private readonly InventoryRepository _repository;
    private InventoryOverview? _selectedProduct;
    private string _quantityText = "1";
    private string _message = "";
    private bool _isBusy;

    public ObservableCollection<InventoryOverview> Products { get; } = new();
    public ICommand RefreshCommand { get; }
    public ICommand RegisterReceiptCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public InventoryOverview? SelectedProduct
    {
        get => _selectedProduct;
        set { _selectedProduct = value; Changed(nameof(SelectedProduct)); }
    }

    public string QuantityText
    {
        get => _quantityText;
        set { _quantityText = value; Changed(nameof(QuantityText)); }
    }

    public string Message
    {
        get => _message;
        private set { _message = value; Changed(nameof(Message)); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; Changed(nameof(IsBusy)); }
    }

    public ReceivingViewModel(InventoryRepository repository)
    {
        _repository = repository;
        RefreshCommand = new RelayCommand(LoadProducts);
        RegisterReceiptCommand = new RelayCommand(RegisterReceipt);
        LoadProducts();
    }

    private void LoadProducts()
    {
        try
        {
            var selectedId = SelectedProduct?.ProductId;
            Products.Clear();
            foreach (var product in _repository.GetInventoryOverview(StoreId))
                Products.Add(product);
            SelectedProduct = selectedId is null ? null : Products.FirstOrDefault(p => p.ProductId == selectedId);
            Message = $"{Products.Count} registrerede produkter hentet. Vælg det produkt, I har modtaget.";
        }
        catch (Exception ex)
        {
            Message = $"Kunne ikke hente produkter fra databasen: {ex.Message}";
        }
    }

    private void RegisterReceipt()
    {
        if (SelectedProduct is null)
        {
            Message = "Vælg et eksisterende produkt fra listen. Produkter, der ikke findes, kan ikke modtages.";
            return;
        }

        if (!int.TryParse(QuantityText, out var quantity) || quantity <= 0)
        {
            Message = "Antal skal være et heltal større end 0.";
            return;
        }

        IsBusy = true;
        try
        {
            var productId = SelectedProduct.ProductId;
            var productName = SelectedProduct.ProductName;
            var newStock = _repository.RegisterGoodsReceipt(StoreId, productId, quantity);
            LoadProducts();
            SelectedProduct = Products.FirstOrDefault(p => p.ProductId == productId);
            Message = $"Modtagelse registreret: {quantity} x {productName}. Nyt lagerantal: {newStock}.";
        }
        catch (Exception ex)
        {
            Message = $"Varemodtagelsen kunne ikke registreres: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Changed(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

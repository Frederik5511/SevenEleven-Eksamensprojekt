using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;
using SevenEleven.WPF.Commands;

namespace SevenEleven.WPF.ViewModels;

public class OrderViewModel : INotifyPropertyChanged
{
    private readonly InventoryRepository _inventory;
    private readonly OrderRepository _orders;
    private Supplier? _selectedSupplier;
    private InventoryOverview? _selectedProduct;
    private OrderDraftLine? _selectedLine;
    private string _quantityText = "1";
    private DateTime? _deliveryDate = DateTime.Today.AddDays(1);
    private string _message = "";
    private bool _isBusy;
    public ObservableCollection<InventoryOverview> Products { get; } = new();
    public ObservableCollection<Supplier> Suppliers { get; } = new();
    public ObservableCollection<OrderDraftLine> Lines { get; } = new();
    public ICommand AddLineCommand { get; }
    public ICommand RemoveLineCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand UseSuggestedQuantityCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public Supplier? SelectedSupplier { get => _selectedSupplier; set { _selectedSupplier = value; Changed(nameof(SelectedSupplier)); } }
    public InventoryOverview? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            Changed(nameof(SelectedProduct));
            Changed(nameof(SuggestedQuantity));
        }
    }
    public OrderDraftLine? SelectedLine { get => _selectedLine; set { _selectedLine = value; Changed(nameof(SelectedLine)); } }
    public string QuantityText { get => _quantityText; set { _quantityText = value; Changed(nameof(QuantityText)); } }
    public DateTime? DeliveryDate { get => _deliveryDate; set { _deliveryDate = value; Changed(nameof(DeliveryDate)); } }
    public string Message { get => _message; private set { _message = value; Changed(nameof(Message)); } }
    public int SuggestedQuantity => SelectedProduct is null ? 0 : Math.Max(0, SelectedProduct.MinimumStock - SelectedProduct.Quantity);
    public bool IsBusy { get => _isBusy; private set { _isBusy = value; Changed(nameof(IsBusy)); } }
    public decimal Total => Lines.Sum(l => l.LineTotal);

    public OrderViewModel(InventoryRepository inventory, OrderRepository orders)
    {
        _inventory = inventory; _orders = orders;
        AddLineCommand = new RelayCommand(AddLine);
        RemoveLineCommand = new RelayCommand(RemoveLine);
        SaveCommand = new RelayCommand(Save);
        RefreshCommand = new RelayCommand(LoadProducts);
        UseSuggestedQuantityCommand = new RelayCommand(() =>
        {
            if (SuggestedQuantity > 0) QuantityText = SuggestedQuantity.ToString();
            else Message = "Produktet er på eller over minimumsbeholdningen.";
        });
        Lines.CollectionChanged += (_, _) => Changed(nameof(Total));
        Load();
    }

    private void Load()
    {
        try
        {
            foreach (var supplier in _orders.GetSuppliers()) Suppliers.Add(supplier);
            SelectedSupplier = Suppliers.FirstOrDefault();
            LoadProducts();
        }
        catch (Exception ex) { Message = $"Kunne ikke hente data fra databasen: {ex.Message}"; }
    }

    private void LoadProducts()
    {
        try
        {
            Products.Clear();
            foreach (var product in _inventory.GetInventoryOverview(1)) Products.Add(product);
            Message = Products.Count == 0 ? "Ingen produkter med lagerregistrering fundet for butikken." : $"{Products.Count} produkter indlæst. Produkter under minimum får et foreslået bestillingsantal.";
        }
        catch (Exception ex) { Message = $"Kunne ikke hente lagerdata: {ex.Message}"; }
    }

    private void AddLine()
    {
        if (SelectedProduct is null) { Message = "Vælg et produkt først."; return; }
        if (!int.TryParse(QuantityText, out var quantity) || quantity <= 0) { Message = "Antal skal være et heltal større end 0."; return; }
        var existing = Lines.FirstOrDefault(l => l.ProductId == SelectedProduct.ProductId);
        if (existing is not null) { existing.Quantity += quantity; Lines.Remove(existing); Lines.Add(existing); }
        else Lines.Add(new OrderDraftLine { ProductId = SelectedProduct.ProductId, ProductName = SelectedProduct.ProductName, Barcode = SelectedProduct.Barcode, StockQuantity = SelectedProduct.Quantity, MinimumStock = SelectedProduct.MinimumStock, UnitPrice = SelectedProduct.PurchasePrice, Quantity = quantity });
        Changed(nameof(Total)); Message = $"{SelectedProduct.ProductName} tilføjet til bestillingen.";
    }

    private void RemoveLine()
    {
        if (SelectedLine is not null) Lines.Remove(SelectedLine);
    }

    private void Save()
    {
        if (SelectedSupplier is null) { Message = "Vælg en leverandør."; return; }
        if (Lines.Count == 0) { Message = "Tilføj mindst ét produkt, før bestillingen kan gemmes."; return; }
        IsBusy = true;
        try
        {
            var id = _orders.CreateOrder(1, SelectedSupplier.SupplierId, DeliveryDate, Lines.ToList());
            Lines.Clear();
            SelectedLine = null;
            Changed(nameof(Total));
            Message = $"Bestilling #{id} er bekræftet og gemt med status 'Oprettet'. Lageret ændres først ved varemodtagelse.";
        }
        catch (Exception ex) { Message = $"Bestillingen kunne ikke gemmes: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    private void Changed(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}

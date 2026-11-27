namespace SevenEleven.Core.Models;

public class OrderDraftLine
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public string Barcode { get; init; } = "";
    public int StockQuantity { get; init; }
    public int MinimumStock { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

using Microsoft.Data.SqlClient;
using SevenEleven.Core.Models;

namespace SevenEleven.Core.Repositories;

public class OrderRepository(string connectionString)
{
    public List<Supplier> GetSuppliers()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT SupplierId, Name FROM Supplier ORDER BY Name", connection);
        using var reader = command.ExecuteReader();
        var suppliers = new List<Supplier>();
        while (reader.Read()) suppliers.Add(new Supplier { SupplierId = reader.GetInt32(0), Name = reader.GetString(1) });
        return suppliers;
    }

    public int CreateOrder(int storeId, int supplierId, DateTime? deliveryDate, IReadOnlyCollection<OrderDraftLine> lines)
    {
        if (lines.Count == 0) throw new InvalidOperationException("Tilføj mindst ét produkt til bestillingen.");
        if (lines.Any(line => line.Quantity <= 0)) throw new InvalidOperationException("Alle antal skal være hele tal større end 0.");
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var line in lines)
            {
                using var productCheck = new SqlCommand(@"SELECT COUNT(1) FROM Inventory
WHERE ProductId = @ProductId AND StoreId = @StoreId", connection, transaction);
                productCheck.Parameters.AddWithValue("@ProductId", line.ProductId);
                productCheck.Parameters.AddWithValue("@StoreId", storeId);
                if ((int)productCheck.ExecuteScalar()! == 0)
                    throw new InvalidOperationException($"Produktet '{line.ProductName}' findes ikke længere i butikkens lager. Opdater produktlisten og prøv igen.");
            }

            using (var supplierCheck = new SqlCommand("SELECT COUNT(1) FROM Supplier WHERE SupplierId = @SupplierId", connection, transaction))
            {
                supplierCheck.Parameters.AddWithValue("@SupplierId", supplierId);
                if ((int)supplierCheck.ExecuteScalar()! == 0)
                    throw new InvalidOperationException("Den valgte leverandør findes ikke længere. Genstart bestillingen og vælg en gyldig leverandør.");
            }

            using var orderCommand = new SqlCommand(@"INSERT INTO [Order](SupplierId, StoreId, OrderDate, ExpectedDeliveryDate, Status)
OUTPUT INSERTED.OrderId VALUES (@SupplierId, @StoreId, SYSDATETIME(), @DeliveryDate, N'Oprettet')", connection, transaction);
            orderCommand.Parameters.AddWithValue("@SupplierId", supplierId);
            orderCommand.Parameters.AddWithValue("@StoreId", storeId);
            orderCommand.Parameters.AddWithValue("@DeliveryDate", (object?)deliveryDate ?? DBNull.Value);
            var orderId = (int)orderCommand.ExecuteScalar()!;

            foreach (var line in lines)
            {
                if (line.Quantity <= 0) throw new InvalidOperationException($"Antallet for {line.ProductName} skal være større end 0.");
                using var lineCommand = new SqlCommand(@"INSERT INTO OrderLine(OrderId, ProductId, Quantity, UnitPrice)
VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)", connection, transaction);
                lineCommand.Parameters.AddWithValue("@OrderId", orderId);
                lineCommand.Parameters.AddWithValue("@ProductId", line.ProductId);
                lineCommand.Parameters.AddWithValue("@Quantity", line.Quantity);
                lineCommand.Parameters.AddWithValue("@UnitPrice", line.UnitPrice);
                lineCommand.ExecuteNonQuery();
            }
            transaction.Commit();
            return orderId;
        }
        catch { transaction.Rollback(); throw; }
    }
}

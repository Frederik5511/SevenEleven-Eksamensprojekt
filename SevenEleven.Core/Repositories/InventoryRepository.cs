
using Microsoft.Data.SqlClient;
using SevenEleven.Core.Models;

namespace SevenEleven.Core.Repositories
{
    public class InventoryRepository
    {
        private readonly string _connectionString;

        public InventoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<Inventory> GetInventoryByStore(int storeId)
        {
            List<Inventory> inventories = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            string sql = @"
        SELECT InventoryId,
               ProductId,
               StoreId,
               Quantity,
               MinimumStock,
               LastUpdated
        FROM Inventory
        WHERE StoreId = @StoreId";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add(
                "@StoreId", System.Data.SqlDbType.Int
            ).Value = storeId;

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Inventory inventory = new Inventory
                {
                    InventoryId = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    StoreId = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    MinimumStock = reader.GetInt32(4),
                    LastUpdated = reader.GetDateTime(5)
                };

                inventories.Add(inventory);
            }

            return inventories;
        }


        public List<InventoryOverview> GetInventoryOverview(int storeId)
        {
            List<InventoryOverview> overview = new();

            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            string sql = @"
        SELECT
            p.ProductId,
            p.Name,
            p.Barcode,
            i.Quantity,
            i.MinimumStock,
            i.LastUpdated,
            p.PurchasePrice
        FROM Inventory i
        INNER JOIN Product p
            ON i.ProductId = p.ProductId
        WHERE i.StoreId = @StoreId
        ORDER BY p.Name";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.Add(
                "@StoreId", System.Data.SqlDbType.Int
            ).Value = storeId;

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                InventoryOverview item = new InventoryOverview
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Barcode = reader.IsDBNull(2)
                        ? "" : reader.GetString(2),
                    Quantity = reader.GetInt32(3),
                    MinimumStock = reader.GetInt32(4),
                    LastUpdated = reader.GetDateTime(5),
                    PurchasePrice = reader.IsDBNull(6) ? 0m : reader.GetDecimal(6)
                };

                overview.Add(item);
            }

            return overview;
        }

        public int RegisterGoodsReceipt(int storeId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Antallet skal være et heltal større end 0.");

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using (SqlCommand productCommand = new SqlCommand(
                    "SELECT COUNT(1) FROM Product WHERE ProductId = @ProductId",
                    connection, transaction))
                {
                    productCommand.Parameters.Add("@ProductId", System.Data.SqlDbType.Int).Value = productId;
                    if ((int)productCommand.ExecuteScalar()! == 0)
                        throw new InvalidOperationException("Produktet findes ikke. Varemodtagelsen blev ikke registreret.");
                }

                int updatedQuantity;
                using (SqlCommand updateCommand = new SqlCommand(@"
UPDATE Inventory WITH (UPDLOCK, SERIALIZABLE)
SET Quantity = Quantity + @Quantity,
    LastUpdated = SYSDATETIME()
OUTPUT INSERTED.Quantity
WHERE ProductId = @ProductId AND StoreId = @StoreId;", connection, transaction))
                {
                    updateCommand.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = quantity;
                    updateCommand.Parameters.Add("@ProductId", System.Data.SqlDbType.Int).Value = productId;
                    updateCommand.Parameters.Add("@StoreId", System.Data.SqlDbType.Int).Value = storeId;
                    object? result = updateCommand.ExecuteScalar();
                    if (result is null || result == DBNull.Value)
                    {
                        using SqlCommand insertCommand = new SqlCommand(@"
INSERT INTO Inventory(ProductId, StoreId, Quantity)
OUTPUT INSERTED.Quantity
VALUES (@ProductId, @StoreId, @Quantity);", connection, transaction);
                        insertCommand.Parameters.Add("@ProductId", System.Data.SqlDbType.Int).Value = productId;
                        insertCommand.Parameters.Add("@StoreId", System.Data.SqlDbType.Int).Value = storeId;
                        insertCommand.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = quantity;
                        result = insertCommand.ExecuteScalar();
                    }
                    updatedQuantity = (int)result!;
                }

                transaction.Commit();
                return updatedQuantity;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}

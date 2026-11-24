
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
            i.LastUpdated
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
                    LastUpdated = reader.GetDateTime(5)
                };

                overview.Add(item);
            }

            return overview;
        }

    }
}

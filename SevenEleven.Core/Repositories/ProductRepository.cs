using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using SevenEleven.Core.Models;

namespace SevenEleven.Core.Repositories
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ExistsByBarcode(string barcode)
        {
            string sql =
                "SELECT COUNT (*) " +
                "FROM dbo.Product " +
                "WHERE Barcode = @Barcode;";
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 50).Value = barcode;

            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());

            return count > 0;
        }

        public void Add(Product product)
        {
            string sql =
                "INSERT INTO dbo.Product " +
                "(Name, Barcode, PurchasePrice, SellingPrice, ShelfLife, IsSeasonal, ProductCategoryId) " +
                "VALUES (@Name, @Barcode, @PurchasePrice, @SellingPrice, @ShelfLife, @IsSeasonal, @ProductCategoryId);";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = product.Name;
            command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 50).Value = product.Barcode;
            command.Parameters.Add("@PurchasePrice", SqlDbType.Decimal).Value = product.PurchasePrice;
            command.Parameters.Add("@SellingPrice", SqlDbType.Decimal).Value = product.SellingPrice;
            command.Parameters.Add("@ShelfLife", SqlDbType.Int).Value = product.ShelfLife;
            command.Parameters.Add("@IsSeasonal", SqlDbType.Bit).Value = product.IsSeasonal;
            command.Parameters.Add("@ProductCategoryId", SqlDbType.Int).Value = (object?)product.ProductCategoryId ?? DBNull.Value;

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}

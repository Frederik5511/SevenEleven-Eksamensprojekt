using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SevenEleven.Core.Repositories
{
    internal class ProductRepository
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
    }
}

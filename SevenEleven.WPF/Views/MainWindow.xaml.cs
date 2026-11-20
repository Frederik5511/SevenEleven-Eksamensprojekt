
using System;
using System.Windows;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;

namespace SevenEleven.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string connectionString =
                "Server=localhost;" +
                "Database=SevenElevenDB;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";

            InventoryRepository repository =
                new InventoryRepository(connectionString);

            try
            {
                List<InventoryOverview> inventory =
                    repository.GetInventoryOverview(1);

                foreach (InventoryOverview item in inventory)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"{item.ProductName}: {item.Quantity} stk."
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Databasefejl: {ex.Message}"
                );
            }
        }
    }
}

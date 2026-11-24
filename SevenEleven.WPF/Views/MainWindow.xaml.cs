
using System.Windows;
using SevenEleven.Core.Repositories;
using SevenEleven.WPF.ViewModels;

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

            InventoryViewModel viewModel =
                new InventoryViewModel(repository);

            DataContext = viewModel;

            viewModel.LoadInventory(1);
        }
    }
}

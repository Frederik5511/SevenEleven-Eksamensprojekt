
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

            InventoryView.DataContext = viewModel;
            OrderView.DataContext = new OrderViewModel(repository, new OrderRepository(connectionString));
            ReceivingView.DataContext = new ReceivingViewModel(repository);

            viewModel.LoadInventory(1);
        }
    }
}

using System.Windows;
using SevenEleven.Core.Repositories;
using SevenEleven.WPF.ViewModels;
using SevenEleven.WPF.Views;
using SevenEleven.Core.Services;

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
                new InventoryViewModel(repository, () => OpenCreateProduct(connectionString));

            DataContext = viewModel;

            viewModel.LoadInventory(1);
        }
        private void OpenCreateProduct(string connectionString)
        {
            ProductRepository productRepository = new ProductRepository(connectionString);
            ProductService productService = new ProductService(productRepository);
            CreateProductViewModel formViewModel = new CreateProductViewModel(productService);
            formViewModel.Categories.AddRange(productRepository.GetCategories());

            CreateProductView window = new CreateProductView();
            window.Owner = this;
            window.DataContext = formViewModel;
            window.ShowDialog();
        }
    }
}

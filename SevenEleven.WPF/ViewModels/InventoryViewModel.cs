
using System.Collections.ObjectModel;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;

namespace SevenEleven.WPF.ViewModels
{
    public class InventoryViewModel
    {
        private readonly InventoryRepository _repository;

        public ObservableCollection<InventoryOverview> InventoryItems
        { get; set; } = new();

        public InventoryViewModel(InventoryRepository repository)
        {
            _repository = repository;
        }

        public void LoadInventory(int storeId)
        {
            InventoryItems.Clear();

            List<InventoryOverview> items =
                _repository.GetInventoryOverview(storeId);

            foreach (InventoryOverview item in items)
            {
                InventoryItems.Add(item);
            }
        }
    }
}

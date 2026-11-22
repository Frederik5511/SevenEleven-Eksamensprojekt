
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;

namespace SevenEleven.WPF.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _repository;
        private string _searchText = "";

        public ObservableCollection<InventoryOverview> InventoryItems
        { get; } = new();

        public ICollectionView FilteredInventory { get; }

        public string SearchText
        {
            get => _searchText;

            set
            {
                if (_searchText == value)
                    return;

                _searchText = value ?? "";

                OnPropertyChanged(nameof(SearchText));
                FilteredInventory.Refresh();
            }
        }

        public InventoryViewModel(InventoryRepository repository)
        {
            _repository = repository;

            FilteredInventory =
                CollectionViewSource.GetDefaultView(InventoryItems);

            FilteredInventory.Filter = FilterItem;
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

        private bool FilterItem(object obj)
        {
            if (obj is not InventoryOverview item)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return item.ProductName.Contains(
                       SearchText,
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   item.Barcode.Contains(
                       SearchText,
                       StringComparison.OrdinalIgnoreCase);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}

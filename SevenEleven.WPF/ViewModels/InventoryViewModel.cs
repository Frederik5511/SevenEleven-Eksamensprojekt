using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using SevenEleven.Core.Models;
using SevenEleven.Core.Repositories;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using SevenEleven.WPF.Commands;

namespace SevenEleven.WPF.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly InventoryRepository _repository;
        private string _searchText = "";

        private InventoryOverview? _selectedInventory;

        public ObservableCollection<InventoryOverview> InventoryItems
        { get; } = new();

        public ICollectionView FilteredInventory { get; }
        public string EmptyMessage { get; private set; }
            = "Ingen produkter fundet";

        public ICommand RefreshCommand { get; }
        public ICommand OpenCreateProductCommand { get; }
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

        public InventoryViewModel(InventoryRepository repository, Action openCreateProduct)
        {
            _repository = repository;

            FilteredInventory =
                CollectionViewSource.GetDefaultView(InventoryItems);

            FilteredInventory.Filter = FilterItem;

            RefreshCommand = new RelayCommand(
                () => LoadInventory(1)
            );
            OpenCreateProductCommand = new RelayCommand(openCreateProduct);
        }

        public void LoadInventory(int storeId)
        {
            SelectedInventory = null;
            InventoryItems.Clear();

            EmptyMessage = "Ingen produkter fundet";
            OnPropertyChanged(nameof(EmptyMessage));

            try
            {
                List<InventoryOverview> items =
                    _repository.GetInventoryOverview(storeId);

                foreach (InventoryOverview item in items)
                {
                    InventoryItems.Add(item);
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                EmptyMessage =
                    "Kunne ikke hente lagerbeholdningen. " +
                    "Kontrollér forbindelsen til SQL Server.";

                OnPropertyChanged(nameof(EmptyMessage));
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
        public InventoryOverview? SelectedInventory
        {
            get => _selectedInventory;

            set
            {
                if (_selectedInventory == value)
                    return;

                _selectedInventory = value;
                OnPropertyChanged(nameof(SelectedInventory));
            }
        }

    }
}

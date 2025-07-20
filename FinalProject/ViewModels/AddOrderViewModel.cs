using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FinalProject.Models;
using FinalProject.ViewModels.Helpers;
using FinalProject.Models.Enum;

namespace FinalProject.ViewModels
{
    public class AddOrderViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<MenuItem> _allItems;
        private ObservableCollection<MenuItem> _filteredItems;
        private ObservableCollection<MenuCategory> _categories;
        private ObservableCollection<OrderItem> _orderItems;
        private ObservableCollection<Table> _tableList;
        private ObservableCollection<Account> _staffList;
        private MenuCategory _selectedCategory;
        private MenuItem _selectedMenuItem;
        private int _itemQuantity = 1;
        private decimal _totalAmount;
        private decimal _discount;
        private decimal _finalAmount;
        private Table _selectedTable;
        private Account _selectedStaff;
        private string _customerFirstName;

        public AddOrderViewModel()
        {
            _dbContext = new Prn212DbContext();
            AllItems = new ObservableCollection<MenuItem>(_dbContext.MenuItems.ToList());
            Categories = new ObservableCollection<MenuCategory>(_dbContext.MenuCategories.ToList());
            FilteredItems = new ObservableCollection<MenuItem>(AllItems);
            OrderItems = new ObservableCollection<OrderItem>();
            TableList = new ObservableCollection<Table>(_dbContext.Tables.ToList());
            StaffList = new ObservableCollection<Account>(_dbContext.Accounts.Where(a => a.Role == (int)AccountRole.Staff).ToList());
            FilterItemsCommand = new RelayCommand(_ => FilterItems());
            AddItemCommand = new RelayCommand(_ => AddItem(), _ => SelectedMenuItem != null && ItemQuantity > 0);
            RemoveItemCommand = new RelayCommand(item => RemoveItem(item as OrderItem), _ => true);
            SaveOrderCommand = new RelayCommand(_ => SaveOrder(), _ => OrderItems.Count > 0 && SelectedTable != null && SelectedStaff != null);
        }

        public ObservableCollection<MenuItem> AllItems
        {
            get => _allItems;
            set { _allItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MenuItem> FilteredItems
        {
            get => _filteredItems;
            set { _filteredItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MenuCategory> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public ObservableCollection<OrderItem> OrderItems
        {
            get => _orderItems;
            set { _orderItems = value; OnPropertyChanged(); UpdateSummary(); }
        }

        public ObservableCollection<Table> TableList
        {
            get => _tableList;
            set { _tableList = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Account> StaffList
        {
            get => _staffList;
            set { _staffList = value; OnPropertyChanged(); }
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set { _selectedTable = value; OnPropertyChanged(); (SaveOrderCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public Account SelectedStaff
        {
            get => _selectedStaff;
            set { _selectedStaff = value; OnPropertyChanged(); (SaveOrderCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        public MenuCategory SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); FilterItems(); }
        }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set { _selectedMenuItem = value; OnPropertyChanged(); }
        }

        public int ItemQuantity
        {
            get => _itemQuantity;
            set { _itemQuantity = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public decimal Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(); UpdateSummary(); }
        }

        public decimal FinalAmount
        {
            get => _finalAmount;
            set { _finalAmount = value; OnPropertyChanged(); }
        }

        public string CustomerFirstName
        {
            get => _customerFirstName;
            set { _customerFirstName = value; OnPropertyChanged(); }
        }

        public ICommand FilterItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveOrderCommand { get; }

        private void FilterItems()
        {
            if (SelectedCategory == null)
            {
                FilteredItems = new ObservableCollection<MenuItem>(AllItems);
            }
            else
            {
                FilteredItems = new ObservableCollection<MenuItem>(AllItems.Where(i => i.CategoryId == SelectedCategory.CategoryId));
            }
        }

        private void AddItem()
        {
            if (SelectedMenuItem == null || ItemQuantity <= 0) return;
            var existing = OrderItems.FirstOrDefault(oi => oi.ItemId == SelectedMenuItem.ItemId);
            if (existing != null)
            {
                existing.Quantity += ItemQuantity;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    ItemId = SelectedMenuItem.ItemId,
                    Item = SelectedMenuItem,
                    Quantity = ItemQuantity,
                    ItemPrice = SelectedMenuItem.Price
                });
            }
            UpdateSummary();
        }

        private void RemoveItem(OrderItem item)
        {
            if (item != null)
            {
                OrderItems.Remove(item);
                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            TotalAmount = OrderItems.Sum(oi => oi.Total);
            FinalAmount = TotalAmount - (TotalAmount * Discount / 100);
        }

        private void SaveOrder()
        {
            // Save logic to be implemented by parent
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
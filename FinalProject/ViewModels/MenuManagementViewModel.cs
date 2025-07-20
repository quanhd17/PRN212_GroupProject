using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FinalProject.Models;
using FinalProject.ViewModels.Helpers;
using Microsoft.Win32;

namespace FinalProject.ViewModels
{
    public class MenuManagementViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<MenuItem> _menuItemList;
        private MenuItem _selectedMenuItem;
        private string _itemName;
        private decimal _price;
        private string _description;
        private bool _isAvailable;
        private MenuCategory _selectedCategory;
        private ObservableCollection<MenuCategory> _categoryOptions;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;
        private string _itemImagePath;
        private MenuCategory _selectedCategoryFilter;
        private string _priceFilter;

        public MenuManagementViewModel()
        {
            _dbContext = new Prn212DbContext();
            MenuItemList = new ObservableCollection<MenuItem>();
            CategoryOptions = new ObservableCollection<MenuCategory>();
            LoadMenuItemsCommand = new RelayCommand(_ => LoadMenuItems());
            AddMenuItemCommand = new RelayCommand(_ => AddMenuItem(), _ => CanAddMenuItem());
            EditMenuItemCommand = new RelayCommand(_ => EditMenuItem(), _ => CanEditMenuItem());
            SaveMenuItemCommand = new RelayCommand(_ => SaveMenuItem(), _ => CanSaveMenuItem());
            DeleteMenuItemCommand = new RelayCommand(_ => DeleteMenuItem(), _ => CanDeleteMenuItem());
            CancelCommand = new RelayCommand(_ => Cancel());
            SearchCommand = new RelayCommand(_ => SearchMenuItem());
            UploadImageCommand = new RelayCommand(_ => UploadImage());
            FilterCommand = new RelayCommand(_ => FilterMenuItems());
            LoadMenuItems();
        }

        public ObservableCollection<MenuItem> MenuItemList
        {
            get => _menuItemList;
            set { _menuItemList = value; OnPropertyChanged(); }
        }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged();
                LoadMenuItemDetails();
                (EditMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string ItemName
        {
            get => _itemName;
            set { _itemName = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set { _isAvailable = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MenuCategory> CategoryOptions
        {
            get => _categoryOptions;
            set { _categoryOptions = value; OnPropertyChanged(); }
        }

        public MenuCategory SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsViewing));
                OnPropertyChanged(nameof(IsEditingOrAdding));
                (EditMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                _isAdding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsViewing));
                OnPropertyChanged(nameof(IsEditingOrAdding));
                (EditMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteMenuItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsViewing => !IsEditing && !IsAdding;
        public bool IsEditingOrAdding => IsEditing || IsAdding;

        public string ItemImagePath
        {
            get => _itemImagePath;
            set { _itemImagePath = value; OnPropertyChanged(); }
        }

        public MenuCategory SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set { _selectedCategoryFilter = value; OnPropertyChanged(); }
        }
        public string PriceFilter
        {
            get => _priceFilter;
            set { _priceFilter = value; OnPropertyChanged(); }
        }

        public ICommand LoadMenuItemsCommand { get; }
        public ICommand AddMenuItemCommand { get; }
        public ICommand EditMenuItemCommand { get; }
        public ICommand SaveMenuItemCommand { get; }
        public ICommand DeleteMenuItemCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand FilterCommand { get; }

        private void LoadMenuItems()
        {
            try
            {
                var items = _dbContext.MenuItems.ToList();
                MenuItemList.Clear();
                foreach (var item in items)
                {
                    MenuItemList.Add(item);
                }
                // Load categories
                var categories = _dbContext.MenuCategories.ToList();
                CategoryOptions.Clear();
                foreach (var cat in categories)
                {
                    CategoryOptions.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMenuItemDetails()
        {
            if (SelectedMenuItem != null)
            {
                ItemName = SelectedMenuItem.ItemName;
                Price = SelectedMenuItem.Price;
                Description = SelectedMenuItem.Description;
                IsAvailable = SelectedMenuItem.IsAvailable ?? true;
                SelectedCategory = SelectedMenuItem.Category;
                ItemImagePath = SelectedMenuItem.ItemUrl; // Use ItemUrl for image path
            }
        }

        private void AddMenuItem()
        {
            IsAdding = true;
            IsEditing = false;
            ItemName = string.Empty;
            Price = 0;
            Description = string.Empty;
            IsAvailable = true;
            SelectedCategory = CategoryOptions.FirstOrDefault();
            ItemImagePath = null;
        }

        private bool CanAddMenuItem() => IsViewing;

        private void EditMenuItem()
        {
            if (SelectedMenuItem == null)
            {
                MessageBox.Show("Please select a menu item to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            IsEditing = true;
            IsAdding = false;
            LoadMenuItemDetails();
        }

        private bool CanEditMenuItem() => IsViewing && SelectedMenuItem != null;

        private void SaveMenuItem()
        {
            if (string.IsNullOrWhiteSpace(ItemName) || SelectedCategory == null)
            {
                MessageBox.Show("Item name and category are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (IsAdding)
                {
                    var newItem = new MenuItem
                    {
                        ItemName = ItemName,
                        Price = Price,
                        Description = Description, // Save actual description
                        ItemUrl = ItemImagePath,    // Save image path to ItemUrl
                        IsAvailable = IsAvailable,
                        CategoryId = SelectedCategory.CategoryId
                    };
                    _dbContext.MenuItems.Add(newItem);
                    _dbContext.SaveChanges();
                    MessageBox.Show("Menu item added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (IsEditing)
                {
                    var itemToUpdate = _dbContext.MenuItems.Find(SelectedMenuItem.ItemId);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.ItemName = ItemName;
                        itemToUpdate.Price = Price;
                        itemToUpdate.Description = Description; // Save actual description
                        itemToUpdate.ItemUrl = ItemImagePath;   // Save image path to ItemUrl
                        itemToUpdate.IsAvailable = IsAvailable;
                        itemToUpdate.CategoryId = SelectedCategory.CategoryId;
                        _dbContext.SaveChanges();
                        MessageBox.Show("Menu item updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                LoadMenuItems();
                Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving menu item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveMenuItem() => IsAdding || IsEditing;

        private void DeleteMenuItem()
        {
            if (SelectedMenuItem == null)
            {
                MessageBox.Show("Please select a menu item to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedMenuItem.ItemName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var itemToDelete = _dbContext.MenuItems.Find(SelectedMenuItem.ItemId);
                    if (itemToDelete != null)
                    {
                        _dbContext.MenuItems.Remove(itemToDelete);
                        _dbContext.SaveChanges();
                        MessageBox.Show("Menu item deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMenuItems();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting menu item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteMenuItem() => IsViewing && SelectedMenuItem != null;

        private void Cancel()
        {
            IsAdding = false;
            IsEditing = false;
            ItemName = string.Empty;
            Price = 0;
            Description = string.Empty;
            IsAvailable = true;
            SelectedCategory = null;
            ItemImagePath = null;
        }

        private void SearchMenuItem()
        {
            try
            {
                var query = _dbContext.MenuItems.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(i => i.ItemName.Contains(SearchText) || i.Description.Contains(SearchText));
                }
                var items = query.ToList();
                MenuItemList.Clear();
                foreach (var item in items)
                {
                    MenuItemList.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching menu items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterMenuItems()
        {
            try
            {
                var query = _dbContext.MenuItems.AsQueryable();
                if (SelectedCategoryFilter != null)
                {
                    query = query.Where(i => i.CategoryId == SelectedCategoryFilter.CategoryId);
                }
                if (!string.IsNullOrWhiteSpace(PriceFilter) && decimal.TryParse(PriceFilter, out var maxPrice))
                {
                    query = query.Where(i => i.Price <= maxPrice);
                }
                var items = query.ToList();
                MenuItemList.Clear();
                foreach (var item in items)
                {
                    MenuItemList.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering menu items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
            };
            if (dialog.ShowDialog() == true)
            {
                ItemImagePath = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
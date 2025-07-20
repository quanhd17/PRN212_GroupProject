using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FinalProject.Models;
using FinalProject.ViewModels.Helpers;

namespace FinalProject.ViewModels
{
    public class CategoryManagementViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<MenuCategory> _categoryList;
        private MenuCategory _selectedCategory;
        private string _categoryName;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;
        private MenuCategory _selectedParentCategory;
        public ObservableCollection<MenuCategory> ParentCategoryOptions { get; set; } = new ObservableCollection<MenuCategory>();

        public CategoryManagementViewModel()
        {
            _dbContext = new Prn212DbContext();
            CategoryList = new ObservableCollection<MenuCategory>();
            LoadCategoriesCommand = new RelayCommand(_ => LoadCategories());
            AddCategoryCommand = new RelayCommand(_ => AddCategory(), _ => CanAddCategory());
            EditCategoryCommand = new RelayCommand(_ => EditCategory(), _ => CanEditCategory());
            SaveCategoryCommand = new RelayCommand(_ => SaveCategory(), _ => CanSaveCategory());
            DeleteCategoryCommand = new RelayCommand(_ => DeleteCategory(), _ => CanDeleteCategory());
            CancelCommand = new RelayCommand(_ => Cancel());
            SearchCommand = new RelayCommand(_ => SearchCategory());
            LoadCategories();
        }

        public ObservableCollection<MenuCategory> CategoryList
        {
            get => _categoryList;
            set { _categoryList = value; OnPropertyChanged(); }
        }

        public MenuCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                LoadCategoryDetails();
                (EditCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(); }
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
                (EditCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                (EditCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsViewing => !IsEditing && !IsAdding;
        public bool IsEditingOrAdding => IsEditing || IsAdding;

        public MenuCategory SelectedParentCategory
        {
            get => _selectedParentCategory;
            set { _selectedParentCategory = value; OnPropertyChanged(); }
        }
       

        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand SaveCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbContext.MenuCategories.ToList();
                CategoryList.Clear();
                foreach (var cat in categories)
                {
                    CategoryList.Add(cat);
                }
                // Update parent options
                ParentCategoryOptions.Clear();
                ParentCategoryOptions.Add(new MenuCategory { CategoryId = 0, CategoryName = "(No Parent)" });
                foreach (var cat in categories)
                {
                    ParentCategoryOptions.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategoryDetails()
        {
            if (SelectedCategory != null)
            {
                CategoryName = SelectedCategory.CategoryName;
                SelectedParentCategory = SelectedCategory.ParentCategory ?? ParentCategoryOptions.FirstOrDefault();
            }
            else
            {
                SelectedParentCategory = ParentCategoryOptions.FirstOrDefault();
            }
        }

        private void AddCategory()
        {
            IsAdding = true;
            IsEditing = false;
            CategoryName = string.Empty;
            SelectedParentCategory = ParentCategoryOptions.FirstOrDefault();
        }

        private bool CanAddCategory() => IsViewing;

        private void EditCategory()
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("Please select a category to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            IsEditing = true;
            IsAdding = false;
            LoadCategoryDetails();
        }

        private bool CanEditCategory() => IsViewing && SelectedCategory != null;

        private void SaveCategory()
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                MessageBox.Show("Category name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (IsAdding)
                {
                    if (_dbContext.MenuCategories.Any(c => c.CategoryName == CategoryName))
                    {
                        MessageBox.Show("Category name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var newCategory = new MenuCategory
                    {
                        CategoryName = CategoryName,
                        ParentCategoryId = SelectedParentCategory?.CategoryId == 0 ? null : SelectedParentCategory?.CategoryId
                    };
                    _dbContext.MenuCategories.Add(newCategory);
                    _dbContext.SaveChanges();
                    MessageBox.Show("Category added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (IsEditing)
                {
                    var catToUpdate = _dbContext.MenuCategories.Find(SelectedCategory.CategoryId);
                    if (catToUpdate != null)
                    {
                        catToUpdate.CategoryName = CategoryName;
                        catToUpdate.ParentCategoryId = SelectedParentCategory?.CategoryId == 0 ? null : SelectedParentCategory?.CategoryId;
                        _dbContext.SaveChanges();
                        MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                LoadCategories();
                Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveCategory() => IsAdding || IsEditing;

        private void DeleteCategory()
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("Please select a category to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedCategory.CategoryName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var catToDelete = _dbContext.MenuCategories.Find(SelectedCategory.CategoryId);
                    if (catToDelete != null)
                    {
                        _dbContext.MenuCategories.Remove(catToDelete);
                        _dbContext.SaveChanges();
                        MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCategories();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteCategory() => IsViewing && SelectedCategory != null;

        private void Cancel()
        {
            IsAdding = false;
            IsEditing = false;
            CategoryName = string.Empty;
        }

        private void SearchCategory()
        {
            try
            {
                var query = _dbContext.MenuCategories.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(c => c.CategoryName.Contains(SearchText));
                }
                var categories = query.ToList();
                CategoryList.Clear();
                foreach (var cat in categories)
                {
                    CategoryList.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
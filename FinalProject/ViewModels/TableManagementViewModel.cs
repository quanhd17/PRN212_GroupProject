using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FinalProject.Models;
using FinalProject.Models.Enum;
using FinalProject.ViewModels.Helpers;

namespace FinalProject.ViewModels
{
    public class TableManagementViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<Table> _tableList;
        private Table _selectedTable;
        private string _tableName;
        private TableStatus _status;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;
        private TableStatus _statusFilter;

        public TableManagementViewModel()
        {
            _dbContext = new Prn212DbContext();
            TableList = new ObservableCollection<Table>();
            LoadTablesCommand = new RelayCommand(_ => LoadTables());
            AddTableCommand = new RelayCommand(_ => AddTable(), _ => CanAddTable());
            EditTableCommand = new RelayCommand(_ => EditTable(), _ => CanEditTable());
            SaveTableCommand = new RelayCommand(_ => SaveTable(), _ => CanSaveTable());
            DeleteTableCommand = new RelayCommand(_ => DeleteTable(), _ => CanDeleteTable());
            CancelCommand = new RelayCommand(_ => Cancel());
            SearchCommand = new RelayCommand(_ => SearchTable());
            FilterCommand = new RelayCommand(_ => FilterTables());
            LoadTables();
        }

        public ObservableCollection<Table> TableList
        {
            get => _tableList;
            set { _tableList = value; OnPropertyChanged(); }
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
                LoadTableDetails();
                (EditTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string TableName
        {
            get => _tableName;
            set { _tableName = value; OnPropertyChanged(); }
        }

        public TableStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
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
                (EditTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                (EditTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTableCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsViewing => !IsEditing && !IsAdding;
        public bool IsEditingOrAdding => IsEditing || IsAdding;

        public TableStatus StatusFilter
        {
            get => _statusFilter;
            set { _statusFilter = value; OnPropertyChanged(); }
        }

        public Array StatusOptions => Enum.GetValues(typeof(TableStatus));

        // Helper method to get status display value
        public static string GetStatusDisplay(int statusValue)
        {
            return ((TableStatus)statusValue).ToString();
        }

        public ICommand LoadTablesCommand { get; }
        public ICommand AddTableCommand { get; }
        public ICommand EditTableCommand { get; }
        public ICommand SaveTableCommand { get; }
        public ICommand DeleteTableCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterCommand { get; }

        private void LoadTables()
        {
            try
            {
                var tables = _dbContext.Tables.ToList();
                TableList.Clear();
                foreach (var table in tables)
                {
                    TableList.Add(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tables: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTableDetails()
        {
            if (SelectedTable != null)
            {
                TableName = SelectedTable.TableName;
                Status = (TableStatus)SelectedTable.Status;
            }
        }

        private void AddTable()
        {
            IsAdding = true;
            IsEditing = false;
            ClearForm();
        }

        private bool CanAddTable() => IsViewing;

        private void EditTable()
        {
            if (SelectedTable != null)
            {
                IsEditing = true;
                IsAdding = false;
                LoadTableDetails();
            }
        }

        private bool CanEditTable() => IsViewing && SelectedTable != null;

        private void SaveTable()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TableName))
                {
                    MessageBox.Show("Table name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (IsAdding)
                {
                    var newTable = new Table
                    {
                        TableName = TableName.Trim(),
                        Status = (int)Status
                    };

                    _dbContext.Tables.Add(newTable);
                    _dbContext.SaveChanges();
                    TableList.Add(newTable);
                    MessageBox.Show("Table added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (IsEditing && SelectedTable != null)
                {
                    SelectedTable.TableName = TableName.Trim();
                    SelectedTable.Status = (int)Status;
                    _dbContext.SaveChanges();
                    MessageBox.Show("Table updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving table: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveTable() => IsAdding || IsEditing;

        private void DeleteTable()
        {
            if (SelectedTable != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete table '{SelectedTable.TableName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Check if table has any orders
                        var hasOrders = _dbContext.Orders.Any(o => o.TableId == SelectedTable.TableId);
                        if (hasOrders)
                        {
                            MessageBox.Show("Cannot delete table that has associated orders.", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        _dbContext.Tables.Remove(SelectedTable);
                        _dbContext.SaveChanges();
                        TableList.Remove(SelectedTable);
                        MessageBox.Show("Table deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting table: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool CanDeleteTable() => IsViewing && SelectedTable != null;

        private void Cancel()
        {
            IsEditing = false;
            IsAdding = false;
            ClearForm();
        }

        private void ClearForm()
        {
            TableName = string.Empty;
            Status = TableStatus.Available;
        }

        private void SearchTable()
        {
            try
            {
                var query = _dbContext.Tables.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(t => t.TableName.Contains(SearchText));
                }

                var results = query.ToList();
                TableList.Clear();
                foreach (var table in results)
                {
                    TableList.Add(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching tables: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterTables()
        {
            try
            {
                var query = _dbContext.Tables.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(t => t.TableName.Contains(SearchText));
                }

                // Apply status filter if selected
                if (StatusFilter != TableStatus.Available || StatusFilter != TableStatus.Occupied || StatusFilter != TableStatus.Reserved)
                {
                    query = query.Where(t => t.Status == (int)StatusFilter);
                }

                var results = query.ToList();
                TableList.Clear();
                foreach (var table in results)
                {
                    TableList.Add(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering tables: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
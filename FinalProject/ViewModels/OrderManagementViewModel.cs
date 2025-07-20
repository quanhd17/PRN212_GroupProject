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
using FinalProject.Views;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewModels
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<Order> _orderList;
        private Order _selectedOrder;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;
        private OrderStatusEnum? _statusFilter;

        public OrderManagementViewModel()
        {
            _dbContext = new Prn212DbContext();
            OrderList = new ObservableCollection<Order>();
            LoadOrdersCommand = new RelayCommand(_ => LoadOrders());
            AddOrderCommand = new RelayCommand(_ => AddOrder(), _ => CanAddOrder());
            EditOrderCommand = new RelayCommand(_ => EditOrder(), _ => CanEditOrder());
            SaveOrderCommand = new RelayCommand(_ => SaveOrder(), _ => CanSaveOrder());
            DeleteOrderCommand = new RelayCommand(_ => DeleteOrder(), _ => CanDeleteOrder());
            CancelCommand = new RelayCommand(_ => Cancel());
            SearchCommand = new RelayCommand(_ => SearchOrder());
            FilterCommand = new RelayCommand(_ => FilterOrders());
            LoadOrders();
        }

        public ObservableCollection<Order> OrderList
        {
            get => _orderList;
            set { _orderList = value; OnPropertyChanged(); }
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                LoadOrderDetails();
                (EditOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
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
                (EditOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                (EditOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsViewing => !IsEditing && !IsAdding;
        public bool IsEditingOrAdding => IsEditing || IsAdding;

        public OrderStatusEnum? StatusFilter
        {
            get => _statusFilter;
            set { _statusFilter = value; OnPropertyChanged(); }
        }

        public Array StatusOptions => Enum.GetValues(typeof(OrderStatusEnum));

        public ICommand LoadOrdersCommand { get; }
        public ICommand AddOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterCommand { get; }

        private void LoadOrders()
        {
            try
            {
                var orders = _dbContext.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .Include(o => o.Table)
                    .Include(o => o.Account)
                    .Include(o => o.Customer)
                    .ToList();
                OrderList.Clear();
                foreach (var order in orders)
                {
                    OrderList.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderDetails()
        {
            // Optionally load details for editing
        }

        private void AddOrder()
        {
            IsAdding = false;
            IsEditing = false;
            var addOrderWindow = new AddOrderWindow();
            var result = addOrderWindow.ShowDialog();
            if (result == true)
            {
                // Retrieve the ViewModel from the window
                var vm = addOrderWindow.DataContext as AddOrderViewModel;
                if (vm != null && vm.OrderItems.Count > 0)
                {
                    try
                    {
                        int? customerId = null;
                        if (!string.IsNullOrWhiteSpace(vm.CustomerFirstName))
                        {
                            var newCustomer = new Customer { FirstName = vm.CustomerFirstName };
                            _dbContext.Customers.Add(newCustomer);
                            _dbContext.SaveChanges();
                            customerId = newCustomer.CustomerId;
                        }
                        // Create new Order
                        var newOrder = new Order
                        {
                            OrderDate = DateTime.Now,
                            TotalAmount = vm.TotalAmount,
                            Discount = vm.Discount,
                            FinalAmount = vm.FinalAmount,
                            Status = Models.Enum.OrderStatusEnum.Pending,
                            TableId = vm.SelectedTable?.TableId ?? 0,
                            AccountId = vm.SelectedStaff?.AccountId ?? 0,
                            CustomerId = customerId
                        };
                        _dbContext.Orders.Add(newOrder);
                        _dbContext.SaveChanges();
                        // Add OrderItems
                        foreach (var oi in vm.OrderItems)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = newOrder.OrderId,
                                ItemId = oi.ItemId,
                                Quantity = oi.Quantity,
                                ItemPrice = oi.ItemPrice
                            };
                            _dbContext.OrderItems.Add(orderItem);
                        }
                        _dbContext.SaveChanges();
                        MessageBox.Show("Order added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool CanAddOrder() => IsViewing;

        private void EditOrder()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Please select an order to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var editOrderWindow = new Views.EditOrderWindow(SelectedOrder);
            var result = editOrderWindow.ShowDialog();
            if (result == true)
            {
                var vm = editOrderWindow.DataContext as EditOrderViewModel;
                if (vm != null && vm.OrderItems.Count > 0)
                {
                    try
                    {
                        // Update order fields
                        SelectedOrder.TableId = vm.SelectedTable?.TableId ?? SelectedOrder.TableId;
                        SelectedOrder.AccountId = vm.SelectedStaff?.AccountId ?? SelectedOrder.AccountId;
                        SelectedOrder.Discount = vm.Discount;
                        SelectedOrder.TotalAmount = vm.TotalAmount;
                        SelectedOrder.FinalAmount = vm.FinalAmount;
                        // Update order items: only add new items (do not remove existing)
                        foreach (var oi in vm.OrderItems)
                        {
                            var exists = _dbContext.OrderItems.FirstOrDefault(x => x.OrderId == SelectedOrder.OrderId && x.ItemId == oi.ItemId);
                            if (exists == null)
                            {
                                var newOrderItem = new OrderItem
                                {
                                    OrderId = SelectedOrder.OrderId,
                                    ItemId = oi.ItemId,
                                    Quantity = oi.Quantity,
                                    ItemPrice = oi.ItemPrice
                                };
                                _dbContext.OrderItems.Add(newOrderItem);
                            }
                            else
                            {
                                exists.Quantity = oi.Quantity; // update quantity if changed
                                exists.ItemPrice = oi.ItemPrice;
                            }
                        }
                        _dbContext.SaveChanges();
                        MessageBox.Show("Order updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool CanEditOrder() => IsViewing && SelectedOrder != null;

        private void SaveOrder()
        {
            try
            {
                if (IsAdding)
                {
                    // Add new order logic
                    // _dbContext.Orders.Add(newOrder);
                }
                else if (IsEditing)
                {
                    // Update order logic
                }
                _dbContext.SaveChanges();
                LoadOrders();
                IsAdding = false;
                IsEditing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveOrder() => IsEditingOrAdding;

        private void DeleteOrder()
        {
            if (SelectedOrder == null)
            {
                MessageBox.Show("Please select an order to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to delete order #{SelectedOrder.OrderId}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var orderToDelete = _dbContext.Orders.Find(SelectedOrder.OrderId);
                    if (orderToDelete != null)
                    {
                        _dbContext.Orders.Remove(orderToDelete);
                        _dbContext.SaveChanges();
                        MessageBox.Show("Order deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteOrder() => IsViewing && SelectedOrder != null;

        private void Cancel()
        {
            IsAdding = false;
            IsEditing = false;
        }

        private void SearchOrder()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadOrders();
                return;
            }
            var filtered = _dbContext.Orders.Where(o => o.OrderId.ToString().Contains(SearchText) || (o.Customer != null && (o.Customer.FirstName + " " + o.Customer.LastName).Contains(SearchText))).ToList();
            OrderList.Clear();
            foreach (var order in filtered)
            {
                OrderList.Add(order);
            }
        }

        private void FilterOrders()
        {
            if (StatusFilter == null)
            {
                LoadOrders();
                return;
            }
            var filtered = _dbContext.Orders.Where(o => o.Status == StatusFilter).ToList();
            OrderList.Clear();
            foreach (var order in filtered)
            {
                OrderList.Add(order);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
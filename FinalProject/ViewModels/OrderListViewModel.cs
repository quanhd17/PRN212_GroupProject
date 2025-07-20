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
    public class OrderListViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<Order> _orderList;
        private Order _selectedOrder;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private OrderStatusEnum _statusFilter;
        private string _searchText;
        private bool _showAllStatuses;

        public OrderListViewModel()
        {
            _dbContext = new Prn212DbContext();
            OrderList = new ObservableCollection<Order>();
            StartDate = DateTime.Today.AddDays(-7); // Default to last 7 days
            EndDate = DateTime.Today;
            StatusFilter = OrderStatusEnum.Pending;
            ShowAllStatuses = true;
            
            LoadOrdersCommand = new RelayCommand(_ => LoadOrders());
            FilterOrdersCommand = new RelayCommand(_ => FilterOrders());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ViewOrderDetailsCommand = new RelayCommand(_ => ViewOrderDetails(), _ => CanViewOrderDetails());
            EditOrderCommand = new RelayCommand(_ => EditOrder(), _ => CanEditOrder());
            CreateNewOrderCommand = new RelayCommand(_ => CreateNewOrder());
            
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
                (ViewOrderDetailsCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        public OrderStatusEnum StatusFilter
        {
            get => _statusFilter;
            set { _statusFilter = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public bool ShowAllStatuses
        {
            get => _showAllStatuses;
            set { _showAllStatuses = value; OnPropertyChanged(); }
        }

        public Array StatusOptions => Enum.GetValues(typeof(OrderStatusEnum));

        public ICommand LoadOrdersCommand { get; }
        public ICommand FilterOrdersCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand CreateNewOrderCommand { get; }

        private void LoadOrders()
        {
            try
            {
                var query = _dbContext.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.Account)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                    .AsQueryable();

                // Apply date filter
                if (StartDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= StartDate.Value);
                }
                if (EndDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= EndDate.Value.AddDays(1).AddSeconds(-1));
                }

                // Apply status filter
                if (!ShowAllStatuses)
                {
                    query = query.Where(o => o.Status == StatusFilter);
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchTerm = SearchText.ToLower();
                    query = query.Where(o => 
                        o.OrderId.ToString().Contains(searchTerm) ||
                        (o.Customer != null && (o.Customer.FirstName + " " + o.Customer.LastName).ToLower().Contains(searchTerm)) ||
                        o.Table.TableName.ToLower().Contains(searchTerm) ||
                        o.Account.FullName.ToLower().Contains(searchTerm)
                    );
                }

                var orders = query.OrderByDescending(o => o.OrderDate).ToList();
                
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

        private void FilterOrders()
        {
            LoadOrders();
        }

        private void ClearFilters()
        {
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;
            StatusFilter = OrderStatusEnum.Pending;
            ShowAllStatuses = true;
            SearchText = string.Empty;
            LoadOrders();
        }

        private void ViewOrderDetails()
        {
            if (SelectedOrder != null)
            {
                // Show order details in a new window
                var detailsWindow = new OrderDetailsWindow(SelectedOrder);
                detailsWindow.Show();
            }
        }

        private bool CanViewOrderDetails() => SelectedOrder != null;

        private void EditOrder()
        {
            if (SelectedOrder != null)
            {
                // Navigate to edit order page
                // This will be handled by the view
                var editWindow = new OrderEditWindow(SelectedOrder);
                editWindow.Show();
            }
        }

        private bool CanEditOrder() => SelectedOrder != null;

        private void CreateNewOrder()
        {
            // Open new order window
            var newOrderWindow = new OrderEditWindow();
            newOrderWindow.Show();
        }

        public static string GetStatusDisplay(OrderStatusEnum status)
        {
            return status.ToString();
        }

        public static string GetCustomerName(Order order)
        {
            if (order.Customer != null)
            {
                return $"{order.Customer.FirstName} {order.Customer.LastName}";
            }
            return "Walk-in Customer";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
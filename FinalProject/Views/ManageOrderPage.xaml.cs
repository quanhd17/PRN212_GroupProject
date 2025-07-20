using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Views
{
    public partial class ManageOrderPage : Page
    {
        public ManageOrderPage()
        {
            InitializeComponent();
        }

        private void ViewDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is Models.Order order)
            {
                var detailsWindow = new OrderDetailsWindow(order);
                detailsWindow.ShowDialog();
            }
        }

        private void DeleteOrder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is Models.Order order)
            {
                var result = MessageBox.Show($"Are you sure you want to delete order #{order.OrderId}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var db = new Models.Prn212DbContext())
                        {
                            var orderToDelete = db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.OrderId == order.OrderId);
                            if (orderToDelete != null)
                            {
                                db.OrderItems.RemoveRange(orderToDelete.OrderItems);
                                db.Orders.Remove(orderToDelete);
                                db.SaveChanges();
                            }
                        }
                        MessageBox.Show("Order deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Refresh the order list
                        if (DataContext is ViewModels.OrderManagementViewModel vm)
                        {
                            vm.LoadOrdersCommand.Execute(null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CheckoutOrder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is Models.Order order)
            {
                var checkoutWindow = new CheckoutWindow(order);
                var result = checkoutWindow.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        using (var db = new Models.Prn212DbContext())
                        {
                            var orderToCheckout = db.Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
                            if (orderToCheckout != null)
                            {
                                orderToCheckout.Status = Models.Enum.OrderStatusEnum.Completed;
                                db.SaveChanges();
                            }
                        }
                        MessageBox.Show("Order checked out successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Refresh the order list
                        if (DataContext is ViewModels.OrderManagementViewModel vm)
                        {
                            vm.LoadOrdersCommand.Execute(null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error checking out order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.OrderManagementViewModel vm)
            {
                vm.LoadOrdersCommand.Execute(null);
            }
        }
    }
} 
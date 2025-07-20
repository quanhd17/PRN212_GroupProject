using System;
using System.Windows;
using FinalProject.Models;

namespace FinalProject.Views
{
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            LoadOrderDetails(order);
        }

        private void LoadOrderDetails(Order order)
        {
            try
            {
                // Set order ID
                OrderIdText.Text = $"Order #{order.OrderId}";

                // Set customer information
                if (order.Customer != null)
                {
                    CustomerNameText.Text = $"Name: {order.Customer.FirstName} {order.Customer.LastName}";
                    CustomerEmailText.Text = $"Email: {order.Customer.Email}";
                    CustomerPhoneText.Text = $"Phone: {order.Customer.Phone}";
                }
                else
                {
                    CustomerNameText.Text = "Name: Walk-in Customer";
                    CustomerEmailText.Text = "Email: N/A";
                    CustomerPhoneText.Text = "Phone: N/A";
                }

                // Set order information
                OrderDateText.Text = $"Date: {order.OrderDate:MM/dd/yyyy HH:mm}";
                TableText.Text = $"Table: {order.Table.TableName}";
                StatusText.Text = $"Status: {order.Status}";

                // Set staff information
                StaffNameText.Text = $"Staff: {order.Account.FullName}";

                // Set order items
                OrderItemsDataGrid.ItemsSource = order.OrderItems;

                // Set order summary
                SubtotalText.Text = order.TotalAmount?.ToString("C") ?? "$0.00";
                DiscountText.Text = order.Discount?.ToString("C") ?? "$0.00";
                FinalAmountText.Text = order.FinalAmount?.ToString("C") ?? "$0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 
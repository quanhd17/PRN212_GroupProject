using System.Windows;
using FinalProject.Models;
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    public partial class CheckoutWindow : Window
    {
        public CheckoutWindow(Order order)
        {
            InitializeComponent();
            DataContext = new CheckoutViewModel(order);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject.Views
{
    /// <summary>
    /// Interaction logic for StaffPage.xaml
    /// </summary>
    public partial class StaffPage : Page
    {
        public StaffPage()
        {
            InitializeComponent();
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Orders management feature will be implemented here.", 
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageTables_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Table management feature will be implemented here.", 
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu management feature will be implemented here.", 
                "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageCategories_Click(object sender, RoutedEventArgs e)
        {
            StaffFrame.Navigate(new ManageCategoryPage());
        }

        private void ManageMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StaffFrame.Navigate(new ManageMenuPage());
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            StaffFrame.Navigate(new OrderListPage());
        }

        private void Tables_Click(object sender, RoutedEventArgs e)
        {
            StaffFrame.Navigate(new ManageTablePage());
        }
    }
}

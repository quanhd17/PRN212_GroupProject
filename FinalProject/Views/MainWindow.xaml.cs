using FinalProject.Models;
using FinalProject.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Account _currentUser;      

        public MainWindow(Account user)
        {
            InitializeComponent();
            _currentUser = user;
            if (_currentUser == null)
            {
                MainFrame.Navigate(new Views.Login());
                lblWelcome.Text = "Welcome to Restaurant Management System";
            }
            else
            {
                LoadUIBasedOnRole();
            }
        }

        private void LoadUIBasedOnRole()
        {
            lblWelcome.Text = $"Welcome, {_currentUser.FullName} ({(_currentUser.Role == 0 ? "Admin" : "Staff")})";

            if (_currentUser.Role == 0) // Admin
            {
                NavigateToAdminPage();
            }
            else if (_currentUser.Role == 1) // Staff
            {
                NavigateToStaffPage();
            }
        }

        private void NavigateToAdminPage()
        {
            var adminPage = new AdminPage();
            MainFrame.Navigate(adminPage);
        }

        public void NavigateToStaffPage()
        {
            var staffPage = new StaffPage();
            MainFrame.Navigate(staffPage);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new Views.Login();
                loginWindow.Show();
                this.Close();
            }
        }

        public void SetUserAndNavigate(Account user)
        {
            _currentUser = user;
            LoadUIBasedOnRole();
        }
    }
}
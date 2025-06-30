using FinalProject.Models;
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
        private readonly Account _currentUser;
        public MainWindow(Account user)
        {
            InitializeComponent();
            _currentUser = user;

            LoadUIBasedOnRole();
        }

        private void LoadUIBasedOnRole()
        {
            if (_currentUser.Role == 0) // Admin
            {
                adminPanel.Visibility = Visibility.Visible;
                staffPanel.Visibility = Visibility.Collapsed;
            }
            else if (_currentUser.Role == 1) // Staff
            {
                adminPanel.Visibility = Visibility.Collapsed;
                staffPanel.Visibility = Visibility.Visible;
            }


            lblWelcome.Text = $"Welcome, {_currentUser.FullName}";
        }
    }
}
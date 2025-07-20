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
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private AdminPageViewModel _viewModel;

        public AdminPage()
        {
            InitializeComponent();
            _viewModel = new AdminPageViewModel();
            DataContext = _viewModel;
            
            // Set up password binding
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            
            // Load staff data
            _viewModel?.StaffManagementViewModel?.LoadStaffCommand?.Execute(null);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.StaffManagementViewModel != null)
            {
                _viewModel.StaffManagementViewModel.Password = PasswordBox.Password;
            }
        }

        private void ClearPasswordBox()
        {
            PasswordBox.Password = "";
        }
    }
}

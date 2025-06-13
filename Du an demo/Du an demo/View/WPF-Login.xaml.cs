using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Du_an_demo.ViewModel;
using MaterialDesignThemes.Wpf; // Đảm bảo bạn đã cài MaterialDesignThemes.Wpf

namespace Du_an_demo.View
{
    /// <summary>
    /// Interaction logic for WPF_Login.xaml
    /// </summary>
    public partial class WPF_Login : Window
    {
        public WPF_Login()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ====== Hiện / Ẩn mật khẩu ======

        private void btnTogglePassword_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible.Text = FloatingPasswordBox.Password;
            FloatingPasswordBox.Visibility = Visibility.Collapsed;
            tbPasswordVisible.Visibility = Visibility.Visible;

            eyeIcon.Kind = PackIconKind.Eye;
        }

        private void btnTogglePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            FloatingPasswordBox.Password = tbPasswordVisible.Text;
            tbPasswordVisible.Visibility = Visibility.Collapsed;
            FloatingPasswordBox.Visibility = Visibility.Visible;

            eyeIcon.Kind = PackIconKind.EyeOff;
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbPasswordVisible.Visibility == Visibility.Visible)
            {
                tbPasswordVisible.Text = FloatingPasswordBox.Password;
            }
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var service = new AccountService();
            var user = service.Login(txbUserName.Text.Trim(), FloatingPasswordBox.Password.Trim());

            if (user != null)
            {
                MessageBox.Show($"Welcome {user.Username}, Role: {user.Role}");

                if (user.Role == "Admin")
                {
                    // Mở Dashboard Admin
                    var dashboard = new DashBoard();
                    dashboard.Show();
                    this.Close();
                }
                else if (user.Role == "Staff")
                {
                    // Mở Dashboard Staff
                    var dashboard = new DashBoard();
                    dashboard.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Login failed! Please check credentials.");
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new WPF_Register(); 
            signUpWindow.Show();
            this.Close(); 


        }
    }
}

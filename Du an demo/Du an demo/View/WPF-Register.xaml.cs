using Du_an_demo.Model;
using Du_an_demo.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Du_an_demo.View
{
    /// <summary>
    /// Interaction logic for WPF_Register.xaml
    /// </summary>
    public partial class WPF_Register : Window
    {
        private AccountService accountService;
        public WPF_Register()
        {
            InitializeComponent();
            accountService = new AccountService();
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
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            // Mở form đăng nhập
            WPF_Login loginWindow = new WPF_Login();
            loginWindow.Show();

            // Đóng form đăng ký hiện tại
            this.Close();
        }

        private void btnTogglePassword_Checked(object sender, RoutedEventArgs e)
        {
            tbPasswordVisible.Text = txbPasswordBoxRegister.Password;
            txbPasswordBoxRegister.Visibility = Visibility.Collapsed;
            tbPasswordVisible.Visibility = Visibility.Visible;

            eyeIcon.Kind = PackIconKind.Eye;
        }

        private void btnTogglePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txbPasswordBoxRegister.Password = tbPasswordVisible.Text;
            tbPasswordVisible.Visibility = Visibility.Collapsed;
            txbPasswordBoxRegister.Visibility = Visibility.Visible;

            eyeIcon.Kind = PackIconKind.EyeOff;
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbPasswordVisible.Visibility == Visibility.Visible)
            {
                tbPasswordVisible.Text = txbPasswordBoxRegister.Password;
            }
        }
        private void btnToggleConfirmPassword_Checked(object sender, RoutedEventArgs e)
        {
            txbConfirmPasswordRegister.Visibility = Visibility.Collapsed;
            txbConfirmPasswordVisible.Visibility = Visibility.Visible;
            eyeIconConfirm.Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye;
        }

        private void btnToggleConfirmPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txbConfirmPasswordRegister.Visibility = Visibility.Visible;
            txbConfirmPasswordVisible.Visibility = Visibility.Collapsed;
            eyeIconConfirm.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
        }

        private void txbConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txbConfirmPasswordVisible.Text = txbConfirmPasswordRegister.Password;
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txbUserNameRegister.Text.Trim();
                string password = txbPasswordBoxRegister.Password.Trim();
                string confirmPassword = txbConfirmPasswordRegister.Password.Trim();

                // Kiểm tra nếu các trường nhập liệu không rỗng
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }

                // Kiểm tra mật khẩu có đủ độ dài và bao gồm ký tự đặc biệt và chữ hoa
                if (password.Length < 6 || !password.Any(char.IsUpper) || !password.Any(ch => !Char.IsLetterOrDigit(ch)))
                {
                    MessageBox.Show("Password must be at least 6 characters long, include an uppercase letter and a special character.");
                    return;
                }

                // Kiểm tra trùng tên đăng nhập
                bool success = accountService.Register(new Account { Username = username, Password = password });

                if (success)
                {
                    MessageBox.Show("Account successfully registered! You can now log in.");
                    var loginWindow = new WPF_Login();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username already exists.");
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi và hiển thị thông báo lỗi chi tiết
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



    }

}


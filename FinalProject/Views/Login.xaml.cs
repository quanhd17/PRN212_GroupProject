using System.Windows;
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    public partial class Login : Window
    {
        private readonly LoginViewModel _viewModel;

        public Login()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Username = UsernameBox.Text;
            _viewModel.Password = PasswordBox.Password;
            if (_viewModel.LoginCommand.CanExecute(null))
            {
                _viewModel.LoginCommand.Execute(this); // Pass this window for closing
            }
        }
    }
} 
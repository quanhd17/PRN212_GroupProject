using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using FinalProject.Models;
    using Microsoft.EntityFrameworkCore;
    using FinalProject.ViewModels.Helpers;

    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            using (var db = new Prn212DbContext())
            {
                var user = db.Accounts
                             .FirstOrDefault(a => a.Username == Username && a.Password == Password);

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.");
                    return;
                }

                if (user.IsBanned == true)
                {
                    MessageBox.Show("Account has been locked.");
                    return;
                }

                switch (user.Role)
                {
                    case 0:
                        MessageBox.Show("Hello Admin: " + user.FullName);
                        break;
                    case 1:
                        MessageBox.Show("Hello Staff: " + user.FullName);
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FinalProject.Models;
using FinalProject.Models.Enum;
using FinalProject.ViewModels.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.ViewModels
{
    public class StaffManagementViewModel : INotifyPropertyChanged
    {
        private readonly Prn212DbContext _dbContext;
        private ObservableCollection<Account> _staffList;
        private Account _selectedStaff;
        private string _searchText;
        private bool _isEditing;
        private bool _isAdding;

        // Properties for new/edit staff
        private string _username;
        private string _password;
        private string _fullName;
        private AccountRole _selectedRole;
        private bool _isBanned;

        public StaffManagementViewModel()
        {
            _dbContext = new Prn212DbContext();
            StaffList = new ObservableCollection<Account>();
            
            // Initialize commands
            LoadStaffCommand = new RelayCommand(_ => LoadStaff());
            AddStaffCommand = new RelayCommand(_ => AddStaff(), _ => CanAddStaff());
            EditStaffCommand = new RelayCommand(_ => EditStaff(), _ => CanEditStaff());
            SaveStaffCommand = new RelayCommand(_ => SaveStaff(), _ => CanSaveStaff());
            DeleteStaffCommand = new RelayCommand(_ => DeleteStaff(), _ => CanDeleteStaff());
            CancelCommand = new RelayCommand(_ => Cancel());
            SearchCommand = new RelayCommand(_ => SearchStaff());

            // Load initial data
            LoadStaff();
        }

        #region Properties

        public ObservableCollection<Account> StaffList
        {
            get => _staffList;
            set
            {
                _staffList = value;
                OnPropertyChanged();
            }
        }

        public Account SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged();
                LoadStaffDetails();
                (EditStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsViewing));
                OnPropertyChanged(nameof(IsEditingOrAdding));
                (EditStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                _isAdding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsViewing));
                OnPropertyChanged(nameof(IsEditingOrAdding));
                (EditStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsViewing => !IsEditing && !IsAdding;

        public bool IsEditingOrAdding => IsAdding || IsEditing;

        // Staff details properties
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public AccountRole SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }

        public bool IsBanned
        {
            get => _isBanned;
            set
            {
                _isBanned = value;
                OnPropertyChanged();
            }
        }

        public Array RoleOptions => Enum.GetValues(typeof(AccountRole));

        #endregion

        #region Commands

        public ICommand LoadStaffCommand { get; }
        public ICommand AddStaffCommand { get; }
        public ICommand EditStaffCommand { get; }
        public ICommand SaveStaffCommand { get; }
        public ICommand DeleteStaffCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }

        #endregion

        #region Methods

        private void LoadStaff()
        {
            try
            {
                var staff = _dbContext.Accounts
                    .Where(a => a.Role == (int)AccountRole.Staff)
                    .ToList();

                StaffList.Clear();
                foreach (var account in staff)
                {
                    StaffList.Add(account);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStaffDetails()
        {
            if (SelectedStaff != null)
            {
                Username = SelectedStaff.Username;
                Password = ""; // Don't show password
                FullName = SelectedStaff.FullName;
                SelectedRole = (AccountRole)SelectedStaff.Role;
                IsBanned = SelectedStaff.IsBanned ?? false;
            }
        }

        private void AddStaff()
        {
            IsAdding = true;
            IsEditing = false;
            ClearStaffDetails();
        }

        private bool CanAddStaff()
        {
            return IsViewing;
        }

        private void EditStaff()
        {
            if (SelectedStaff == null)
            {
                MessageBox.Show("Please select a staff member to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            IsEditing = true;
            IsAdding = false;
            LoadStaffDetails();
        }

        private bool CanEditStaff()
        {
            return IsViewing && SelectedStaff != null;
        }

        private void SaveStaff()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName))
            {
                MessageBox.Show("Username and Full Name are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsAdding)
                {
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        MessageBox.Show("Password is required for new staff.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Check if username already exists
                    if (_dbContext.Accounts.Any(a => a.Username == Username))
                    {
                        MessageBox.Show("Username already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newStaff = new Account
                    {
                        Username = Username,
                        Password = Password, // In production, this should be hashed
                        FullName = FullName,
                        Role = (int)SelectedRole,
                        IsBanned = IsBanned
                    };

                    _dbContext.Accounts.Add(newStaff);
                    _dbContext.SaveChanges();

                    MessageBox.Show("Staff member added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (IsEditing)
                {
                    var staffToUpdate = _dbContext.Accounts.Find(SelectedStaff.AccountId);
                    if (staffToUpdate != null)
                    {
                        staffToUpdate.Username = Username;
                        staffToUpdate.FullName = FullName;
                        staffToUpdate.Role = (int)SelectedRole;
                        staffToUpdate.IsBanned = IsBanned;

                        // Only update password if provided
                        if (!string.IsNullOrWhiteSpace(Password))
                        {
                            staffToUpdate.Password = Password; // In production, this should be hashed
                        }

                        _dbContext.SaveChanges();

                        MessageBox.Show("Staff member updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LoadStaff();
                Cancel();
                OnPropertyChanged(nameof(Password)); // Notify UI to clear password box
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveStaff()
        {
            return IsAdding || IsEditing;
        }

        private void DeleteStaff()
        {
            if (SelectedStaff == null)
            {
                MessageBox.Show("Please select a staff member to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedStaff.FullName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var staffToDelete = _dbContext.Accounts.Find(SelectedStaff.AccountId);
                    if (staffToDelete != null)
                    {
                        _dbContext.Accounts.Remove(staffToDelete);
                        _dbContext.SaveChanges();

                        MessageBox.Show("Staff member deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStaff();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteStaff()
        {
            return IsViewing && SelectedStaff != null;
        }

        private void Cancel()
        {
            IsAdding = false;
            IsEditing = false;
            ClearStaffDetails();
            OnPropertyChanged(nameof(Password)); // Notify UI to clear password box
        }

        private void ClearStaffDetails()
        {
            Username = "";
            Password = "";
            FullName = "";
            SelectedRole = AccountRole.Staff;
            IsBanned = false;
        }

        public void ClearPassword()
        {
            Password = "";
        }

        private void SearchStaff()
        {
            try
            {
                var query = _dbContext.Accounts.Where(a => a.Role == (int)AccountRole.Staff);

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(a => a.Username.Contains(SearchText) || a.FullName.Contains(SearchText));
                }

                var staff = query.ToList();

                StaffList.Clear();
                foreach (var account in staff)
                {
                    StaffList.Add(account);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
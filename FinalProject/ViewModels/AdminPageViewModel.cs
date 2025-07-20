using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinalProject.ViewModels;

namespace FinalProject.ViewModels
{
    public class AdminPageViewModel : INotifyPropertyChanged
    {
        private StaffManagementViewModel _staffManagementViewModel;

        public AdminPageViewModel()
        {
            StaffManagementViewModel = new StaffManagementViewModel();
        }

        public StaffManagementViewModel StaffManagementViewModel
        {
            get => _staffManagementViewModel;
            set
            {
                _staffManagementViewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
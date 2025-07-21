using ClosedXML.Excel;
using FinalProject.Models;
using FinalProject.Models.Enum;
using FinalProject.ViewModels;
using FinalProject.ViewModels.Helpers;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FinalProject.ViewModels
{
    public class AdminPageViewModel : INotifyPropertyChanged
    {
        private StaffManagementViewModel _staffManagementViewModel;
        public ICommand ExportExcelCommand { get; }
        public AdminPageViewModel()
        {
            StaffManagementViewModel = new StaffManagementViewModel();
            ExportExcelCommand = new RelayCommand(_ => ExportAllData());

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
        private void ExportAllData()
        {
            try
            {
                using var workbook = new XLWorkbook();

                using var context = new Prn212DbContext();

                // 1. Export Tables
                var tables = context.Tables.ToList();
                var tableSheet = workbook.Worksheets.Add("Tables");
                tableSheet.Cell(1, 1).Value = "Table ID";
                tableSheet.Cell(1, 2).Value = "Table Name";
                tableSheet.Cell(1, 3).Value = "Status";
                for (int i = 0; i < tables.Count; i++)
                {
                    tableSheet.Cell(i + 2, 1).Value = tables[i].TableId;
                    tableSheet.Cell(i + 2, 2).Value = tables[i].TableName;
                    tableSheet.Cell(i + 2, 3).Value = ((TableStatus)tables[i].Status).ToString();
                }

                // 2. Export Menu
                var menuItems = context.MenuItems.ToList(); // assuming your menu model is MenuItem
                var menuSheet = workbook.Worksheets.Add("Menu");
                menuSheet.Cell(1, 1).Value = "Item ID";
                menuSheet.Cell(1, 2).Value = "Item Name";
                menuSheet.Cell(1, 3).Value = "Price";
                for (int i = 0; i < menuItems.Count; i++)
                {
                    menuSheet.Cell(i + 2, 1).Value = menuItems[i].ItemId;
                    menuSheet.Cell(i + 2, 2).Value = menuItems[i].ItemName;
                    menuSheet.Cell(i + 2, 3).Value = menuItems[i].Price;
                }

                // 3. Export Staff
                var staffs = context.Accounts
                    .Where(a => a.Role == 1)
                    .ToList();

                var staffSheet = workbook.Worksheets.Add("Staff");
                staffSheet.Cell(1, 1).Value = "Account ID";
                staffSheet.Cell(1, 2).Value = "Username";
                staffSheet.Cell(1, 3).Value = "Full Name";
                staffSheet.Cell(1, 4).Value = "Role";
                staffSheet.Cell(1, 5).Value = "Is Banned";

                for (int i = 0; i < staffs.Count; i++)
                {
                    staffSheet.Cell(i + 2, 1).Value = staffs[i].AccountId;
                    staffSheet.Cell(i + 2, 2).Value = staffs[i].Username;
                    staffSheet.Cell(i + 2, 3).Value = staffs[i].FullName;
                    staffSheet.Cell(i + 2, 4).Value = staffs[i].Role;
                    staffSheet.Cell(i + 2, 5).Value = staffs[i].IsBanned == true ? "Yes" : "No";

                }


                // 4. Export Orders
                var orders = context.Orders.ToList();
                var orderSheet = workbook.Worksheets.Add("Orders");
                orderSheet.Cell(1, 1).Value = "Order ID";
                orderSheet.Cell(1, 2).Value = "Table ID";
                orderSheet.Cell(1, 3).Value = "Order Date";
                orderSheet.Cell(1, 4).Value = "Total Amount";
                for (int i = 0; i < orders.Count; i++)
                {
                    orderSheet.Cell(i + 2, 1).Value = orders[i].OrderId;
                    orderSheet.Cell(i + 2, 2).Value = orders[i].TableId;
                    orderSheet.Cell(i + 2, 3).Value = orders[i].OrderDate;
                    orderSheet.Cell(i + 2, 4).Value = orders[i].TotalAmount;
                }

                // Save file
                var dialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "RestaurantData.xlsx"
                };

                if (dialog.ShowDialog() == true)
                {
                    workbook.SaveAs(dialog.FileName);
                    MessageBox.Show("Export successful!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 
using System.Windows.Controls;
using System.Windows;
using FinalProject.ViewModels;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Linq;

namespace FinalProject.Views
{
    public partial class StaffStatisticsPage : Page
    {
        public StaffStatisticsPage()
        {
            InitializeComponent();
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is StaffStatisticsViewModel vm && vm.DailyStats != null && vm.DailyStats.Any())
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"DailyStats_{System.DateTime.Now:yyyyMMdd}.xlsx"
                };
                if (dialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("Daily Stats");
                        ws.Cell(1, 1).Value = "Date";
                        ws.Cell(1, 2).Value = "Orders";
                        ws.Cell(1, 3).Value = "Revenue";
                        ws.Cell(1, 4).Value = "Most Popular Item";
                        int row = 2;
                        foreach (var stat in vm.DailyStats)
                        {
                            ws.Cell(row, 1).Value = stat.Date.ToShortDateString();
                            ws.Cell(row, 2).Value = stat.Orders;
                            ws.Cell(row, 3).Value = stat.Revenue;
                            ws.Cell(row, 4).Value = stat.MostPopularItem;
                            row++;
                        }
                        ws.Columns().AdjustToContents();
                        workbook.SaveAs(dialog.FileName);
                    }
                    MessageBox.Show("Exported to Excel successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
} 
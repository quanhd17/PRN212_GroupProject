using System.Windows.Controls;
using System.Windows.Input;
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    public partial class ManageTablePage : Page
    {
        public ManageTablePage()
        {
            InitializeComponent();
            DataContext = new TableManagementViewModel();
        }

        private void TableCard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Models.Table table)
            {
                if (this.DataContext is TableManagementViewModel vm)
                {
                    vm.SelectedTable = table;
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle list box selection changes if needed
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle status filter selection changes if needed
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle DataGrid selection changes if needed
        }
    }
} 
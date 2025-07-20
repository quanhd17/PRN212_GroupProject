using System.Windows.Controls;
using System.Windows.Input;
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    public partial class ManageMenuPage : Page
    {
        public ManageMenuPage()
        {
            InitializeComponent();
        }

		private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
		{

		}

        private void MenuItemCard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Models.MenuItem menuItem)
            {
                if (this.DataContext is MenuManagementViewModel vm)
                {
                    vm.SelectedMenuItem = menuItem;
                }
            }
        }

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
		{

		}
	}
} 
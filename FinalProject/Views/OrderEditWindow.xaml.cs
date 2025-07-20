using System.Windows;
using System.Windows.Controls;
using FinalProject.Models;
using FinalProject.ViewModels;

namespace FinalProject.Views
{
    public partial class OrderEditWindow : Window
    {
        public OrderEditWindow(Order orderToEdit = null)
        {
            InitializeComponent();
            DataContext = new OrderEditViewModel(orderToEdit);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is OrderEditViewModel viewModel && e.NewValue is MenuCategory selectedCategory)
            {
                viewModel.OnCategorySelected(selectedCategory);
            }
        }
    }
} 
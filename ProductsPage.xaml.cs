using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace StockManagement_WinUI3
{
    public sealed partial class ProductsPage : Page
    {
        public ProductsViewModel ViewModel { get; set; }

        public ProductsPage()
        {
            InitializeComponent();
            ViewModel = new ProductsViewModel();
            _ = ProductsViewModel.LoadProducts(ViewModel);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text;
            var status = StatusComboBox.SelectedIndex == 1 ? true : false;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name is required");
                return;
            }

            _ = ProductsViewModel.AddProduct(ViewModel, name, status);

            NameTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is not ProductsItem productItem) return;
            _ = ProductsViewModel.DeleteProduct(ViewModel, int.Parse(productItem.Code));
            Console.WriteLine($"Deleted product: {productItem.Name}");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ProductsViewModel.LoadProducts(ViewModel);
        }
    }
}
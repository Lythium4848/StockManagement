using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace StockManagement_WinUI3
{
    public sealed partial class StockPage : Page
    {
        private StockViewModel ViewModel { get; set; }

        public StockPage()
        {
            InitializeComponent();
            ViewModel = new StockViewModel();
            _ = StockViewModel.LoadStock(ViewModel);
            AppViewModel.UserLoggedIn += OnUserLoggedIn;
        }

        private void OnUserLoggedIn(UserReturn user)
        {
            _ = StockViewModel.LoadStock(ViewModel);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text;
            var transactionDate = DateTime.Parse(TransactionDateInput.Date.ToString());
            var quantity = int.Parse(QuantityInput.Text);
            var status = StatusComboBox.SelectedIndex == 1;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name is required");
                return;
            }

            _ = StockViewModel.AddStock(ViewModel, name, transactionDate, quantity, status);

            NameTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is not StockItem stockItem) return;
            _ = StockViewModel.DeleteStock(ViewModel, stockItem.Id);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            _ = StockViewModel.LoadStock(ViewModel);
        }
    }
}
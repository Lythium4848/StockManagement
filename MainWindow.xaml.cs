using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace StockManagement_WinUI3
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = (NavigationViewItem)args.SelectedItem;
            switch ((string)item.Tag)
            {
                case "productsPage":
                    RootFrame.Navigate(typeof(ProductsPage));
                    break;
                case "stockPage":
                    RootFrame.Navigate(typeof(StockPage));
                    break;
                default:
                    RootFrame.Navigate(typeof(StockPage));
                    break;
            }
        }

        private void MainWindow_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            RootFrame.Navigate(typeof(ProductsPage));
            MyNavigation.SelectedItem = MyNavigation.MenuItems[0];
            Activated -= MainWindow_OnActivated;
        }
    }
}
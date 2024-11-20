using Microsoft.UI.Xaml;

namespace StockManagement_WinUI3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _mWindow = new MainWindow();
            _mWindow.Activate();
        }

        private Window? _mWindow;
    }
}
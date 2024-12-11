using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;

namespace StockManagement_WinUI3
{
    public sealed partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            this.InitializeComponent();
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (AppViewModel.LoggedIn)
            {
                UserButton.Content = $"Logged in as {AppViewModel.Username}";
                UserButton.Visibility = Visibility.Visible;
                LoginButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginButton.Visibility = Visibility.Visible;
                UserButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginErrorBox.Visibility = Visibility.Collapsed;
            var username = LoginUsernameInput.Text;
            var password = LoginPasswordInput.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LoginErrorBox.Text = "Invalid Username or Password";
                LoginErrorBox.Visibility = Visibility.Visible;
                return;
            }

            var loginTask = AppViewModel.Login(username, password);
            loginTask.ContinueWith(task =>
            {
                var data = task.Result;
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!data.success)
                    {
                        LoginErrorBox.Text = "Invalid Username or Password";
                        LoginErrorBox.Visibility = Visibility.Visible;
                        return;
                    }

                    UserButton.Visibility = Visibility.Visible;
                    LoginButton.Visibility = Visibility.Collapsed;
                    UserButton.Content = $"Logged in as {data.user?.Username}";

                    LoginFlyout.Hide();
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserButton.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Visible;

            var _ = AppViewModel.Logout();
            LogoutFlyout.Hide();
        }
    }
}
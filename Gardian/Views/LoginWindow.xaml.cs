using System.Windows;
using Gardian.Managers;

namespace Gardian.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            TxtUsername.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TxtError.Text = "Please enter both username and password.";
                return;
            }

            if (UserSessionManager.TryLogin(username, password))
            {
                // Авторизация успешна — открываем терминал
                var osWin = new OsDesktopWindow();
                osWin.Show();
                this.Close();
            }
            else
            {
                TxtError.Text = "Invalid username or password.";
                TxtPassword.Clear();
                TxtPassword.Focus();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Создаем терминал и показываем его
            var terminal = new DosTerminalWindow();
            terminal.Show();

            // Закрываем окно авторизации
            this.Close();
        }

        // Enter в любом поле вызывает OK
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnOk_Click(null, null);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
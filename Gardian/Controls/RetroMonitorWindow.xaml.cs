using System.Windows;
using System.Windows.Input;

namespace Gardian.Controls
{
    public partial class RetroMonitorWindow : Window
    {
        public RetroMonitorWindow()
        {
            InitializeComponent();
        }

        // Закрытие по ESC
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        // Закрытие по клику
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Close();
        }
    }
}
using System.Windows;
using System.Windows.Input;
using Gardian.Views;

namespace Gardian
{
    public partial class App : Application
    {
        private HotkeysNoteWindow _hotkeysNote;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Глобальная обработка клавиш для всего приложения
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Подписываемся на нажатие клавиш на уровне всего приложения
            EventManager.RegisterClassHandler(
                typeof(Window),
                Window.PreviewKeyDownEvent,
                new KeyEventHandler(Global_KeyDownHandler),
                true);
        }

        private void Global_KeyDownHandler(object sender, KeyEventArgs e)
        {
            // F12 — открыть/закрыть записку с хоткеями (глобально)
            if (e.Key == Key.F12)
            {
                ToggleHotkeysNote();
                e.Handled = true;
            }
        }

        private void ToggleHotkeysNote()
        {
            if (_hotkeysNote == null || !_hotkeysNote.IsVisible)
            {
                _hotkeysNote = new HotkeysNoteWindow();

                // Позиционируем записку в левом верхнем углу активного окна
                var activeWindow = Application.Current.Windows.Cast<Window>()
                    .FirstOrDefault(w => w.IsActive);

                if (activeWindow != null)
                {
                    _hotkeysNote.Left = activeWindow.Left + 20;
                    _hotkeysNote.Top = activeWindow.Top + 20;
                }
                else
                {
                    _hotkeysNote.Left = 20;
                    _hotkeysNote.Top = 20;
                }

                _hotkeysNote.Show();
            }
            else
            {
                _hotkeysNote.Close();
                _hotkeysNote = null;
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Обработка необработанных исключений
            MessageBox.Show($"Произошла ошибка: {e.Exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
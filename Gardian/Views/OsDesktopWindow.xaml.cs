using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Gardian.Apps;
using Gardian.Controls;
using Gardian.Managers;
using Gardian.Utils;
using Gardian.ViewModels;

namespace Gardian.Views
{
    // НАСЛЕДУЕМСЯ ОТ CrtWindow, А НЕ ОТ Window
    public partial class OsDesktopWindow : CrtWindow
    {
        private readonly DispatcherTimer _clockTimer;
        private readonly List<MockWindow> _openWindows = new();
        private HotkeysNoteWindow _hotkeysNote;

        public OsDesktopWindow()
        {
            InitializeComponent();

            // Таймер часов
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (s, e) => ClockText.Text = DateTime.Now.ToString("HH:mm");
            _clockTimer.Start();

            // Подписка на тему (шейдером занимается базовый класс CrtWindow)
            SettingsManager.SettingsChanged += OnSettingsChanged;
            ApplyThemeToDesktop();
        }

        private void OnSettingsChanged()
        {
            ApplyThemeToDesktop();
        }

        private void ApplyThemeToDesktop()
        {
            var bgColor = SettingsManager.CurrentTheme == SettingsManager.ThemeType.Win98
                ? (Color)ColorConverter.ConvertFromString("#008080")
                : SettingsManager.GetDesktopBackgroundColor();

            RootGrid.Background = new SolidColorBrush(bgColor);
            Taskbar.Background = new SolidColorBrush(SettingsManager.GetControlFaceColor());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSessionManager.IsLoggedIn)
            {
                var terminal = new DosTerminalWindow();
                terminal.Show();
                this.Close();
                return;
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F12)
            {
                ToggleHotkeysNote();
                e.Handled = true;
                return;
            }

            if (Gardian.Utils.KeyboardHandler.HandleHotkeys(e.Key))
            {
                e.Handled = true;
                SettingsManager.Apply();
            }
        }

        private void ToggleHotkeysNote()
        {
            if (_hotkeysNote == null || !_hotkeysNote.IsVisible)
            {
                _hotkeysNote = new HotkeysNoteWindow();
                _hotkeysNote.Left = this.Left + 20;
                _hotkeysNote.Top = this.Top + 20;
                _hotkeysNote.Show();
            }
            else
            {
                _hotkeysNote.Close();
                _hotkeysNote = null;
            }
        }

        private void Icon_TestWindow_Click(object sender, RoutedEventArgs e) => OpenMockWindow("Test Window", CreateTestContent());
        private void Icon_About_Click(object sender, RoutedEventArgs e) => OpenMockWindow("About GARDIAN OS", CreateAboutContent());

        private void Icon_Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }

        private void Icon_Skype_Click(object sender, RoutedEventArgs e)
        {
            var skypeWindow = new MockWindow { Title = "Skype™", Width = 600, Height = 450 };
            var skypeApp = new SkypeApp();

            string currentUser = UserSessionManager.CurrentUser?.Username ?? "unknown";
            var viewModel = new SkypeViewModel(currentUser);
            skypeApp.Initialize(viewModel);

            skypeWindow.WindowContent.Content = skypeApp;
            Canvas.SetLeft(skypeWindow, 150);
            Canvas.SetTop(skypeWindow, 100);
            Canvas.SetZIndex(skypeWindow, 100);

            DesktopCanvas.Children.Add(skypeWindow);
        }

        private void OpenMockWindow(string title, UIElement content)
        {
            var mockWindow = new MockWindow { Title = title, Width = 400, Height = 300 };
            mockWindow.WindowContent.Content = content;
            mockWindow.CloseRequested += MockWindow_CloseRequested;
            mockWindow.MinimizeRequested += MockWindow_MinimizeRequested;

            Canvas.SetLeft(mockWindow, 100 + _openWindows.Count * 30);
            Canvas.SetTop(mockWindow, 100 + _openWindows.Count * 30);
            Canvas.SetZIndex(mockWindow, _openWindows.Count + 1);

            DesktopCanvas.Children.Add(mockWindow);
            _openWindows.Add(mockWindow);
            UpdateTaskbarButtons();
        }

        private void MockWindow_CloseRequested(object sender, RoutedEventArgs e)
        {
            if (sender is MockWindow window)
            {
                _openWindows.Remove(window);
                UpdateTaskbarButtons();
            }
        }

        private void MockWindow_MinimizeRequested(object sender, RoutedEventArgs e) => UpdateTaskbarButtons();

        private void UpdateTaskbarButtons()
        {
            TaskbarButtons.Children.Clear();
            foreach (var window in _openWindows)
            {
                var btn = new Button
                {
                    Content = window.Title,
                    Width = 120,
                    Height = 30,
                    Margin = new Thickness(2, 0, 2, 0),
                    FontFamily = new FontFamily("Tahoma"),
                    FontSize = 11,
                    Tag = window
                };
                btn.Click += (s, e) =>
                {
                    var targetWindow = (MockWindow)((Button)s).Tag;
                    targetWindow.Visibility = Visibility.Visible;
                    targetWindow.BringToFront();
                };
                TaskbarButtons.Children.Add(btn);
            }
        }

        private UIElement CreateTestContent()
        {
            var stack = new StackPanel { Margin = new Thickness(10) };
            stack.Children.Add(new Button { Content = "Test Button", Width = 150, Height = 30, Margin = new Thickness(0, 0, 0, 10) });
            stack.Children.Add(new CheckBox { Content = "Test CheckBox", Margin = new Thickness(0, 0, 0, 10) });
            stack.Children.Add(new TextBox { Text = "Test TextBox", Width = 200, Margin = new Thickness(0, 0, 0, 10) });
            stack.Children.Add(new ProgressBar { Width = 200, Height = 20, IsIndeterminate = true, Margin = new Thickness(0, 0, 0, 10) });
            return stack;
        }

        private UIElement CreateAboutContent()
        {
            var stack = new StackPanel { Margin = new Thickness(10) };
            stack.Children.Add(new TextBlock { Text = "GARDIAN OS v0.1.0", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });
            stack.Children.Add(new TextBlock { Text = "A retro-futuristic terminal environment.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) });
            stack.Children.Add(new TextBlock { Text = "Built with WPF & Pixel Shaders.", TextWrapping = TextWrapping.Wrap });
            return stack;
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show(this, "Start Menu (Coming Soon)", "GARDIAN", MessageBoxButton.OK, MessageBoxImage.Information);

        private void Btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            SettingsManager.SettingsChanged -= OnSettingsChanged;
            base.OnClosed(e);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Gardian.Apps;
using Gardian.Controls;
using Gardian.Managers;
using Gardian.ViewModels;

namespace Gardian.Views
{
    public partial class OsDesktopWindow : Window
    {
        private readonly DispatcherTimer _clockTimer;
        private DosTerminalWindow _parentTerminal;
        private readonly List<MockWindow> _openWindows = new();
        
        public OsDesktopWindow()
        {
            InitializeComponent();
            // 1. Подписываемся на изменения настроек (чтобы рабочий стол обновлялся, 
            // если пользователь открыл настройки, не закрывая рабочий стол)
            SettingsManager.SettingsChanged += OnSettingsChanged;

            // 2. Сразу применяем текущие настройки из памяти
            ApplyShaderSettings();

            // ... остальной код конструктора (таймеры и т.д.) ...
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _clockTimer.Tick += (s, e) => ClockText.Text = DateTime.Now.ToString("HH:mm");
            _clockTimer.Start();

            SettingsManager.SettingsChanged += OnSettingsChanged;

        }
        private void OnSettingsChanged()
        {
            ApplyShaderSettings();
            ApplyThemeToDesktop();
        }

        // Этот метод берет значения из SettingsManager и передает их шейдеру
        private void ApplyShaderSettings()
        {
            if (CrtEffect == null) return;

            CrtEffect.EnableScanline = SettingsManager.EnableScanlines ? 1.0 : 0.0;
            CrtEffect.EnableDistortion = SettingsManager.EnableVignette ? 1.0 : 0.0;
            CrtEffect.EnableChromatic = SettingsManager.EnableChromatic ? 1.0 : 0.0;
            CrtEffect.EnableBlur = SettingsManager.EnableBlur ? 1.0 : 0.0;
            CrtEffect.EnableNoise = SettingsManager.EnableNoise ? 1.0 : 0.0;
            CrtEffect.GlowStrength = SettingsManager.EnableBlur ? 0.4 : 0.0;

            CrtEffect.DistortionAmount = SettingsManager.DistortionAmount;
            CrtEffect.ChromaticSpread = SettingsManager.ChromaticSpread;
            CrtEffect.ScanlineFactor = SettingsManager.ScanlineFactor;
            CrtEffect.ScanlineFrequency = SettingsManager.ScanlineFrequency;
            CrtEffect.PhosphorGlow = SettingsManager.PhosphorGlow;
            CrtEffect.BrightnessBoost = SettingsManager.BrightnessBoost;
            CrtEffect.NoiseAmount = SettingsManager.NoiseAmount;
            CrtEffect.LineSpeed = SettingsManager.LineSpeed;
            CrtEffect.LineWidth = SettingsManager.LineWidth;
        }

        // Не забудь отписаться при закрытии окна, чтобы не было утечек памяти
        protected override void OnClosed(EventArgs e)
        {
            SettingsManager.SettingsChanged -= OnSettingsChanged;
            base.OnClosed(e);
        }
        private void Icon_Skype_Click(object sender, RoutedEventArgs e)
        {
            var skypeWindow = new MockWindow
            {
                Title = "Skype™",
                Width = 600,
                Height = 450
            };

            var skypeApp = new SkypeApp();

            // ← КЛЮЧЕВОЙ МОМЕНТ: Передаем текущего пользователя
            string currentUser = "admin_user"; // В будущем бери из SettingsManager или AuthManager
            var viewModel = new SkypeViewModel(currentUser);

            skypeApp.Initialize(viewModel);

            skypeWindow.WindowContent.Content = skypeApp;

            Canvas.SetLeft(skypeWindow, 150);
            Canvas.SetTop(skypeWindow, 100);
            Canvas.SetZIndex(skypeWindow, 100);

            DesktopCanvas.Children.Add(skypeWindow);
        }

        private void OnSettingsChanged() => ApplyThemeToDesktop();

        private void ApplyThemeToDesktop()
        {
            var bgColor = SettingsManager.CurrentTheme == SettingsManager.ThemeType.Win98
                ? (Color)ColorConverter.ConvertFromString("#008080")
                : SettingsManager.GetDesktopBackgroundColor();

            RootGrid.Background = new SolidColorBrush(bgColor);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Если не авторизован — выкидываем в DOS
            if (!UserSessionManager.IsLoggedIn)
            {
                var terminal = new DosTerminalWindow();
                terminal.Show();
                this.Close();
                return;
            }

            // Остальной код загрузки...
            //ApplyShaderSettings();
            ApplyThemeToDesktop();
        }

        // === СОЗДАНИЕ ВИРТУАЛЬНЫХ ОКОН ===
        private void Icon_TestWindow_Click(object sender, RoutedEventArgs e)
        {
            OpenMockWindow("Test Window", CreateTestContent());
        }

        private void Icon_Settings_Click(object sender, RoutedEventArgs e)
        {
            // Settings всё ещё открываем как отдельное окно, потому что оно модальное
            var settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }

        private void Icon_About_Click(object sender, RoutedEventArgs e)
        {
            OpenMockWindow("About GARDIAN OS", CreateAboutContent());
        }
        
        private void OpenMockWindow(string title, UIElement content)
        {
            var mockWindow = new MockWindow
            {
                Title = title,
                Width = 400,
                Height = 300
            };

            mockWindow.WindowContent.Content = content;
            mockWindow.CloseRequested += MockWindow_CloseRequested;
            mockWindow.MinimizeRequested += MockWindow_MinimizeRequested;

            // Позиционируем окно
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

        private void MockWindow_MinimizeRequested(object sender, RoutedEventArgs e)
        {
            UpdateTaskbarButtons();
        }

        // === ОБНОВЛЕНИЕ ПАНЕЛИ ЗАДАЧ ===
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
                    if (targetWindow.Visibility == Visibility.Collapsed)
                    {
                        targetWindow.Visibility = Visibility.Visible;
                        targetWindow.BringToFront();
                    }
                    else
                    {
                        targetWindow.BringToFront();
                    }
                };

                TaskbarButtons.Children.Add(btn);
            }
        }

        // === КОНТЕНТ ОКОН ===
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

        // === ПАНЕЛЬ ЗАДАЧ ===
        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "Start Menu (Coming Soon)", "GARDIAN", MessageBoxButton.OK, MessageBoxImage.Information);
        }

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
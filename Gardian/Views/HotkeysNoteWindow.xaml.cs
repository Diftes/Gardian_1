using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Gardian.Managers;

namespace Gardian.Views
{
    public partial class HotkeysNoteWindow : Window
    {
        public HotkeysNoteWindow()
        {
            InitializeComponent();

            // Подписываемся на изменения настроек — записка обновляется в реальном времени
            SettingsManager.SettingsChanged += OnSettingsChanged;

            // Начальное заполнение
            RebuildHotkeysList();
        }

        private void OnSettingsChanged()
        {
            // Пересоздаём список при любом изменении настроек
            RebuildHotkeysList();
        }

        private void RebuildHotkeysList()
        {
            HotkeysList.Children.Clear();

            // F3 — Vignette / Distortion
            AddHotkeyRow("F3", "Vignette", SettingsManager.EnableVignette);

            // F4 — Phosphor Glow
            AddHotkeyRow("F4", "Phosphor", SettingsManager.PhosphorGlow > 0);

            // F5 — Noise
            AddHotkeyRow("F5", "Noise", SettingsManager.EnableNoise);

            // F6 — Brightness
            AddHotkeyRow("F6", "Brightness", SettingsManager.BrightnessBoost > 0);

            // F7 — Blur / Bloom
            AddHotkeyRow("F7", "Blur", SettingsManager.EnableBlur);

            // F8 — Glitch Speed
            AddHotkeyRow("F8", "Glitch Spd", SettingsManager.LineSpeed > 0);

            // F9 — Glitch Width
            AddHotkeyRow("F9", "Glitch Wth", SettingsManager.LineWidth > 0);

            // Разделитель
            HotkeysList.Children.Add(new Separator { Margin = new Thickness(0, 4, 0, 4) });

            // Тема
            AddInfoRow("Theme", SettingsManager.CurrentTheme.ToString());
        }

        private void AddHotkeyRow(string key, string name, bool isEnabled)
        {
            var row = new DockPanel { Margin = new Thickness(0, 2, 0, 2) };

            var keyBlock = new TextBlock
            {
                Text = key,
                FontWeight = FontWeights.Bold,
                Width = 30,
                Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(keyBlock, Dock.Left);

            var nameBlock = new TextBlock
            {
                Text = name,
                Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55)),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(nameBlock, Dock.Left);

            var statusBlock = new TextBlock
            {
                Text = isEnabled ? "ON" : "OFF",
                FontWeight = FontWeights.Bold,
                Foreground = isEnabled
                    ? new SolidColorBrush(Color.FromRgb(0x00, 0x80, 0x00))  // зелёный
                    : new SolidColorBrush(Color.FromRgb(0xAA, 0x00, 0x00)), // красный
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(statusBlock, Dock.Right);

            row.Children.Add(keyBlock);
            row.Children.Add(nameBlock);
            row.Children.Add(statusBlock);

            HotkeysList.Children.Add(row);
        }

        private void AddInfoRow(string name, string value)
        {
            var row = new DockPanel { Margin = new Thickness(0, 2, 0, 2) };

            var nameBlock = new TextBlock
            {
                Text = name + ":",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33)),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(nameBlock, Dock.Left);

            var valueBlock = new TextBlock
            {
                Text = value,
                Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55)),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(valueBlock, Dock.Right);

            row.Children.Add(nameBlock);
            row.Children.Add(valueBlock);

            HotkeysList.Children.Add(row);
        }

        // Перетаскивание записки мышью
        private void NoteBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            SettingsManager.SettingsChanged -= OnSettingsChanged;
            base.OnClosed(e);
        }
    }
}
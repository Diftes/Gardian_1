using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Gardian.Managers;


namespace Gardian.Controls
{
    public partial class CrtPreviewPanel : UserControl
    {
        private readonly DispatcherTimer _timer;

        public CrtPreviewPanel()
        {
            InitializeComponent();

            // Анимация времени для шейдера (чтобы работали сканлайны и глитч)
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            _timer.Tick += (s, e) =>
            {
                if (PreviewEffect != null) PreviewEffect.Time += 0.03;
            };
            _timer.Start();

            // Инициализация текущими настройками
            UpdateFromSettings();
        }

        public void SetPreviewMode(int mode)
        {
            ViewTestPattern.Visibility = mode == 1 ? Visibility.Visible : Visibility.Collapsed;
            ViewRgbText.Visibility = mode == 2 ? Visibility.Visible : Visibility.Collapsed;
            ViewTheme.Visibility = mode == 3 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void UpdateFromSettings()
        {
            if (PreviewEffect == null) return;

            PreviewEffect.EnableDistortion = SettingsManager.EnableVignette ? 1.0 : 0.0;
            PreviewEffect.EnableChromatic = SettingsManager.EnableChromatic ? 1.0 : 0.0;
            PreviewEffect.EnableBlur = SettingsManager.EnableBlur ? 1.0 : 0.0;
            PreviewEffect.EnableNoise = SettingsManager.EnableNoise ? 1.0 : 0.0;
            PreviewEffect.GlowStrength = SettingsManager.EnableBlur ? 0.5 : 0.0;

            PreviewEffect.DistortionAmount = SettingsManager.DistortionAmount;
            PreviewEffect.ChromaticSpread = SettingsManager.ChromaticSpread;
            PreviewEffect.NoiseAmount = SettingsManager.NoiseAmount;
            PreviewEffect.LineSpeed = SettingsManager.LineSpeed;
            PreviewEffect.LineWidth = SettingsManager.LineWidth;
        }
    }
}
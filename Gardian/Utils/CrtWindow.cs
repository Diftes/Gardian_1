using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Gardian.Effects;
using Gardian.Managers;

namespace Gardian.Utils
{
    public class CrtWindow : Window
    {
        private CrtShaderEffect _crtEffect;
        private DispatcherTimer _shaderTimer;

        public CrtWindow()
        {
            this.Loaded += CrtWindow_Loaded;
            SettingsManager.SettingsChanged += OnSettingsChanged;
        }

        private void CrtWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Content is FrameworkElement rootElement)
            {
                _crtEffect = new CrtShaderEffect();
                rootElement.Effect = _crtEffect;

                _shaderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
                _shaderTimer.Tick += (s, args) =>
                {
                    if (_crtEffect != null)
                    {
                        _crtEffect.Time += 0.03;
                        _crtEffect.Resolution = new Vector(ActualWidth, ActualHeight);
                    }
                };
                _shaderTimer.Start();
                ApplySettings();
            }
        }

        private void OnSettingsChanged() => ApplySettings();

        private void ApplySettings()
        {
            if (_crtEffect == null) return;
            _crtEffect.EnableDistortion = SettingsManager.EnableVignette ? 1.0 : 0.0;
            _crtEffect.EnableNoise = SettingsManager.EnableNoise ? 1.0 : 0.0;
            _crtEffect.EnableChromatic = 1.0;
            _crtEffect.EnableBlur = SettingsManager.EnableBlur ? 1.0 : 0.0;
            _crtEffect.GlowStrength = SettingsManager.EnableBlur ? 0.4 : 0.0;
            _crtEffect.PhosphorGlow = SettingsManager.PhosphorGlow;
            _crtEffect.BrightnessBoost = SettingsManager.BrightnessBoost;
            _crtEffect.DistortionAmount = SettingsManager.DistortionAmount;
            _crtEffect.ChromaticSpread = SettingsManager.ChromaticSpread;
            _crtEffect.NoiseAmount = SettingsManager.NoiseAmount;
            _crtEffect.LineSpeed = SettingsManager.LineSpeed;
            _crtEffect.LineWidth = SettingsManager.LineWidth;
        }

        protected override void OnClosed(EventArgs e)
        {
            _shaderTimer?.Stop();
            SettingsManager.SettingsChanged -= OnSettingsChanged;
            base.OnClosed(e);
        }
    }
}
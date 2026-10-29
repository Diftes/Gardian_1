using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Gardian.Managers;
using Gardian.Utils;

namespace Gardian.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.Loaded += SettingsWindow_Loaded;
            this.Closed += SettingsWindow_Closed;
            
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            Mouse.OverrideCursor = Cursors.Arrow;
            UpdateCheckBoxesFromSettings();
            UpdateSlidersFromSettings();
            UpdateLabels();
        }

        private void SettingsWindow_Closed(object sender, System.EventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void UpdateCheckBoxesFromSettings()
        {

            if (ChkDistortion != null) ChkDistortion.IsChecked = SettingsManager.EnableVignette;
            if (ChkPhosphorGlow != null) ChkPhosphorGlow.IsChecked = SettingsManager.PhosphorGlow > 0;
            if (ChkNoise != null) ChkNoise.IsChecked = SettingsManager.EnableNoise;
            if (ChkBrightness != null) ChkBrightness.IsChecked = SettingsManager.BrightnessBoost > 0;
            if (ChkBlur != null) ChkBlur.IsChecked = SettingsManager.EnableBlur;
            if (ChkGlitchSpeed != null) ChkGlitchSpeed.IsChecked = SettingsManager.LineSpeed > 0;
            if (ChkGlitchWidth != null) ChkGlitchWidth.IsChecked = SettingsManager.LineWidth > 0;
        }

        private void UpdateSlidersFromSettings()
        {
            if (SldDistortion != null) SldDistortion.Value = SettingsManager.DistortionAmount;
            if (SldChromatic != null) SldChromatic.Value = SettingsManager.ChromaticSpread;
            if (SldPhosphorGlow != null) SldPhosphorGlow.Value = SettingsManager.PhosphorGlow;
            if (SldBrightness != null) SldBrightness.Value = SettingsManager.BrightnessBoost;
            if (SldNoise != null) SldNoise.Value = SettingsManager.NoiseAmount;
            if (SldSpeed != null) SldSpeed.Value = SettingsManager.LineSpeed;
            if (SldWidth != null) SldWidth.Value = SettingsManager.LineWidth;
        }

        private void ToggleSetting(object sender, RoutedEventArgs e)
        {
            if (ChkDistortion != null) SettingsManager.EnableVignette = ChkDistortion.IsChecked == true;
            if (ChkPhosphorGlow != null) SettingsManager.PhosphorGlow = ChkPhosphorGlow.IsChecked == true ? SettingsManager.LastPhosphorGlow : 0;
            if (ChkNoise != null) SettingsManager.EnableNoise = ChkNoise.IsChecked == true;
            if (ChkBrightness != null) SettingsManager.BrightnessBoost = ChkBrightness.IsChecked == true ? SettingsManager.LastBrightnessBoost : 0;
            if (ChkBlur != null) SettingsManager.EnableBlur = ChkBlur.IsChecked == true;
            if (ChkGlitchSpeed != null) SettingsManager.LineSpeed = ChkGlitchSpeed.IsChecked == true ? SettingsManager.LastLineSpeed : 0;
            if (ChkGlitchWidth != null) SettingsManager.LineWidth = ChkGlitchWidth.IsChecked == true ? SettingsManager.LastLineWidth : 0;

            SettingsManager.Apply();
            ApplyToActiveTerminal();
            PreviewPanel?.UpdateFromSettings();
        }

        private void TweakChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SldDistortion != null) SettingsManager.DistortionAmount = SldDistortion.Value;
            if (SldChromatic != null) SettingsManager.ChromaticSpread = SldChromatic.Value;

            // ВАЖНО: Обновляем и основное, и Last значение, чтобы toggle запоминал твой выбор!
            if (SldPhosphorGlow != null) { SettingsManager.PhosphorGlow = SldPhosphorGlow.Value; if (SldPhosphorGlow.Value > 0) SettingsManager.LastPhosphorGlow = SldPhosphorGlow.Value; }
            if (SldBrightness != null) { SettingsManager.BrightnessBoost = SldBrightness.Value; if (SldBrightness.Value > 0) SettingsManager.LastBrightnessBoost = SldBrightness.Value; }

            if (SldNoise != null) SettingsManager.NoiseAmount = SldNoise.Value;

            if (SldSpeed != null) { SettingsManager.LineSpeed = SldSpeed.Value; if (SldSpeed.Value > 0) SettingsManager.LastLineSpeed = SldSpeed.Value; }
            if (SldWidth != null) { SettingsManager.LineWidth = SldWidth.Value; if (SldWidth.Value > 0) SettingsManager.LastLineWidth = SldWidth.Value; }

            UpdateLabels();
            ApplyToActiveTerminal();
            PreviewPanel?.UpdateFromSettings();
        }

        private void UpdateLabels()
        {
            if (TxtDistortion != null && SldDistortion != null) TxtDistortion.Text = SldDistortion.Value.ToString("F2");
            if (TxtChromatic != null && SldChromatic != null) TxtChromatic.Text = SldChromatic.Value.ToString("F1");
            if (TxtScanline != null && SldScanline != null) TxtScanline.Text = SldScanline.Value.ToString("F2");
            if (TxtScanlineFreq != null && SldScanlineFreq != null) TxtScanlineFreq.Text = SldScanlineFreq.Value.ToString("F1");
            if (TxtPhosphorGlow != null && SldPhosphorGlow != null) TxtPhosphorGlow.Text = SldPhosphorGlow.Value.ToString("F1");
            if (TxtBrightness != null && SldBrightness != null) TxtBrightness.Text = SldBrightness.Value.ToString("F2");
            if (TxtNoise != null && SldNoise != null) TxtNoise.Text = SldNoise.Value.ToString("F2");
            if (TxtSpeed != null && SldSpeed != null) TxtSpeed.Text = SldSpeed.Value.ToString("F1");
            if (TxtWidth != null && SldWidth != null) TxtWidth.Text = SldWidth.Value.ToString("F0");
        }

        private void ApplyToActiveTerminal()
        {
            foreach (var window in Application.Current.Windows)
            {
                if (window is DosTerminalWindow terminal && terminal.CrtEffect != null)
                {
                    terminal.CrtEffect.EnableDistortion = SettingsManager.EnableVignette ? 1.0 : 0.0;
                    terminal.CrtEffect.EnableChromatic = SettingsManager.EnableChromatic ? 1.0 : 0.0;
                    terminal.CrtEffect.EnableBlur = SettingsManager.EnableBlur ? 1.0 : 0.0;
                    terminal.CrtEffect.EnableNoise = SettingsManager.EnableNoise ? 1.0 : 0.0;
                    terminal.CrtEffect.GlowStrength = SettingsManager.EnableBlur ? 0.4 : 0.0;
                    terminal.CrtEffect.DistortionAmount = SettingsManager.DistortionAmount;
                    terminal.CrtEffect.ChromaticSpread = SettingsManager.ChromaticSpread;
                    terminal.CrtEffect.PhosphorGlow = SettingsManager.PhosphorGlow;
                    terminal.CrtEffect.BrightnessBoost = SettingsManager.BrightnessBoost;
                    terminal.CrtEffect.NoiseAmount = SettingsManager.NoiseAmount;
                    terminal.CrtEffect.LineSpeed = SettingsManager.LineSpeed;
                    terminal.CrtEffect.LineWidth = SettingsManager.LineWidth;
                }
            }
            ApplyThemeToActiveTerminal();
        }

        private void ApplyThemeToActiveTerminal()
        {
            foreach (var window in Application.Current.Windows)
            {
                if (window is DosTerminalWindow terminal)
                {
                    var mainColor = new SolidColorBrush(SettingsManager.GetThemeColor());
                    var dimColor = new SolidColorBrush(SettingsManager.GetThemeDimColor());

                    if (terminal.OutputLog != null) terminal.OutputLog.Foreground = mainColor;
                    if (terminal.PromptBlock != null) terminal.PromptBlock.Foreground = dimColor;
                    if (terminal.CmdInput != null)
                    {
                        terminal.CmdInput.Foreground = mainColor;
                        terminal.CmdInput.CaretBrush = mainColor;
                    }
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
                e.Handled = true;
                return;
            }

            if (KeyboardHandler.HandleHotkeys(e.Key))
            {
                e.Handled = true;
                UpdateCheckBoxesFromSettings();
                UpdateSlidersFromSettings();
                ApplyToActiveTerminal();
                PreviewPanel?.UpdateFromSettings();
            }
        }

        private void ThemeChanged(object sender, RoutedEventArgs e)
        {
            if (RadGreen != null && RadGreen.IsChecked == true)
                SettingsManager.CurrentTheme = SettingsManager.ThemeType.Green;
            else if (RadCyberpunk != null && RadCyberpunk.IsChecked == true)
                SettingsManager.CurrentTheme = SettingsManager.ThemeType.Cyberpunk;
            else if (RadWhite != null && RadWhite.IsChecked == true)
                SettingsManager.CurrentTheme = SettingsManager.ThemeType.White;
            else if (RadWin98 != null && RadWin98.IsChecked == true) // ← ЭТА СТРОКА КРИТИЧЕСКИ ВАЖНА
                SettingsManager.CurrentTheme = SettingsManager.ThemeType.Win98;

            SettingsManager.Apply();
            ApplyThemeToActiveTerminal();
        }

        private void ApplyPreset(double distortion, double chromatic, double scanlineFactor,
                                 double scanlineFreq, double phosphorGlow, double brightness,
                                 double noise, double lineSpeed, double lineWidth)
        {
            SettingsManager.DistortionAmount = distortion;
            SettingsManager.ChromaticSpread = chromatic;
            SettingsManager.PhosphorGlow = phosphorGlow;
            SettingsManager.BrightnessBoost = brightness;
            SettingsManager.NoiseAmount = noise;
            SettingsManager.LineSpeed = lineSpeed;
            SettingsManager.LineWidth = lineWidth;

            // Синхронизируем Last значения с пресетом
            SettingsManager.LastPhosphorGlow = phosphorGlow > 0 ? phosphorGlow : 4.0;
            SettingsManager.LastBrightnessBoost = brightness > 0 ? brightness : 0.15;
            SettingsManager.LastLineSpeed = lineSpeed > 0 ? lineSpeed : 0.3;
            SettingsManager.LastLineWidth = lineWidth > 0 ? lineWidth : 20.0;

            UpdateSlidersFromSettings();
            UpdateLabels();
            ApplyToActiveTerminal();
            PreviewPanel?.UpdateFromSettings();
        }

        private void PresetModern_Click(object sender, RoutedEventArgs e) => ApplyPreset(0.01, 0.2, 0.3, 3.0, 0.0, 0.0, 0.05, 0.1, 10.0);
        private void PresetClassic_Click(object sender, RoutedEventArgs e) => ApplyPreset(0.06, 0.8, 0.7, 4.0, 4.0, 0.15, 0.15, 0.3, 20.0);
        private void PresetGlitch_Click(object sender, RoutedEventArgs e) => ApplyPreset(0.15, 3.0, 0.9, 8.0, 6.0, 0.2, 0.4, 1.5, 40.0);
        private void PresetMilitary_Click(object sender, RoutedEventArgs e) => ApplyPreset(0.04, 0.0, 0.85, 3.5, 1.0, 0.15, 0.08, 0.2, 15.0);

        private void ResetToRecommended_Click(object sender, RoutedEventArgs e) => PresetClassic_Click(sender, e);

        private void PreviewMode1_Click(object sender, RoutedEventArgs e) => PreviewPanel?.SetPreviewMode(1);
        private void PreviewMode2_Click(object sender, RoutedEventArgs e) => PreviewPanel?.SetPreviewMode(2);
        private void PreviewMode3_Click(object sender, RoutedEventArgs e) => PreviewPanel?.SetPreviewMode(3);

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Apply();
            ApplyToActiveTerminal();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ApplyToActiveTerminal();
            Close();
        }
    }
}
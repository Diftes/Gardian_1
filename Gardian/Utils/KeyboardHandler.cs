using System.Windows.Input;
using Gardian.Managers;

namespace Gardian.Utils
{
    public static class KeyboardHandler
    {
        public static bool HandleHotkeys(Key key)
        {
            switch (key)
            {
                /*  case Key.F1:
                    SettingsManager.EnableScanlines = !SettingsManager.EnableScanlines;
                    return true;

                case Key.F2:
                    if (SettingsManager.ScanlineFactor > 0)
                    {
                        SettingsManager.LastScanlineFactor = SettingsManager.ScanlineFactor;
                        SettingsManager.ScanlineFactor = 0;
                    }
                    else
                    {
                        SettingsManager.ScanlineFactor = SettingsManager.LastScanlineFactor > 0 ? SettingsManager.LastScanlineFactor : 0.7;
                    }
                    return true;   */

                case Key.F3:
                    SettingsManager.EnableVignette = !SettingsManager.EnableVignette;
                    return true;

                case Key.F4:
                    if (SettingsManager.PhosphorGlow > 0)
                    {
                        SettingsManager.LastPhosphorGlow = SettingsManager.PhosphorGlow;
                        SettingsManager.PhosphorGlow = 0;
                    }
                    else
                    {
                        SettingsManager.PhosphorGlow = SettingsManager.LastPhosphorGlow > 0 ? SettingsManager.LastPhosphorGlow : 4.0;
                    }
                    return true;

                case Key.F5:
                    SettingsManager.EnableNoise = !SettingsManager.EnableNoise;
                    return true;

                case Key.F6:
                    if (SettingsManager.BrightnessBoost > 0)
                    {
                        SettingsManager.LastBrightnessBoost = SettingsManager.BrightnessBoost;
                        SettingsManager.BrightnessBoost = 0;
                    }
                    else
                    {
                        SettingsManager.BrightnessBoost = SettingsManager.LastBrightnessBoost > 0 ? SettingsManager.LastBrightnessBoost : 0.15;
                    }
                    return true;

                case Key.F7:
                    SettingsManager.EnableBlur = !SettingsManager.EnableBlur;
                    return true;

                case Key.F8:
                    if (SettingsManager.LineSpeed > 0)
                    {
                        SettingsManager.LastLineSpeed = SettingsManager.LineSpeed;
                        SettingsManager.LineSpeed = 0;
                    }
                    else
                    {
                        SettingsManager.LineSpeed = SettingsManager.LastLineSpeed > 0 ? SettingsManager.LastLineSpeed : 0.3;
                    }
                    return true;

                case Key.F9:
                    if (SettingsManager.LineWidth > 0)
                    {
                        SettingsManager.LastLineWidth = SettingsManager.LineWidth;
                        SettingsManager.LineWidth = 0;
                    }
                    else
                    {
                        SettingsManager.LineWidth = SettingsManager.LastLineWidth > 0 ? SettingsManager.LastLineWidth : 20.0;
                    }
                    return true;
                case Key.F1:
                    // Открываем записку с хоткеями
                    // Возвращаем false, потому что само окно открывается через событие в главном окне
                    return false;
            }
            return false;
        }
    }
}

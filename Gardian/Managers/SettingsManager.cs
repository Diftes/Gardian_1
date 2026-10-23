using System;
using System.Windows.Media;

namespace Gardian.Managers
{
    public static class SettingsManager
    {
        public static event Action SettingsChanged;
        private static void OnSettingsChanged() => SettingsChanged?.Invoke();

        // === 1. ЛОГИЧЕСКИЕ ПЕРЕКЛЮЧАТЕЛИ ===
        public static bool EnableScanlines { get; set; } = true;
        public static bool EnableVignette { get; set; } = true;
        public static bool EnableNoise { get; set; } = true;
        public static bool EnableBlur { get; set; } = false;
        public static bool EnableChromatic { get; set; } = true;

        // === 2. ЧИСЛОВЫЕ НАСТРОЙКИ И "ПОСЛЕДНИЕ" ЗНАЧЕНИЯ ===
        public static double ScanlineFactor { get; set; } = 0.7;
        public static double LastScanlineFactor { get; set; } = 0.7;

        public static double PhosphorGlow { get; set; } = 4.0;
        public static double LastPhosphorGlow { get; set; } = 4.0;

        public static double BrightnessBoost { get; set; } = 0.15;
        public static double LastBrightnessBoost { get; set; } = 0.15;

        public static double LineSpeed { get; set; } = 0.3;
        public static double LastLineSpeed { get; set; } = 0.3;

        public static double LineWidth { get; set; } = 20.0;
        public static double LastLineWidth { get; set; } = 20.0;

        public static double DistortionAmount { get; set; } = 0.06;
        public static double ChromaticSpread { get; set; } = 0.8;
        public static double ScanlineFrequency { get; set; } = 4.0;
        public static double NoiseAmount { get; set; } = 0.2;

        // === 3. ТЕМЫ ===
        public enum ThemeType { Green, Cyberpunk, White, Win98 }
        public static ThemeType CurrentTheme { get; set; } = ThemeType.Green;

        public static Color GetThemeColor()
        {
            switch (CurrentTheme)
            {
                case ThemeType.Cyberpunk: return (Color)ColorConverter.ConvertFromString("#FF00FF");
                case ThemeType.White: return (Color)ColorConverter.ConvertFromString("#FFFFFF");
                case ThemeType.Win98: return (Color)ColorConverter.ConvertFromString("#000000");
                case ThemeType.Green:
                default: return (Color)ColorConverter.ConvertFromString("#00FF41");
            }
        }

        public static Color GetThemeDimColor()
        {
            switch (CurrentTheme)
            {
                case ThemeType.Cyberpunk: return (Color)ColorConverter.ConvertFromString("#AA00AA");
                case ThemeType.White: return (Color)ColorConverter.ConvertFromString("#AAAAAA");
                case ThemeType.Win98: return (Color)ColorConverter.ConvertFromString("#808080");
                case ThemeType.Green:
                default: return (Color)ColorConverter.ConvertFromString("#00AA22");
            }
        }

        public static Color GetDesktopBackgroundColor()
        {
            switch (CurrentTheme)
            {
                case ThemeType.Win98: return (Color)ColorConverter.ConvertFromString("#008080");
                case ThemeType.Cyberpunk: return (Color)ColorConverter.ConvertFromString("#0A0015");
                case ThemeType.White: return (Color)ColorConverter.ConvertFromString("#F0F0F0");
                case ThemeType.Green:
                default: return (Color)ColorConverter.ConvertFromString("#0A0A15");
            }
        }

        public static Color GetControlFaceColor()
        {
            switch (CurrentTheme)
            {
                case ThemeType.Win98: return (Color)ColorConverter.ConvertFromString("#C0C0C0");
                default: return (Color)ColorConverter.ConvertFromString("#111111");
            }
        }

        public static Uri GetThemeResourceUri()
        {
            switch (CurrentTheme)
            {
                case ThemeType.Win98:
                    return new Uri("/PresentationFramework.Classic;component/themes/Classic.xaml", UriKind.Relative);
                case ThemeType.Cyberpunk:
                    return new Uri("/PresentationFramework.Aero2;component/themes/Aero2.NormalColor.xaml", UriKind.Relative);
                default:
                    return null;
            }
        }

        public static bool IsUsingSystemTheme()
        {
            return CurrentTheme == ThemeType.Win98 || CurrentTheme == ThemeType.Cyberpunk;
        }

        // Этот метод вызывается, когда пользователь нажимает OK в настройках или двигает ползунок
        public static void Apply()
        {
            OnSettingsChanged();
        }
    }
}
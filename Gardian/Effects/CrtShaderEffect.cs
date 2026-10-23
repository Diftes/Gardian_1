using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Gardian.Effects
{
    public class CrtShaderEffect : ShaderEffect
    {
        // Входная текстура (автоматически берётся из элемента, к которому применён эффект)
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(CrtShaderEffect), 0);

        // Uniforms — соответствуют register(c0), c1, c2... в шейдере
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Vector), typeof(CrtShaderEffect),
                new UIPropertyMetadata(new Vector(800, 600), PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Color), typeof(CrtShaderEffect),
                new UIPropertyMetadata(Colors.Black, PixelShaderConstantCallback(4)));

        // Enable-флаги для управления эффектами из C#
        public static readonly DependencyProperty EnableDistortionProperty =
            DependencyProperty.Register("EnableDistortion", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(8)));

        public static readonly DependencyProperty EnableNoiseProperty =
            DependencyProperty.Register("EnableNoise", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(9)));

        public static readonly DependencyProperty EnableChromaticProperty =
            DependencyProperty.Register("EnableChromatic", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(10)));

        public static readonly DependencyProperty EnableBlurProperty =
            DependencyProperty.Register("EnableBlur", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(11)));

        public static readonly DependencyProperty EnableScanlineProperty =
            DependencyProperty.Register("EnableScanline", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(12)));
        // ... (предыдущие свойства) ...

        public static readonly DependencyProperty DistortionAmountProperty =
            DependencyProperty.Register("DistortionAmount", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(0.06, PixelShaderConstantCallback(14)));
        public double DistortionAmount { get => (double)GetValue(DistortionAmountProperty); set => SetValue(DistortionAmountProperty, value); }

        public static readonly DependencyProperty ChromaticSpreadProperty =
            DependencyProperty.Register("ChromaticSpread", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(0.8, PixelShaderConstantCallback(15)));
        public double ChromaticSpread { get => (double)GetValue(ChromaticSpreadProperty); set => SetValue(ChromaticSpreadProperty, value); }

        public static readonly DependencyProperty ScanlineFactorProperty =
            DependencyProperty.Register("ScanlineFactor", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(0.7, PixelShaderConstantCallback(16)));
        public double ScanlineFactor { get => (double)GetValue(ScanlineFactorProperty); set => SetValue(ScanlineFactorProperty, value); }

        public static readonly DependencyProperty NoiseAmountProperty =
            DependencyProperty.Register("NoiseAmount", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(0.2, PixelShaderConstantCallback(17)));
        public double NoiseAmount { get => (double)GetValue(NoiseAmountProperty); set => SetValue(NoiseAmountProperty, value); }

        public static readonly DependencyProperty LineSpeedProperty =
            DependencyProperty.Register("LineSpeed", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(0.3, PixelShaderConstantCallback(18)));
        public double LineSpeed { get => (double)GetValue(LineSpeedProperty); set => SetValue(LineSpeedProperty, value); }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(double), typeof(CrtShaderEffect), new UIPropertyMetadata(20.0, PixelShaderConstantCallback(19)));
        public double LineWidth { get => (double)GetValue(LineWidthProperty); set => SetValue(LineWidthProperty, value); }

        // В конец класса, перед закрывающей }
        public static readonly DependencyProperty ScanlineFrequencyProperty =
            DependencyProperty.Register("ScanlineFrequency", typeof(double), typeof(CrtShaderEffect),
            new UIPropertyMetadata(4.0, PixelShaderConstantCallback(20))); // Регистр c20

        public double ScanlineFrequency
        {
            get => (double)GetValue(ScanlineFrequencyProperty);
            set => SetValue(ScanlineFrequencyProperty, value);
        }

        public CrtShaderEffect()
        {
            // Загружаем скомпилированный шейдер из ресурсов проекта
            PixelShader = new PixelShader
            {
                UriSource = new Uri("pack://application:,,,/Gardian;component/Effects/CrtEffect.ps", UriKind.Absolute)
            };

            // Регистрируем все свойства для обновления шейдера
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(ScaleProperty);
            UpdateShaderValue(ResolutionProperty);
            UpdateShaderValue(BackgroundProperty);
            UpdateShaderValue(EnableDistortionProperty);
            UpdateShaderValue(EnableNoiseProperty);
            UpdateShaderValue(EnableChromaticProperty);
            UpdateShaderValue(EnableBlurProperty);
            UpdateShaderValue(EnableScanlineProperty);
            UpdateShaderValue(GlowStrengthProperty);
            UpdateShaderValue(DistortionAmountProperty);
            UpdateShaderValue(ChromaticSpreadProperty);
            UpdateShaderValue(ScanlineFactorProperty);
            UpdateShaderValue(NoiseAmountProperty);
            UpdateShaderValue(LineSpeedProperty);
            UpdateShaderValue(LineWidthProperty);
            UpdateShaderValue(PhosphorGlowProperty);
            UpdateShaderValue(BrightnessBoostProperty);


        }

        // C# свойства для удобного доступа из XAML и кода
        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Time
        {
            get => (double)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public Vector Resolution
        {
            get => (Vector)GetValue(ResolutionProperty);
            set => SetValue(ResolutionProperty, value);
        }

        public Color Background
        {
            get => (Color)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public double EnableDistortion
        {
            get => (double)GetValue(EnableDistortionProperty);
            set => SetValue(EnableDistortionProperty, value);
        }

        public double EnableNoise
        {
            get => (double)GetValue(EnableNoiseProperty);
            set => SetValue(EnableNoiseProperty, value);
        }

        public double EnableChromatic
        {
            get => (double)GetValue(EnableChromaticProperty);
            set => SetValue(EnableChromaticProperty, value);
        }

        public double EnableBlur
        {
            get => (double)GetValue(EnableBlurProperty);
            set => SetValue(EnableBlurProperty, value);
        }

        public double EnableScanline
        {
            get => (double)GetValue(EnableScanlineProperty);
            set => SetValue(EnableScanlineProperty, value);
        }

        // ... (предыдущие свойства) ...

        public static readonly DependencyProperty GlowStrengthProperty =
            DependencyProperty.Register("GlowStrength", typeof(double), typeof(CrtShaderEffect),
                new UIPropertyMetadata(0.3, PixelShaderConstantCallback(13))); // Регистр c13

        public double GlowStrength
        {
            get => (double)GetValue(GlowStrengthProperty);
            set => SetValue(GlowStrengthProperty, value);
        }

        public static readonly DependencyProperty PhosphorGlowProperty =
    DependencyProperty.Register("PhosphorGlow", typeof(double), typeof(CrtShaderEffect),
    new UIPropertyMetadata(0.0, PixelShaderConstantCallback(21)));

        public double PhosphorGlow
        {
            get => (double)GetValue(PhosphorGlowProperty);
            set => SetValue(PhosphorGlowProperty, value);
        }

        public static readonly DependencyProperty BrightnessBoostProperty =
            DependencyProperty.Register("BrightnessBoost", typeof(double), typeof(CrtShaderEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(22)));

        public double BrightnessBoost
        {
            get => (double)GetValue(BrightnessBoostProperty);
            set => SetValue(BrightnessBoostProperty, value);
        }
    }
}
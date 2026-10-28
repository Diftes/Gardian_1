using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Threading;
using Gardian.Managers;
using Gardian.Controls;
using Gardian.Utils; // ← ДОБАВЛЕНО: для KeyboardHandler

namespace Gardian.Views
{
    public partial class DosTerminalWindow : CrtWindow
    {
        private readonly List<string> _history = new();
        private int _historyIndex = -1;
        private string _draft = "";

        private char _currentDrive = 'A';
        private readonly DispatcherTimer _driveTimer;
        private readonly DispatcherTimer _shaderTimer;


        public DosTerminalWindow()
        {
            InitializeComponent();

            Mouse.OverrideCursor = Cursors.None;

            Print("GARDIAN SYSTEM v0.1.0 [Terminal Interface]");
            Print("Type 'help' to list available commands.");
            Print("");
            CmdInput.Focus();

            _driveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _driveTimer.Tick += DriveTimer_Tick;
            _driveTimer.Start();
            // 🔥 УНИВЕРСАЛЬНОЕ РЕШЕНИЕ: при любой активации окна возвращаем фокус
            this.Activated += (s, e) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (CmdInput != null && !CmdInput.IsFocused)
                    {
                        CmdInput.Focus();
                    }
                }), System.Windows.Threading.DispatcherPriority.Input);
            };

           
            

            // Применяем текущие настройки шейдера при запуске
            
            ApplyCurrentTheme();
        }

        private void DriveTimer_Tick(object? sender, EventArgs e)
        {
            _currentDrive = _currentDrive == 'Z' ? 'A' : (char)(_currentDrive + 1);
            PromptBlock.Text = $"{_currentDrive}:\\GARDIAN> ";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!CmdInput.IsFocused) CmdInput.Focus();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) => HandleHotkeys(e);

        // ✅ ИСПРАВЛЕНО: Теперь использует глобальный KeyboardHandler для F1-F9
        private void HandleHotkeys(KeyEventArgs e)
        {
            if (KeyboardHandler.HandleHotkeys(e.Key))
            {
                e.Handled = true; // Останавливаем всплытие
                SettingsManager.Apply();
                UpdateShaderFromSettings();
                CmdInput.Focus(); // Возвращаем фокус полю ввода
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CrtEffect != null)
            {
                CrtEffect.Resolution = new Vector(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void Print(string text)
        {
            var run = new Run(text + "\n")
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"))
            };
            OutputLog.Inlines.Add(run);
            LogScroll.ScrollToEnd();
        }

        private void PrintColored(string text, string hexColor)
        {
            var run = new Run(text + "\n");
            try
            {
                run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
            }
            catch
            {
                run.Foreground = Brushes.White;
            }
            OutputLog.Inlines.Add(run);
            LogScroll.ScrollToEnd();
        }

        private void CmdInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                string cmd = CmdInput.Text.Trim().ToLower();
                CmdInput.Text = "";
                ProcessCommand(cmd);
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                if (_history.Count > 0)
                {
                    _historyIndex = Math.Max(0, _historyIndex - 1);
                    CmdInput.Text = _history[_historyIndex];
                    CmdInput.CaretIndex = CmdInput.Text.Length;
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                if (_historyIndex < _history.Count - 1)
                {
                    _historyIndex++;
                    CmdInput.Text = _history[_historyIndex];
                }
                else
                {
                    _historyIndex = _history.Count;
                    CmdInput.Text = _draft;
                }
            }
        }

        private void ProcessCommand(string cmd)
        {
            PromptBlock.Text = $"{_currentDrive}:\\GARDIAN> ";
            Print($"{_currentDrive}:\\GARDIAN> {cmd}");

            if (string.IsNullOrEmpty(cmd)) return;

            _history.Add(cmd);
            _historyIndex = _history.Count;
            _draft = "";

            switch (cmd)
            {
                case "help":
                    Print("Available commands:");
                    Print("  os.exe       - Launch main desktop environment");
                    Print("  settings.exe - Open visual configuration panel");
                    Print("  monitor.exe  - View retro CRT monitor frame");
                    Print("  rgb          - Display RGB color test pattern");
                    Print("  cls          - Clear screen");
                    Print("  exit         - Close application");
                    break;

                case "os.exe":
                    Print("Launching OS environment...");
                    Mouse.OverrideCursor = null;
                    var osWin = new LoginWindow();
                    this.Hide();
                    osWin.Show();
                    break;

                case "settings.exe":
                    Print("Opening settings panel...");
                    var setWin = new SettingsWindow();
                    setWin.Owner = this;
                    setWin.ShowDialog();
                    this.Activate();
                    Mouse.OverrideCursor = Cursors.None;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CmdInput.Focus();
                    }), System.Windows.Threading.DispatcherPriority.Input);
                    break;

                case "monitor.exe":
                    Print("Launching retro monitor viewer...");
                    var monitorWin = new RetroMonitorWindow();
                    monitorWin.ShowDialog();
                    CmdInput.Focus();
                    break;

                case "rgb":
                    Print("Generating RGB test pattern...");
                    PrintColored("========================================", "#FFFFFF");
                    PrintColored(" RED:    RRRRRRRRRRRRRRRRRRRRRRRRRRRRRR", "#FF3333");
                    PrintColored(" GREEN:  GGGGGGGGGGGGGGGGGGGGGGGGGGGGGG", "#33FF33");
                    PrintColored(" BLUE:   BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB", "#3333FF");
                    PrintColored(" WHITE:  WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW", "#FFFFFF");
                    PrintColored(" YELLOW: YYYYYYYYYYYYYYYYYYYYYYYYYYYYYY", "#FFFF33");
                    PrintColored(" CYAN:   CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC", "#33FFFF");
                    PrintColored(" MAGENTA:MMMMMMMMMMMMMMMMMMMMMMMMMMMMMM", "#FF33FF");
                    PrintColored("========================================", "#FFFFFF");
                    Print("💡 Tip: Press F1-F9 to test effects on this pattern!");
                    break;

                case "cls":
                    OutputLog.Inlines.Clear(); // Очищаем Inlines вместо Text, так как мы используем цветной текст
                    break;

                case "exit":
                    Mouse.OverrideCursor = null;
                    Application.Current.Shutdown();
                    break;

                default:
                    Print($"'{cmd}' is not recognized as an internal or external command.");
                    break;
            }
            Print("");
        }

        // ✅ ИСПРАВЛЕНО: Добавлены новые параметры (Frequency, PhosphorGlow, Brightness)
        private void UpdateShaderFromSettings()
        {
            if (CrtEffect == null) return;

            // Флаги вкл/выкл

            CrtEffect.EnableDistortion = SettingsManager.EnableVignette ? 1.0 : 0.0;
            CrtEffect.EnableChromatic = SettingsManager.EnableChromatic ? 1.0 : 0.0;
            CrtEffect.EnableBlur = SettingsManager.EnableBlur ? 1.0 : 0.0;
            CrtEffect.EnableNoise = SettingsManager.EnableNoise ? 1.0 : 0.0;
            CrtEffect.GlowStrength = SettingsManager.EnableBlur ? 0.4 : 0.0;

            // Числовые значения
            CrtEffect.DistortionAmount = SettingsManager.DistortionAmount;
            CrtEffect.ChromaticSpread = SettingsManager.ChromaticSpread;
            CrtEffect.PhosphorGlow = SettingsManager.PhosphorGlow;          // ← ДОБАВЛЕНО
            CrtEffect.BrightnessBoost = SettingsManager.BrightnessBoost;    // ← ДОБАВЛЕНО
            CrtEffect.NoiseAmount = SettingsManager.NoiseAmount;
            CrtEffect.LineSpeed = SettingsManager.LineSpeed;
            CrtEffect.LineWidth = SettingsManager.LineWidth;
        }

        private void ApplyCurrentTheme()
        {
            var mainColor = new SolidColorBrush(SettingsManager.GetThemeColor());
            var dimColor = new SolidColorBrush(SettingsManager.GetThemeDimColor());

            if (OutputLog != null)
            {
                OutputLog.Foreground = mainColor;
                OutputLog.Background = Brushes.Transparent;
            }

            if (PromptBlock != null)
            {
                PromptBlock.Foreground = dimColor;
                PromptBlock.Background = Brushes.Transparent;
            }

            if (CmdInput != null)
            {
                CmdInput.Foreground = mainColor;
                CmdInput.Background = Brushes.Transparent; // ← КРИТИЧЕСКИ ВАЖНО
                CmdInput.CaretBrush = mainColor; // Цвет курсора
                CmdInput.SelectionBrush = new SolidColorBrush(
                    Color.FromArgb(100, mainColor.Color.R, mainColor.Color.G, mainColor.Color.B));
            }
        }
    }
}
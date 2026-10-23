using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Gardian.Controls;
using Gardian.Managers;
using Gardian.Utils; // ← ДОБАВЛЕНО: для KeyboardHandler

namespace Gardian.Views
{
    public partial class PostBootWindow : Window
    {
        private readonly List<string> _messages = new()
        {
            "Award Modular BIOS v4.51PG", "Copyright (C) 1984-98, Award Software Inc.", "",
            "PENTIUM-S CPU at 133MHz", "Memory Test :   65536K OK", "",
            "Detecting HDD Primary Master ... MAXTOR 2GB", "Initializing VGA BIOS...", "Starting Gardian..."
        };

        public PostBootWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.None; // Скрываем курсор

            foreach (var msg in _messages)
            {
                await Task.Delay(Random.Shared.Next(400, 900));
                Dispatcher.Invoke(() =>
                {
                    BootLog.Text += msg + "\n";
                    LogScroll.ScrollToEnd();
                });
            }

            BootBar.Visibility = Visibility.Visible;
            for (double p = 0; p <= 100; p += 5)
            {
                await Task.Delay(150);
                Dispatcher.Invoke(() => BootBar.Value = p);
            }

            var terminal = new DosTerminalWindow();
            terminal.Show();
            terminal.Activate();

            await Task.Delay(600);
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!BootBar.IsVisible)
            {
                e.Cancel = true;
            }
            else
            {
                Mouse.OverrideCursor = null; // Возвращаем курсор при закрытии
            }
        }

        // ✅ ИСПРАВЛЕНО: Теперь использует глобальный KeyboardHandler для F1-F9
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyboardHandler.HandleHotkeys(e.Key))
            {
                e.Handled = true;
                SettingsManager.Apply();
            }
        }
    }
}
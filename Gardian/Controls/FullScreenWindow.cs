using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gardian.Controls
{
    public class FullScreenWindow : Window
    {
        public FullScreenWindow()
        {
            // Эти настройки применятся ко ВСЕМ окнам, которые наследуют этот класс
            this.Loaded += (s, e) =>
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;
                this.ShowInTaskbar = false;
            };
        }
    }
}

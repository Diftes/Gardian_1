using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gardian.Controls
{
    public partial class MockWindow : UserControl
    {
        // === ПОЛЯ ДЛЯ RESIZE ===
        private enum ResizeDirection { None, TopLeft, TopRight, BottomLeft, BottomRight, Top, Bottom, Left, Right }
        private ResizeDirection _resizeDirection = ResizeDirection.None;
        private Point _resizeStartMouse;
        private double _resizeStartWidth;
        private double _resizeStartHeight;
        private double _resizeStartLeft;
        private double _resizeStartTop;

        private const double MinWidth = 200;
        private const double MinHeight = 150;

        // === ЛОГИКА RESIZE ===
        private void Resize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var handle = sender as Border;
            if (handle == null) return;

            var parentCanvas = VisualTreeHelper.GetParent(this) as Canvas;
            if (parentCanvas == null) return;

            // Определяем направление по имени элемента
            if (handle.Name == "ResizeTopLeft") _resizeDirection = ResizeDirection.TopLeft;
            else if (handle.Name == "ResizeTopRight") _resizeDirection = ResizeDirection.TopRight;
            else if (handle.Name == "ResizeBottomLeft") _resizeDirection = ResizeDirection.BottomLeft;
            else if (handle.Name == "ResizeBottomRight") _resizeDirection = ResizeDirection.BottomRight;
            else if (handle.Name == "ResizeTop") _resizeDirection = ResizeDirection.Top;
            else if (handle.Name == "ResizeBottom") _resizeDirection = ResizeDirection.Bottom;
            else if (handle.Name == "ResizeLeft") _resizeDirection = ResizeDirection.Left;
            else if (handle.Name == "ResizeRight") _resizeDirection = ResizeDirection.Right;

            // Запоминаем начальные значения
            _resizeStartMouse = e.GetPosition(parentCanvas);
            _resizeStartWidth = this.Width;
            _resizeStartHeight = this.Height;
            _resizeStartLeft = Canvas.GetLeft(this);
            _resizeStartTop = Canvas.GetTop(this);

            // Захватываем мышь
            handle.CaptureMouse();

            // Поднимаем окно наверх
            BringToFront();

            e.Handled = true;
        }

        private void Resize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var handle = sender as Border;
            handle?.ReleaseMouseCapture();
            _resizeDirection = ResizeDirection.None;
        }

        private void Resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizeDirection == ResizeDirection.None) return;

            var handle = sender as Border;
            if (handle == null || !handle.IsMouseCaptured) return;

            var parentCanvas = VisualTreeHelper.GetParent(this) as Canvas;
            if (parentCanvas == null) return;

            var currentMouse = e.GetPosition(parentCanvas);
            var deltaX = currentMouse.X - _resizeStartMouse.X;
            var deltaY = currentMouse.Y - _resizeStartMouse.Y;

            double newWidth = _resizeStartWidth;
            double newHeight = _resizeStartHeight;
            double newLeft = _resizeStartLeft;
            double newTop = _resizeStartTop;

            switch (_resizeDirection)
            {
                case ResizeDirection.Right:
                    newWidth = _resizeStartWidth + deltaX;
                    break;
                case ResizeDirection.Left:
                    newWidth = _resizeStartWidth - deltaX;
                    newLeft = _resizeStartLeft + deltaX;
                    break;
                case ResizeDirection.Bottom:
                    newHeight = _resizeStartHeight + deltaY;
                    break;
                case ResizeDirection.Top:
                    newHeight = _resizeStartHeight - deltaY;
                    newTop = _resizeStartTop + deltaY;
                    break;
                case ResizeDirection.BottomRight:
                    newWidth = _resizeStartWidth + deltaX;
                    newHeight = _resizeStartHeight + deltaY;
                    break;
                case ResizeDirection.BottomLeft:
                    newWidth = _resizeStartWidth - deltaX;
                    newLeft = _resizeStartLeft + deltaX;
                    newHeight = _resizeStartHeight + deltaY;
                    break;
                case ResizeDirection.TopRight:
                    newWidth = _resizeStartWidth + deltaX;
                    newHeight = _resizeStartHeight - deltaY;
                    newTop = _resizeStartTop + deltaY;
                    break;
                case ResizeDirection.TopLeft:
                    newWidth = _resizeStartWidth - deltaX;
                    newLeft = _resizeStartLeft + deltaX;
                    newHeight = _resizeStartHeight - deltaY;
                    newTop = _resizeStartTop + deltaY;
                    break;
            }

            // Применяем минимальные размеры
            if (newWidth < MinWidth)
            {
                if (_resizeDirection == ResizeDirection.Left || _resizeDirection == ResizeDirection.TopLeft || _resizeDirection == ResizeDirection.BottomLeft)
                {
                    newLeft -= (MinWidth - newWidth);
                }
                newWidth = MinWidth;
            }
            if (newHeight < MinHeight)
            {
                if (_resizeDirection == ResizeDirection.Top || _resizeDirection == ResizeDirection.TopLeft || _resizeDirection == ResizeDirection.TopRight)
                {
                    newTop -= (MinHeight - newHeight);
                }
                newHeight = MinHeight;
            }

            // Применяем изменения
            this.Width = newWidth;
            this.Height = newHeight;
            Canvas.SetLeft(this, newLeft);
            Canvas.SetTop(this, newTop);

            e.Handled = true;
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MockWindow), new PropertyMetadata("Window"));

        public event RoutedEventHandler CloseRequested;
        public event RoutedEventHandler MinimizeRequested;

        private Point _dragStartMouse;
        private Point _dragStartWindow;

        public MockWindow()
        {
            InitializeComponent();

            // Добавляем обработчик нажатия на всё окно, чтобы поднимать его наверх даже при клике на контент
            this.MouseLeftButtonDown += (s, e) => BringToFront();
        }

        // === ПЕРЕТАСКИВАНИЕ (ИСПРАВЛЕННОЕ) ===
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Двойной клик можно оставить для будущих улучшений, пока просто игнорируем
                return;
                // BtnMaximize_Click(sender, e); 
            }

            var parentCanvas = VisualTreeHelper.GetParent(this) as Canvas;
            if (parentCanvas != null)
            {
                // 1. Запоминаем позицию мыши относительно CANVAS
                _dragStartMouse = e.GetPosition(parentCanvas);

                // 2. Запоминаем текущую позицию окна на CANVAS
                _dragStartWindow = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

                // 3. Поднимаем окно наверх
                BringToFront();

                // 4. Захватываем мышь, чтобы перетаскивание работало, даже если курсор выйдет за границы окна
                TitleBar.CaptureMouse();
            }
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TitleBar.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (TitleBar.IsMouseCaptured)
            {
                var parentCanvas = VisualTreeHelper.GetParent(this) as Canvas;
                if (parentCanvas != null)
                {
                    // Считаем новую позицию мыши относительно CANVAS
                    var currentMouse = e.GetPosition(parentCanvas);

                    // Вычисляем смещение (дельту)
                    var deltaX = currentMouse.X - _dragStartMouse.X;
                    var deltaY = currentMouse.Y - _dragStartMouse.Y;

                    // Применяем смещение к исходной позиции окна
                    Canvas.SetLeft(this, _dragStartWindow.X + deltaX);
                    Canvas.SetTop(this, _dragStartWindow.Y + deltaY);
                }
            }
            base.OnMouseMove(e);
        }

        // === УПРАВЛЕНИЕ ОКНОМ ===
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, e);
            if (VisualTreeHelper.GetParent(this) is Canvas parentCanvas)
            {
                parentCanvas.Children.Remove(this);
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            MinimizeRequested?.Invoke(this, e);
        }
        // ✨ ДОБАВЛЯЕМ ЭТОТ МЕТОД: Разворачивание / Восстановление окна
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetParent(this) is Canvas parentCanvas)
            {
                // Простая проверка: если окно уже занимает весь Canvas (минус панель задач), сворачиваем его обратно
                if (this.Width == parentCanvas.ActualWidth && this.Height == parentCanvas.ActualHeight - 45)
                {
                    // Восстанавливаем стандартный размер и позицию
                    this.Width = 400;
                    this.Height = 300;
                    Canvas.SetLeft(this, 100);
                    Canvas.SetTop(this, 100);
                }
                else
                {
                    // Разворачиваем на весь доступный экран (высота Canvas минус 45px на панель задач)
                    this.Width = parentCanvas.ActualWidth;
                    this.Height = parentCanvas.ActualHeight - 45;
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                }

                // После изменения размера всё равно поднимаем окно наверх
                BringToFront();
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BringToFront();
        }

        // === Z-INDEX (ИСПРАВЛЕННОЕ) ===
        public void BringToFront()
        {
            if (VisualTreeHelper.GetParent(this) is Canvas parentCanvas)
            {
                int maxZ = 0;

                // Ищем максимальный Z-Index ТОЛЬКО среди ВИДИМЫХ окон
                foreach (UIElement child in parentCanvas.Children)
                {
                    if (child is MockWindow w && w.Visibility == Visibility.Visible)
                    {
                        int z = Canvas.GetZIndex(w);
                        if (z > maxZ) maxZ = z;
                    }
                }

                // Ставим текущее окно на уровень выше самого верхнего
                Canvas.SetZIndex(this, maxZ + 1);
            }
        }
    }
}
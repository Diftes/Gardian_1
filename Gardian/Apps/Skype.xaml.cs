using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Gardian.ViewModels;

namespace Gardian.Apps
{
    public partial class SkypeApp : UserControl
    {
        private readonly DispatcherTimer _botTimer;
        private string _lastMessage = "";
        private bool _isInCall = false;
        private SkypeViewModel _viewModel; // ← ДОБАВЛЕНО

        public SkypeApp()
        {
            InitializeComponent();
            _botTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            _botTimer.Tick += BotTimer_Tick;

            AppendToChat("System", "Echo-bot for sound testing is connected.");
        }

        // Вызывается ИЗВНЕ после создания UserControl
        public void Initialize(SkypeViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel; // ← ПРИВЯЗЫВАЕМ DATACONTEXT ЗДЕСЬ
        }

        private void Send_Click(object sender, RoutedEventArgs e) => SendMessage();
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string text = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            AppendToChat("retro_user", text);
            _lastMessage = text;
            MessageInput.Clear();

            if (!_isInCall) _botTimer.Start();
        }

        private void BotTimer_Tick(object sender, EventArgs e)
        {
            _botTimer.Stop();
            AppendToChat("Echo-bot", $"[ECHO]: {_lastMessage}");
        }

        private void AppendToChat(string senderName, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            ChatLog.Text += $"[{timestamp}] {senderName}: {message}\r\n";
            ChatLog.ScrollToEnd();
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            // ← ДОБАВЛЕНА ПРОВЕРКА
            if (_viewModel?.CanStartVN != true) return;

            _isInCall = true;
            NormalChatPanel.Visibility = Visibility.Collapsed;
            VisualNovelPanel.Visibility = Visibility.Visible;
            BtnEndCall.IsEnabled = true;

            StartVNDialogue();
        }

        private void EndCall_Click(object sender, RoutedEventArgs e)
        {
            _isInCall = false;
            NormalChatPanel.Visibility = Visibility.Visible;
            VisualNovelPanel.Visibility = Visibility.Collapsed;
            BtnEndCall.IsEnabled = false;
            AppendToChat("System", "Call ended.");
        }

        // Методы VN остаются без изменений (StartVNDialogue, AddVNChoice, OnChoice1-3)
        // ... (вставь сюда код VN из предыдущего ответа) ...

        private void StartVNDialogue()
        {
            VN_Choices.Children.Clear();
            VN_Subtitle.Text = "Echo-bot: *bzzzt* Connection established. Do you read me?";
            AddVNChoice("Yes, I read you loud and clear.", OnChoice1);
            AddVNChoice("No, the connection is terrible.", OnChoice2);
            AddVNChoice("Just testing the audio system.", OnChoice3);
        }

        private void AddVNChoice(string text, RoutedEventHandler handler)
        {
            var btn = new Button
            {
                Content = "> " + text,
                Margin = new Thickness(0, 5, 0, 0),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };
            btn.MouseEnter += (s, e) => btn.Background = Brushes.DarkGray;
            btn.MouseLeave += (s, e) => btn.Background = Brushes.Transparent;
            btn.Click += handler;
            VN_Choices.Children.Add(btn);
        }

        private void OnChoice1(object sender, RoutedEventArgs e)
        {
            VN_Subtitle.Text = "Echo-bot: Excellent. Audio channels are stable. *beep*";
            VN_Choices.Children.Clear();
        }

        private void OnChoice2(object sender, RoutedEventArgs e)
        {
            VN_Subtitle.Text = "Echo-bot: *static noise* I'm sorry, please repeat.";
            VN_Choices.Children.Clear();
            Dispatcher.BeginInvoke(new Action(() => {
                VN_Subtitle.Text = "Echo-bot: Are you still there?";
                AddVNChoice("Yes, testing again.", OnChoice1);
                AddVNChoice("Ending call.", (s, args) => EndCall_Click(s, args));
            }), TimeSpan.FromSeconds(2));
        }

        private void OnChoice3(object sender, RoutedEventArgs e)
        {
            VN_Subtitle.Text = "Echo-bot: Acknowledged. Test complete.";
            VN_Choices.Children.Clear();
            AddVNChoice("End Call", (s, args) => EndCall_Click(s, args));
        }
    }
}
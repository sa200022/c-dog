using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class MainWindow : Window
{
    private TextBox _inputBox;      // AI 輸入框
    private TextBlock _chatOutput;  // 顯示對話內容

    public MainWindow()
    {
        Title = "AI 視窗";
        Width = 800;
        Height = 500;

        // ===== 根 Grid，藍色背景 =====
        Grid root = new Grid
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 215))
        };

        // 上面是聊天區，下方是輸入列
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // ===== 右上角四個按鈕 =====
        StackPanel buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10)
        };

        Button btn1 = new Button { Content = "按鈕1", Margin = new Thickness(5) };
        Button btn2 = new Button { Content = "按鈕2", Margin = new Thickness(5) };
        Button btn3 = new Button { Content = "按鈕3", Margin = new Thickness(5) };
        Button btn4 = new Button { Content = "按鈕4", Margin = new Thickness(5) };

        btn1.Click += (s, e) => MessageBox.Show("你按了按鈕1");
        btn2.Click += (s, e) => MessageBox.Show("你按了按鈕2");
        btn3.Click += (s, e) => MessageBox.Show("你按了按鈕3");
        btn4.Click += (s, e) => MessageBox.Show("你按了按鈕4");

        buttonPanel.Children.Add(btn1);
        buttonPanel.Children.Add(btn2);
        buttonPanel.Children.Add(btn3);
        buttonPanel.Children.Add(btn4);

        Grid.SetRow(buttonPanel, 0);
        root.Children.Add(buttonPanel);

        // ===== 中間：AI 對話顯示區 =====
        _chatOutput = new TextBlock
        {
            Margin = new Thickness(10, 50, 10, 10),
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.White
        };

        ScrollViewer scroll = new ScrollViewer
        {
            Content = _chatOutput,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        Grid.SetRow(scroll, 0);
        root.Children.Add(scroll);

        // ===== 下方：AI 輸入列 =====
        Grid inputGrid = new Grid
        {
            Margin = new Thickness(10),
            Background = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0))
        };

        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        _inputBox = new TextBox
        {
            Margin = new Thickness(5),
            MinWidth = 200
        };
        Grid.SetColumn(_inputBox, 0);

        Button sendButton = new Button
        {
            Content = "送出",
            Margin = new Thickness(5),
            Padding = new Thickness(15, 5, 15, 5)
        };
        sendButton.Click += OnSendClicked;
        Grid.SetColumn(sendButton, 1);

        inputGrid.Children.Add(_inputBox);
        inputGrid.Children.Add(sendButton);

        Grid.SetRow(inputGrid, 1);
        root.Children.Add(inputGrid);

        // 設定視窗內容
        Content = root;
    }

    private void OnSendClicked(object sender, RoutedEventArgs e)
    {
        string userText = _inputBox.Text.Trim();
        if (string.IsNullOrEmpty(userText))
            return;

        // 先把使用者輸入顯示出來
        AppendLine($"你：{userText}");

        // 這裡未來改成「丟給 Python / OpenAI」，現在先假裝 AI 回覆
        string fakeAiReply = $"（AI 假裝回覆：你說的是「{userText}」）";
        AppendLine(fakeAiReply);

        _inputBox.Clear();
    }

    private void AppendLine(string text)
    {
        if (string.IsNullOrEmpty(_chatOutput.Text))
            _chatOutput.Text = text;
        else
            _chatOutput.Text += Environment.NewLine + text;
    }
}

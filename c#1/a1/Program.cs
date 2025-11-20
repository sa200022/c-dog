using System;
using System.Windows;
using System.Windows.Controls;

public class Program : Application
{
    [STAThread]
    public static void Main()
    {
        var app = new Program();
        var window = new Window()
        {
            Title = "WPF 純程式碼視窗",
            Width = 500,
            Height = 300
        };

        var panel = new StackPanel();
        var text = new TextBlock() { Text = "Hello WPF", Margin = new Thickness(10) };
        var btn = new Button() { Content = "按我", Margin = new Thickness(10) };

        btn.Click += (s, e) =>
        {
            MessageBox.Show("你按了按鈕！");
        };

        panel.Children.Add(text);
        panel.Children.Add(btn);
        window.Content = panel;

        app.Run(window);
    }
}

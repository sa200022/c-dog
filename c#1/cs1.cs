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
            Title = "純C# WPF",
            Width = 400,
            Height = 300
        };

        var panel = new StackPanel();

        var input = new TextBox() { Margin = new Thickness(10) };
        var button = new Button() { Content = "送出", Margin = new Thickness(10) };
        var output = new TextBlock() { Margin = new Thickness(10) };

        button.Click += (s, e) =>
        {
            output.Text = "你輸入了：" + input.Text;
        };

        panel.Children.Add(input);
        panel.Children.Add(button);
        panel.Children.Add(output);

        window.Content = panel;
        app.Run(window);
    }
}

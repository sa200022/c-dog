using System;
using System.Windows;

public class Program : Application
{
    [STAThread]
    public static void Main()
    {
        new Program().Run(new MainWindow());
    }
}

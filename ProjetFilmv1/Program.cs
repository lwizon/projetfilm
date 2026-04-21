using System;
using System.Windows;

namespace ProjetFilmv1;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App();
        app.Run(new MainWindow());
    }
}

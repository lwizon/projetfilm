using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ProjetFilmv1;

public partial class App : Application
{
    private const string LogFileName = "tmdb_debug.log";

    public bool IsDarkTheme { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
            var listener = new TextWriterTraceListener(logPath, "fileListener");
            Trace.Listeners.Add(listener);

            try
            {
                var debugType = typeof(Debug);
                var prop = debugType.GetProperty("Listeners", BindingFlags.Static | BindingFlags.Public);
                if (prop != null)
                {
                    var dbgListeners = prop.GetValue(null) as TraceListenerCollection;
                    dbgListeners?.Add(listener);
                }
            }
            catch
            {
            }

            Trace.AutoFlush = true;
        }
        catch
        {
        }

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        ApplyTheme(false);
        base.OnStartup(e);
    }

    public void ToggleTheme()
    {
        ApplyTheme(!IsDarkTheme);
    }

    public void ApplyTheme(bool darkTheme)
    {
        IsDarkTheme = darkTheme;

        SetBrush("AppBackgroundBrush", darkTheme ? "#10131A" : "#FFF8FC");
        SetBrush("SurfaceBrush", darkTheme ? "#181C24" : "#FFFFFF");
        SetBrush("SurfaceAltBrush", darkTheme ? "#1F2430" : "#F7F4FF");
        SetBrush("TextBrush", darkTheme ? "#F8FAFC" : "#18181B");
        SetBrush("SecondaryTextBrush", darkTheme ? "#CBD5E1" : "#5B6475");
        SetBrush("OnDarkTextBrush", darkTheme ? "#E6E1D8" : "#FFFFFF");
        SetBrush("AccentBrush", "#7D5FFF");
        SetBrush("AccentSoftBrush", darkTheme ? "#2A2246" : "#F1EDFF");
        SetBrush("BorderBrush", darkTheme ? "#31384A" : "#E5DEFF");
        SetBrush("NavBackgroundBrush", darkTheme ? "#0F141D" : "#141924");
        SetBrush("InputBorderBrush", darkTheme ? "#414B63" : "#D9D0FF");
        SetBrush("InputTextBrush", darkTheme ? "#F8FAFC" : "#18181B");
    }

    private void SetBrush(string key, string hexColor)
    {
        Resources[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Debug.WriteLine($"DispatcherUnhandledException: {e.Exception}");
        try { MessageBox.Show($"Erreur non gérée: {e.Exception}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Debug.WriteLine($"CurrentDomain_UnhandledException: {e.ExceptionObject}");
        try { MessageBox.Show($"Erreur critique: {e.ExceptionObject}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Debug.WriteLine($"UnobservedTaskException: {e.Exception}");
        try { MessageBox.Show($"Erreur tâche: {e.Exception}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
    }
}

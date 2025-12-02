using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ProjetFilmv1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string LogFileName = "tmdb_debug.log";

    protected override void OnStartup(StartupEventArgs e)
    {
        // ensure log file in app folder
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
            var listener = new TextWriterTraceListener(logPath, "fileListener");

            // add to Trace listeners (always available)
            Trace.Listeners.Add(listener);

            // try to add to Debug listeners via reflection (avoids compile/runtime issues where Debug.Listeners isn't available)
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
            catch (Exception) { /* ignore reflection failures */ }

            Trace.AutoFlush = true;
        }
        catch { }

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        base.OnStartup(e);
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
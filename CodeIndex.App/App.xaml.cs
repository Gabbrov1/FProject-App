using System.Windows;

namespace CodeIndex.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Catch unhandled exceptions on the UI thread
            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"UI Error:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}", "Crash");
                args.Handled = true; // prevents close
            };

            // Catch unhandled exceptions on background threads
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MessageBox.Show($"Fatal Error:\n{(args.ExceptionObject as Exception)?.Message}", "Crash");
            };

            // Catch unhandled exceptions in async tasks
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                MessageBox.Show($"Async Error:\n{args.Exception.Message}", "Crash");
                args.SetObserved();
            };
        }
    }
}
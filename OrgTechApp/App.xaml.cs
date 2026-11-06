using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OrgTechApp
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static void WriteLog(Exception exception)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "error.log");
                File.AppendAllText(path,
                    "==== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ====" + Environment.NewLine +
                    exception + Environment.NewLine + Environment.NewLine);
            }
            catch
            {
                // Логирование не должно скрывать исходную ошибку.
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            WriteLog(e.Exception);
            MessageBox.Show(
                "Ошибка приложения. Подробности записаны в error.log рядом с exe.\n\n" + e.Exception.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteLog(e.ExceptionObject as Exception ?? new Exception("Неизвестная ошибка"));
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            WriteLog(e.Exception);
            e.SetObserved();
        }
    }
}

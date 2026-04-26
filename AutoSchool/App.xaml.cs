using AutoSchool.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Threading;

namespace AutoSchool
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                using var db = new ApplicationDbContext();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка инициализации БД:\n\n" + ex,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Unhandled exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject?.ToString() ?? "Unknown error", "Fatal error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
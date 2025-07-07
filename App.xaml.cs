using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Configuration;
using QuitSmartApp.ViewModels;
using QuitSmartApp.Models;
using System;
using System.Windows;

namespace QuitSmartApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Create service collection and configure services
                var serviceCollection = new ServiceCollection();

                // Configure application services
                serviceCollection.ConfigureServices(AppSettings.DefaultConnectionString);

                // Register MainWindow and MainViewModel
                serviceCollection.AddTransient<MainWindow>();

                // Build service provider
                ServiceProvider = serviceCollection.BuildServiceProvider();

                // Start the main window with MainViewModel
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error: {ex.Message}\n\nDetails: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}",
                    "Application Startup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Dispose service provider
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Configuration;
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
            // Create service collection and configure services
            var serviceCollection = new ServiceCollection();
            
            // Configure application services
            serviceCollection.ConfigureServices(AppSettings.DefaultConnectionString);
            
            // Register MainWindow
            serviceCollection.AddTransient<MainWindow>();

            // Build service provider
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Start the main window
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
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

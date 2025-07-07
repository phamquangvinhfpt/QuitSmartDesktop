using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Configuration;
using QuitSmartApp.ViewModels;
using QuitSmartApp.Models;
using System;
using System.Linq;
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

                // Initialize database
                using (var scope = ServiceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<QuitSmartDesktopContext>();
                    context.Database.EnsureCreated();
                    
                    // Create default admin if not exists
                    try
                    {
                        if (!context.Admins.Any())
                        {
                            CreateDefaultAdmin(scope);
                        }
                    }
                    catch
                    {
                        // Ignore errors during admin creation
                    }
                }

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

        private void CreateDefaultAdmin(IServiceScope scope)
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<QuitSmartDesktopContext>();
                
                // Check if any admin exists
                if (!context.Admins.Any())
                {
                    var defaultAdmin = new Models.Admin
                    {
                        AdminId = Guid.NewGuid(),
                        Username = "admin",
                        Email = "admin@quitsmart.com",
                        PasswordHash = Helpers.PasswordHelper.HashPassword("admin123"),
                        FullName = "Administrator",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    context.Admins.Add(defaultAdmin);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                System.Diagnostics.Debug.WriteLine($"Error creating default admin: {ex.Message}");
            }
        }
    }
}

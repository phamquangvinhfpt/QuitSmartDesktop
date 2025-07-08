using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services;
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.ViewModels;
using System;

namespace QuitSmartApp.Configuration
{
    // Dependency injection configuration for the application
    public static class ServiceConfiguration
    {
        // Configure all application services and repositories
        public static IServiceCollection ConfigureServices(this IServiceCollection services, string connectionString)
        {
            // Database context
            services.AddDbContext<QuitSmartDesktopContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IDailyLogRepository, DailyLogRepository>();
            services.AddScoped<IUserStatisticRepository, UserStatisticRepository>();
            services.AddScoped<IBadgeDefinitionRepository, BadgeDefinitionRepository>();
            services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
            services.AddScoped<IMotivationalMessageRepository, MotivationalMessageRepository>();
            services.AddScoped<IHealthInfoRepository, HealthInfoRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAdminLogRepository, AdminLogRepository>();

            // Services - Note: Order matters due to dependencies
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMotivationalService, MotivationalService>();
            services.AddScoped<IAdminService, AdminService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<UserDashboardViewModel>();
            services.AddTransient<GuestViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<DailyTrackingViewModel>();
            services.AddTransient<BadgeCollectionViewModel>();
            services.AddTransient<HealthInfoViewModel>();
            services.AddTransient<AdminDashboardViewModel>();
            services.AddTransient<RegisterViewModel>();

            return services;
        }
    }
}

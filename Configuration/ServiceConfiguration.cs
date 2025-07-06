using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services;
using QuitSmartApp.Services.Interfaces;
using System;

namespace QuitSmartApp.Configuration
{
    /// <summary>
    /// Dependency injection configuration for the application
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configure all application services and repositories
        /// </summary>
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
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMotivationalService, MotivationalService>();
            services.AddScoped<IAdminService, AdminService>();

            return services;
        }
    }
}

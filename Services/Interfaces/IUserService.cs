using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Services.Interfaces
{
    // User service interface for user-related business operations
    public interface IUserService
    {
        // User management
        Task<User?> GetUserAsync(Guid userId);
        Task UpdateUserAsync(User user);

        // User profile management
        Task<UserProfile?> GetUserProfileAsync(Guid userId);
        Task<UserProfile> CreateOrUpdateProfileAsync(Guid userId, UserProfile profile);

        // User statistics
        Task<UserStatistic?> GetUserStatisticsAsync(Guid userId);
        Task RefreshUserStatisticsAsync(Guid userId);

        // Daily tracking
        Task<DailyLog> LogDailyStatusAsync(Guid userId, DateOnly date, bool hasSmoked, string? healthStatus, string? notes);
        Task<IEnumerable<DailyLog>> GetUserDailyLogsAsync(Guid userId, int? days = null);
        Task<DailyLog?> GetTodayLogAsync(Guid userId);

        // Dashboard data
        Task<object> GetUserDashboardDataAsync(Guid userId);
    }
}

using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Services.Interfaces
{
    // Admin service interface for administrative operations
    public interface IAdminService
    {
        // User management
        Task<IEnumerable<UserOverview>> GetAllUsersOverviewAsync();
        Task<User?> GetUserDetailsAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<User> UpdateUserAsync(User user);
        
        // User logs and tracking
        Task<IEnumerable<DailyLog>> GetUserDailyLogsAsync(Guid userId);
        Task<IEnumerable<HealthTrackingOverview>> GetHealthTrackingOverviewAsync();
        
        // Admin logging
        Task LogAdminActionAsync(Guid adminId, string action, Guid? targetUserId = null, string? details = null);
        Task<IEnumerable<AdminLog>> GetAdminLogsAsync(Guid? adminId = null);
        
        // Dashboard data
        Task<object> GetAdminDashboardDataAsync();
    }
}

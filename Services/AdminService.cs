using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    /// <summary>
    /// Admin service implementation for administrative operations
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IAdminLogRepository _adminLogRepository;

        public AdminService(
            IUserRepository userRepository,
            IDailyLogRepository dailyLogRepository,
            IAdminLogRepository adminLogRepository)
        {
            _userRepository = userRepository;
            _dailyLogRepository = dailyLogRepository;
            _adminLogRepository = adminLogRepository;
        }

        public async Task<IEnumerable<UserOverview>> GetAllUsersOverviewAsync()
        {
            // This would require implementing UserOverview repository or using DbContext directly
            // For now, return active users as basic overview
            var users = await _userRepository.GetActiveUsersAsync();
            return users.Select(u => new UserOverview
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Gender = u.Gender,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<User?> GetUserDetailsAsync(Guid userId)
        {
            return await _userRepository.GetUserWithProfileAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var result = await _userRepository.DeleteByIdAsync(userId);
            if (result)
            {
                await _userRepository.SaveChangesAsync();
            }
            return result;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<DailyLog>> GetUserDailyLogsAsync(Guid userId)
        {
            return await _dailyLogRepository.GetUserLogsAsync(userId);
        }

        public async Task<IEnumerable<HealthTrackingOverview>> GetHealthTrackingOverviewAsync()
        {
            // This would require implementing HealthTrackingOverview repository
            // For now, return empty collection
            return new List<HealthTrackingOverview>();
        }

        public async Task LogAdminActionAsync(Guid adminId, string action, Guid? targetUserId = null, string? details = null)
        {
            await _adminLogRepository.LogActionAsync(adminId, action, targetUserId, details);
        }

        public async Task<IEnumerable<AdminLog>> GetAdminLogsAsync(Guid? adminId = null)
        {
            return await _adminLogRepository.GetAdminLogsAsync(adminId, 100); // Limit to 100 recent logs
        }

        public async Task<object> GetAdminDashboardDataAsync()
        {
            var totalUsers = await _userRepository.CountAsync(u => u.IsActive == true);
            var recentActivity = await _adminLogRepository.GetRecentActivityAsync(7);
            var activeUsers = await _userRepository.GetActiveUsersAsync();

            return new
            {
                TotalUsers = totalUsers,
                ActiveUsersCount = activeUsers.Count(),
                RecentActivityCount = recentActivity.Count(),
                RecentUsers = activeUsers.OrderByDescending(u => u.CreatedAt).Take(5)
            };
        }
    }
}

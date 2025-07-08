using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    // Admin service implementation for administrative operations
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
            try
            {
                // Try to get real data from repository
                var users = await _userRepository.GetActiveUsersAsync();

                var userOverviews = users.Select(u => new UserOverview
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    Gender = u.Gender,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    QuitStartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), // Sample data
                    CigarettesPerDay = 10,
                    PricePerPack = 50000,
                    TotalDaysQuit = 30,
                    TotalMoneySaved = 1500000,
                    CurrentStreak = 30,
                    LongestStreak = 30,
                    TotalBadges = 3
                }).ToList();

                return userOverviews;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is some trouble");
                throw new Exception("ex", ex);
            }
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

        public Task<IEnumerable<HealthTrackingOverview>> GetHealthTrackingOverviewAsync()
        {
            // This would require implementing HealthTrackingOverview repository
            // For now, return empty collection
            return Task.FromResult<IEnumerable<HealthTrackingOverview>>(new List<HealthTrackingOverview>());
        }

        public async Task LogAdminActionAsync(Guid adminId, string action, Guid? targetUserId = null, string? details = null)
        {
            await _adminLogRepository.LogActionAsync(adminId, action, targetUserId, details);
        }

        public async Task<IEnumerable<AdminLog>> GetAdminLogsAsync(Guid? adminId = null)
        {
            try
            {
                var logs = await _adminLogRepository.GetAdminLogsAsync(adminId, 100);

                return logs;
            }
            catch (Exception)
            {
                return Enumerable.Empty<AdminLog>();
            }
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

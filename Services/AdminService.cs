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
                var users = await _userRepository.GetActiveUsersAsync();
                var userOverviews = new List<UserOverview>();

                foreach (var user in users)
                {
                    var userWithDetails = await _userRepository.GetUserWithProfileAsync(user.UserId);
                    if (userWithDetails == null) continue;

                    var latestProfile = userWithDetails.UserProfiles?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

                    var stats = userWithDetails.UserStatistic;

                    var totalBadges = userWithDetails.UserBadges?.Count ?? 0;

                    var quitStartDate = latestProfile?.QuitStartDate;
                    var totalDaysQuit = stats?.TotalDaysQuit;

                    if (quitStartDate.HasValue && !totalDaysQuit.HasValue)
                    {
                        var daysDiff = DateOnly.FromDateTime(DateTime.Now).DayNumber - quitStartDate.Value.DayNumber;
                        totalDaysQuit = Math.Max(0, daysDiff);
                    }

                    decimal? totalMoneySaved = stats?.TotalMoneySaved;
                    if (!totalMoneySaved.HasValue && latestProfile != null && totalDaysQuit.HasValue)
                    {
                        var cigarettesPerDay = latestProfile.CigarettesPerDay;
                        var pricePerPack = latestProfile.PricePerPack;
                        var cigarettesPerPack = latestProfile.CigarettesPerPack ?? 20;

                        if (cigarettesPerDay > 0 && pricePerPack > 0)
                        {
                            var dailyCost = (decimal)cigarettesPerDay / cigarettesPerPack * pricePerPack;
                            totalMoneySaved = dailyCost * totalDaysQuit.Value;
                        }
                    }

                    var userOverview = new UserOverview
                    {
                        UserId = userWithDetails.UserId,
                        Username = userWithDetails.Username,
                        Email = userWithDetails.Email,
                        FullName = userWithDetails.FullName,
                        Gender = userWithDetails.Gender,
                        IsActive = userWithDetails.IsActive,
                        CreatedAt = userWithDetails.CreatedAt ?? DateTime.Now,

                        QuitStartDate = quitStartDate,
                        CigarettesPerDay = latestProfile?.CigarettesPerDay,
                        PricePerPack = latestProfile?.PricePerPack,
                        QuitReason = latestProfile?.QuitReason,

                        TotalDaysQuit = totalDaysQuit,
                        TotalMoneySaved = totalMoneySaved,
                        CurrentStreak = stats?.CurrentStreak,
                        LongestStreak = stats?.LongestStreak,

                        TotalBadges = totalBadges
                    };

                    userOverviews.Add(userOverview);
                }

                return userOverviews;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllUsersOverviewAsync: {ex.Message}");
                throw new Exception("Failed to get users overview", ex);
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

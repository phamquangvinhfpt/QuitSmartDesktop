using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IDailyLogRepository _dailyLogRepository;
        private readonly IUserStatisticRepository _userStatisticRepository;
        private readonly IBadgeService _badgeService;

        public UserService(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IDailyLogRepository dailyLogRepository,
            IUserStatisticRepository userStatisticRepository,
            IBadgeService badgeService)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _dailyLogRepository = dailyLogRepository;
            _userStatisticRepository = userStatisticRepository;
            _badgeService = badgeService;
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserProfile?> GetUserProfileAsync(Guid userId)
        {
            return await _userProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task<UserProfile> CreateOrUpdateProfileAsync(Guid userId, UserProfile profile)
        {
            profile.UserId = userId;
            var result = await _userProfileRepository.CreateOrUpdateAsync(profile);
            await _userProfileRepository.SaveChangesAsync();

            // Refresh statistics after profile update
            await RefreshUserStatisticsAsync(userId);

            return result;
        }

        public async Task<UserStatistic?> GetUserStatisticsAsync(Guid userId)
        {
            return await _userStatisticRepository.GetByUserIdAsync(userId);
        }

        public async Task RefreshUserStatisticsAsync(Guid userId)
        {
            await _userStatisticRepository.RefreshUserStatisticsAsync(userId);
        }

        public async Task<DailyLog> LogDailyStatusAsync(Guid userId, DateOnly date, bool hasSmoked, string? healthStatus, string? notes)
        {
            var result = await _dailyLogRepository.CreateOrUpdateDailyLogAsync(userId, date, hasSmoked, healthStatus, notes);
            await _dailyLogRepository.SaveChangesAsync();

            // Refresh statistics after logging
            await RefreshUserStatisticsAsync(userId);

            // Check for new badges
            await _badgeService.CheckAndAwardBadgesAsync(userId);

            return result;
        }

        public async Task<IEnumerable<DailyLog>> GetUserDailyLogsAsync(Guid userId, int? days = null)
        {
            return await _dailyLogRepository.GetUserLogsAsync(userId, days);
        }

        public async Task<DailyLog?> GetTodayLogAsync(Guid userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _dailyLogRepository.GetByUserAndDateAsync(userId, today);
        }

        public async Task<object> GetUserDashboardDataAsync(Guid userId)
        {
            var profile = await GetUserProfileAsync(userId);
            var statistics = await GetUserStatisticsAsync(userId);
            var recentLogs = await GetUserDailyLogsAsync(userId, 7);
            var todayLog = await GetTodayLogAsync(userId);
            var badges = await _badgeService.GetUserBadgeCollectionAsync(userId);

            return new
            {
                Profile = profile,
                Statistics = statistics,
                RecentLogs = recentLogs,
                TodayLog = todayLog,
                BadgeCount = badges.Count(),
                HasLoggedToday = todayLog != null
            };
        }
    }
}

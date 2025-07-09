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

            _userStatisticRepository.ClearChangeTracker();
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

        public async Task<object> GetDetailedUserStatisticsAsync(Guid userId)
        {
            var profile = await GetUserProfileAsync(userId);
            var statistics = await GetUserStatisticsAsync(userId);
            var allLogs = await GetUserDailyLogsAsync(userId);
            var badges = await _badgeService.GetUserBadgeCollectionAsync(userId);

            var logsList = allLogs.ToList();
            var successfulDays = logsList.Count(l => l.HasSmoked == false);
            var failedDays = logsList.Count(l => l.HasSmoked == true);
            var totalDaysTracked = logsList.Count;

            // Weekly statistics
            var weeklyStats = logsList
                .GroupBy(l => GetWeekOfYear(l.LogDate.ToDateTime(TimeOnly.MinValue)))
                .OrderByDescending(g => g.Key)
                .Take(12)
                .Select(g => new
                {
                    Week = g.Key,
                    SuccessfulDays = g.Count(l => l.HasSmoked == false),
                    TotalDays = g.Count(),
                    SuccessRate = g.Count() > 0 ? (double)g.Count(l => l.HasSmoked == false) / g.Count() * 100 : 0
                })
                .ToList();

            // Monthly money saved
            decimal dailyCost = 0;
            if (profile != null)
            {
                var cigarettesPerDay = profile.CigarettesPerDay;
                var pricePerPack = profile.PricePerPack;
                var cigarettesPerPack = profile.CigarettesPerPack ?? 20;

                if (cigarettesPerDay > 0 && pricePerPack > 0 && cigarettesPerPack > 0)
                {
                    dailyCost = (decimal)cigarettesPerDay / cigarettesPerPack * pricePerPack;
                }
            }

            var monthlyMoneySaved = logsList
                .GroupBy(l => new { l.LogDate.Year, l.LogDate.Month })
                .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
                .Take(6)
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MoneySaved = g.Count(l => l.HasSmoked == false) * dailyCost
                })
                .ToList();

            return new
            {
                Profile = profile,
                Statistics = statistics,
                TotalDaysTracked = totalDaysTracked,
                SuccessfulDays = successfulDays,
                FailedDays = failedDays,
                OverallSuccessRate = totalDaysTracked > 0 ? (double)successfulDays / totalDaysTracked * 100 : 0,
                WeeklyStats = weeklyStats,
                MonthlyMoneySaved = monthlyMoneySaved,
                RecentLogs = logsList.OrderByDescending(l => l.LogDate).Take(30),
                TotalBadges = badges.Count(),
                DailyCost = dailyCost
            };
        }

        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }
    }
}

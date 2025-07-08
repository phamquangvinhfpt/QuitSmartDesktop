using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for DailyLog entity operations
    public class DailyLogRepository : BaseRepository<DailyLog>, IDailyLogRepository
    {
        public DailyLogRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<DailyLog?> GetByUserAndDateAsync(Guid userId, DateOnly date)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.UserId == userId && d.LogDate == date);
        }

        public async Task<IEnumerable<DailyLog>> GetUserLogsAsync(Guid userId, int? days = null)
        {
            IQueryable<DailyLog> query = _dbSet.Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LogDate);

            if (days.HasValue)
                query = query.Take(days.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<DailyLog>> GetUserLogsInRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && d.LogDate >= startDate && d.LogDate <= endDate)
                .OrderBy(d => d.LogDate)
                .ToListAsync();
        }

        public async Task<int> GetNoSmokingDaysCountAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(d => d.UserId == userId && d.HasSmoked == false);
        }

        public async Task<int> GetCurrentStreakAsync(Guid userId)
        {
            var recentLogs = await _dbSet
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.LogDate)
                .ToListAsync();

            int streak = 0;
            foreach (var log in recentLogs)
            {
                if (log.HasSmoked == false)
                    streak++;
                else
                    break;
            }

            return streak;
        }

        public async Task<DailyLog> CreateOrUpdateDailyLogAsync(Guid userId, DateOnly date, bool hasSmoked, string? healthStatus, string? notes)
        {
            var existingLog = await GetByUserAndDateAsync(userId, date);

            if (existingLog != null)
            {
                // Update existing log
                existingLog.HasSmoked = hasSmoked;
                existingLog.HealthStatus = healthStatus;
                existingLog.Notes = notes;
                existingLog.UpdatedAt = DateTime.UtcNow;

                return await UpdateAsync(existingLog);
            }
            else
            {
                // Create new log
                var newLog = new DailyLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = userId,
                    LogDate = date,
                    HasSmoked = hasSmoked,
                    HealthStatus = healthStatus,
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return await AddAsync(newLog);
            }
        }
    }
}

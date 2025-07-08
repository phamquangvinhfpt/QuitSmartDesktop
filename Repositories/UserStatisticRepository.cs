using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for UserStatistic entity operations
    public class UserStatisticRepository : BaseRepository<UserStatistic>, IUserStatisticRepository
    {
        public UserStatisticRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<UserStatistic?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<UserStatistic> CreateOrUpdateAsync(UserStatistic statistic)
        {
            var existing = await GetByUserIdAsync(statistic.UserId);
            
            if (existing != null)
            {
                existing.TotalDaysQuit = statistic.TotalDaysQuit;
                existing.TotalMoneySaved = statistic.TotalMoneySaved;
                existing.CurrentStreak = statistic.CurrentStreak;
                existing.LongestStreak = statistic.LongestStreak;
                existing.LastCalculatedAt = DateTime.UtcNow;
                existing.UpdatedAt = DateTime.UtcNow;
                
                return await UpdateAsync(existing);
            }
            else
            {
                statistic.StatId = Guid.NewGuid();
                statistic.LastCalculatedAt = DateTime.UtcNow;
                statistic.UpdatedAt = DateTime.UtcNow;
                
                return await AddAsync(statistic);
            }
        }

        public async Task RefreshUserStatisticsAsync(Guid userId)
        {
            // Execute stored procedure to recalculate statistics
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [CalculateUserStatistics] @UserId = {0}", userId);
        }
    }
}

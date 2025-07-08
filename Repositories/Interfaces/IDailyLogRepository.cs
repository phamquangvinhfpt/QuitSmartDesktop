using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    /// Repository interface for DailyLog entity operations
    public interface IDailyLogRepository : IBaseRepository<DailyLog>
    {
        Task<DailyLog?> GetByUserAndDateAsync(Guid userId, DateOnly date);
        Task<IEnumerable<DailyLog>> GetUserLogsAsync(Guid userId, int? days = null);
        Task<IEnumerable<DailyLog>> GetUserLogsInRangeAsync(Guid userId, DateOnly startDate, DateOnly endDate);
        Task<int> GetNoSmokingDaysCountAsync(Guid userId);
        Task<int> GetCurrentStreakAsync(Guid userId);
        Task<DailyLog> CreateOrUpdateDailyLogAsync(Guid userId, DateOnly date, bool hasSmoked, string? healthStatus, string? notes);
    }
}

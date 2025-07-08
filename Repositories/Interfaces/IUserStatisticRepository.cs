using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for UserStatistic entity operations
    public interface IUserStatisticRepository : IBaseRepository<UserStatistic>
    {
        Task<UserStatistic?> GetByUserIdAsync(Guid userId);
        Task<UserStatistic> CreateOrUpdateAsync(UserStatistic statistic);
        Task RefreshUserStatisticsAsync(Guid userId);
    }
}

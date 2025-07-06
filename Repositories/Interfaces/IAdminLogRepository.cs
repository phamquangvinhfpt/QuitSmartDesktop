using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for AdminLog entity operations
    /// </summary>
    public interface IAdminLogRepository : IBaseRepository<AdminLog>
    {
        Task<AdminLog> LogActionAsync(Guid adminId, string action, Guid? targetUserId = null, string? details = null);
        Task<IEnumerable<AdminLog>> GetAdminLogsAsync(Guid? adminId = null, int? limit = null);
        Task<IEnumerable<AdminLog>> GetRecentActivityAsync(int days = 30);
    }
}

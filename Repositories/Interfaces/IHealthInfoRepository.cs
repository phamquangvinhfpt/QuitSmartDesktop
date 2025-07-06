using QuitSmartApp.Models;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for HealthInfo entity operations
    /// </summary>
    public interface IHealthInfoRepository : IBaseRepository<HealthInfo>
    {
        Task<IEnumerable<HealthInfo>> GetActiveHealthInfoAsync();
        Task<IEnumerable<HealthInfo>> GetHealthInfoByTypeAsync(string infoType);
        Task<HealthInfo?> GetRandomHealthInfoAsync(string? infoType = null);
    }
}

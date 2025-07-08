using QuitSmartApp.Models;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for HealthInfo entity operations
    public interface IHealthInfoRepository : IBaseRepository<HealthInfo>
    {
        Task<IEnumerable<HealthInfo>> GetActiveHealthInfoAsync();
        Task<IEnumerable<HealthInfo>> GetHealthInfoByTypeAsync(string infoType);
        Task<HealthInfo?> GetRandomHealthInfoAsync(string? infoType = null);
    }
}

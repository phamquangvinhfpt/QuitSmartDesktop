using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Services.Interfaces
{
    public interface IMotivationalService
    {
        Task<MotivationalMessage?> GetDailyMotivationAsync();
        Task<MotivationalMessage?> GetPersonalizedMessageAsync(Guid userId);
        Task<IEnumerable<MotivationalMessage>> GetEncouragementMessagesAsync();
        Task<IEnumerable<HealthInfo>> GetHealthInfoAsync(string? infoType = null);
        Task<HealthInfo?> GetRandomHealthTipAsync();
    }
}

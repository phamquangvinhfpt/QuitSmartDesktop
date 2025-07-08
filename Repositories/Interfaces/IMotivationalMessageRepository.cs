using QuitSmartApp.Models;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for MotivationalMessage entity operations
    public interface IMotivationalMessageRepository : IBaseRepository<MotivationalMessage>
    {
        Task<MotivationalMessage?> GetRandomDailyMessageAsync();
        Task<MotivationalMessage?> GetRandomMessageByTypeAsync(string messageType);
        Task<IEnumerable<MotivationalMessage>> GetActiveMessagesAsync();
        Task<IEnumerable<MotivationalMessage>> GetMessagesByTypeAsync(string messageType);
    }
}

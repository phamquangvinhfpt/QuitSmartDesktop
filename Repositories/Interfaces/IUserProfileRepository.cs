using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for UserProfile entity operations
    public interface IUserProfileRepository : IBaseRepository<UserProfile>
    {
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task<UserProfile> CreateOrUpdateAsync(UserProfile profile);
        Task<bool> HasProfileAsync(Guid userId);
    }
}

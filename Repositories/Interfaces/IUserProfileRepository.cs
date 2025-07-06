using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for UserProfile entity operations
    /// </summary>
    public interface IUserProfileRepository : IBaseRepository<UserProfile>
    {
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task<UserProfile> CreateOrUpdateAsync(UserProfile profile);
        Task<bool> HasProfileAsync(Guid userId);
    }
}

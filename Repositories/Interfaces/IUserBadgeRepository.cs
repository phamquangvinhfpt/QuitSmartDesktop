using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for UserBadge entity operations
    public interface IUserBadgeRepository : IBaseRepository<UserBadge>
    {
        Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId);
        Task<IEnumerable<UserBadgeCollection>> GetUserBadgeCollectionAsync(Guid userId);
        Task<bool> HasUserEarnedBadgeAsync(Guid userId, Guid badgeId);
        Task<UserBadge> AwardBadgeAsync(Guid userId, Guid badgeId);
        Task<IEnumerable<UserBadge>> GetUnnotifiedBadgesAsync(Guid userId);
        Task MarkBadgesAsNotifiedAsync(IEnumerable<Guid> userBadgeIds);
    }
}

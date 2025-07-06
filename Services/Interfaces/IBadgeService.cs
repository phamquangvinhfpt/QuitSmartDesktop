using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Services.Interfaces
{
    /// <summary>
    /// Badge service interface for achievement system operations
    /// </summary>
    public interface IBadgeService
    {
        Task<IEnumerable<UserBadgeCollection>> GetUserBadgeCollectionAsync(Guid userId);
        Task<IEnumerable<BadgeDefinition>> GetAllAvailableBadgesAsync();
        Task CheckAndAwardBadgesAsync(Guid userId);
        Task<IEnumerable<UserBadge>> GetNewlyEarnedBadgesAsync(Guid userId);
        Task MarkBadgesAsSeenAsync(Guid userId, IEnumerable<Guid> badgeIds);
    }
}

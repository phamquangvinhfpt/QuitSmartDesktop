using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    // Repository interface for BadgeDefinition entity operations
    public interface IBadgeDefinitionRepository : IBaseRepository<BadgeDefinition>
    {
        Task<IEnumerable<BadgeDefinition>> GetActiveBadgesAsync();
        Task<IEnumerable<BadgeDefinition>> GetBadgesByTypeAsync(string badgeType);
    }
}

using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for BadgeDefinition entity operations
    public class BadgeDefinitionRepository : BaseRepository<BadgeDefinition>, IBadgeDefinitionRepository
    {
        public BadgeDefinitionRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BadgeDefinition>> GetActiveBadgesAsync()
        {
            return await _dbSet
                .Where(b => b.IsActive == true)
                .OrderBy(b => b.BadgeType)
                .ThenBy(b => b.RequiredValue)
                .ToListAsync();
        }

        public async Task<IEnumerable<BadgeDefinition>> GetBadgesByTypeAsync(string badgeType)
        {
            return await _dbSet
                .Where(b => b.BadgeType == badgeType && b.IsActive == true)
                .OrderBy(b => b.RequiredValue)
                .ToListAsync();
        }
    }
}

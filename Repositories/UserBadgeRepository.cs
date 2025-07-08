using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for UserBadge entity operations
    public class UserBadgeRepository : BaseRepository<UserBadge>, IUserBadgeRepository
    {
        public UserBadgeRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserBadge>> GetUserBadgesAsync(Guid userId)
        {
            return await _dbSet
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserId == userId)
                .OrderByDescending(ub => ub.EarnedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserBadgeCollection>> GetUserBadgeCollectionAsync(Guid userId)
        {
            return await _context.UserBadgeCollections
                .Where(ubc => ubc.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> HasUserEarnedBadgeAsync(Guid userId, Guid badgeId)
        {
            return await _dbSet.AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
        }

        public async Task<UserBadge> AwardBadgeAsync(Guid userId, Guid badgeId)
        {
            // Check if badge already earned
            if (await HasUserEarnedBadgeAsync(userId, badgeId))
                throw new InvalidOperationException("User has already earned this badge");

            var userBadge = new UserBadge
            {
                UserBadgeId = Guid.NewGuid(),
                UserId = userId,
                BadgeId = badgeId,
                EarnedAt = DateTime.UtcNow,
                IsNotified = false
            };

            return await AddAsync(userBadge);
        }

        public async Task<IEnumerable<UserBadge>> GetUnnotifiedBadgesAsync(Guid userId)
        {
            return await _dbSet
                .Include(ub => ub.Badge)
                .Where(ub => ub.UserId == userId && ub.IsNotified == false)
                .ToListAsync();
        }

        public async Task MarkBadgesAsNotifiedAsync(IEnumerable<Guid> userBadgeIds)
        {
            var badges = await _dbSet
                .Where(ub => userBadgeIds.Contains(ub.UserBadgeId))
                .ToListAsync();

            foreach (var badge in badges)
            {
                badge.IsNotified = true;
            }

            await SaveChangesAsync();
        }
    }
}

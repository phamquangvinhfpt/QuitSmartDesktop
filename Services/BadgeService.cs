using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    /// <summary>
    /// Badge service implementation for achievement system operations
    /// </summary>
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeDefinitionRepository _badgeDefinitionRepository;
        private readonly IUserBadgeRepository _userBadgeRepository;
        private readonly IUserStatisticRepository _userStatisticRepository;

        public BadgeService(
            IBadgeDefinitionRepository badgeDefinitionRepository,
            IUserBadgeRepository userBadgeRepository,
            IUserStatisticRepository userStatisticRepository)
        {
            _badgeDefinitionRepository = badgeDefinitionRepository;
            _userBadgeRepository = userBadgeRepository;
            _userStatisticRepository = userStatisticRepository;
        }

        public async Task<IEnumerable<UserBadgeCollection>> GetUserBadgeCollectionAsync(Guid userId)
        {
            return await _userBadgeRepository.GetUserBadgeCollectionAsync(userId);
        }

        public async Task<IEnumerable<BadgeDefinition>> GetAllAvailableBadgesAsync()
        {
            return await _badgeDefinitionRepository.GetActiveBadgesAsync();
        }

        public async Task CheckAndAwardBadgesAsync(Guid userId)
        {
            var statistics = await _userStatisticRepository.GetByUserIdAsync(userId);
            if (statistics == null)
                return;

            var allBadges = await _badgeDefinitionRepository.GetActiveBadgesAsync();

            foreach (var badge in allBadges)
            {
                // Skip if user already has this badge
                if (await _userBadgeRepository.HasUserEarnedBadgeAsync(userId, badge.BadgeId))
                    continue;

                bool shouldAward = badge.BadgeType switch
                {
                    "Days" => statistics.TotalDaysQuit >= (int)badge.RequiredValue,
                    "Money" => statistics.TotalMoneySaved >= badge.RequiredValue,
                    "Streak" => statistics.CurrentStreak >= (int)badge.RequiredValue,
                    _ => false
                };

                if (shouldAward)
                {
                    await _userBadgeRepository.AwardBadgeAsync(userId, badge.BadgeId);
                }
            }

            await _userBadgeRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserBadge>> GetNewlyEarnedBadgesAsync(Guid userId)
        {
            return await _userBadgeRepository.GetUnnotifiedBadgesAsync(userId);
        }

        public async Task MarkBadgesAsSeenAsync(Guid userId, IEnumerable<Guid> badgeIds)
        {
            await _userBadgeRepository.MarkBadgesAsNotifiedAsync(badgeIds);
        }
    }
}

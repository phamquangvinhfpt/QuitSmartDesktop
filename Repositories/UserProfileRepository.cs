using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for UserProfile entity operations
    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile> CreateOrUpdateAsync(UserProfile profile)
        {
            var existingProfile = await GetByUserIdAsync(profile.UserId);
            
            if (existingProfile != null)
            {
                // Update existing profile
                existingProfile.QuitStartDate = profile.QuitStartDate;
                existingProfile.QuitGoalDate = profile.QuitGoalDate;
                existingProfile.CigarettesPerDay = profile.CigarettesPerDay;
                existingProfile.PricePerPack = profile.PricePerPack;
                existingProfile.CigarettesPerPack = profile.CigarettesPerPack;
                existingProfile.SmokingYears = profile.SmokingYears;
                existingProfile.QuitReason = profile.QuitReason;
                existingProfile.UpdatedAt = DateTime.UtcNow;
                
                return await UpdateAsync(existingProfile);
            }
            else
            {
                // Create new profile
                profile.ProfileId = Guid.NewGuid();
                profile.CreatedAt = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;
                
                return await AddAsync(profile);
            }
        }

        public async Task<bool> HasProfileAsync(Guid userId)
        {
            return await _dbSet.AnyAsync(p => p.UserId == userId);
        }
    }
}

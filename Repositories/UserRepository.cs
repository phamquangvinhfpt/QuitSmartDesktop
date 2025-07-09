using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for User entity operations
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive == true);
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return null;

            // Use PasswordHelper to verify password against hash
            return PasswordHelper.VerifyPassword(password, user.PasswordHash) ? user : null;
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithProfileAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.UserProfiles)
                .Include(u => u.UserStatistic)
                .Include(u => u.UserBadges)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);
        }

        public async Task<IEnumerable<User>> GetUsersWithDetailsAsync()
        {
            return await _dbSet
                .Include(u => u.UserProfiles)
                .Include(u => u.UserStatistic)
                .Include(u => u.UserBadges)
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}

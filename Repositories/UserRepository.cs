using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    /// <summary>
    /// Repository implementation for User entity operations
    /// </summary>
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
        {            // NOTE: In production, use proper password hashing verification
            var user = await GetByUsernameAsync(username);
            return user?.PasswordHash == password ? user : null;
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
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);
        }
    }
}

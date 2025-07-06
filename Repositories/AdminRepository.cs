using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Helpers;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    /// <summary>
    /// Repository implementation for Admin entity operations
    /// </summary>
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive == true);
        }

        public async Task<Admin?> ValidateAdminAsync(string username, string password)
        {
            var admin = await GetByUsernameAsync(username);
            
            if (admin == null)
                return null;
                
            // Use PasswordHelper for verification
            return PasswordHelper.VerifyPassword(password, admin.PasswordHash) ? admin : null;
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(a => a.Username == username);
        }

        public async Task UpdateLastLoginAsync(Guid adminId)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin != null)
            {
                admin.LastLoginAt = DateTime.UtcNow;
                admin.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(admin);
                await SaveChangesAsync();
            }
        }
    }
}

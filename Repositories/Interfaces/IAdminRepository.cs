using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Admin entity operations
    /// </summary>
    public interface IAdminRepository : IBaseRepository<Admin>
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin?> ValidateAdminAsync(string username, string password);
        Task<bool> IsUsernameExistsAsync(string username);
        Task UpdateLastLoginAsync(Guid adminId);
    }
}

using Microsoft.EntityFrameworkCore;
using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitSmartApp.Repositories
{
    // Repository implementation for AdminLog entity operations
    public class AdminLogRepository : BaseRepository<AdminLog>, IAdminLogRepository
    {
        public AdminLogRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<AdminLog> LogActionAsync(Guid adminId, string action, Guid? targetUserId = null, string? details = null)
        {
            var log = new AdminLog
            {
                LogId = Guid.NewGuid(),
                AdminId = adminId,
                Action = action,
                TargetUserId = targetUserId,
                Details = details,
                CreatedAt = DateTime.UtcNow
            };

            var result = await AddAsync(log);
            await SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<AdminLog>> GetAdminLogsAsync(Guid? adminId = null, int? limit = null)
        {
            var query = _dbSet.AsQueryable();
            
            if (adminId.HasValue)
                query = query.Where(l => l.AdminId == adminId.Value);
                
            query = query.OrderByDescending(l => l.CreatedAt);
            
            if (limit.HasValue)
                query = query.Take(limit.Value);
                
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<AdminLog>> GetRecentActivityAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return await _dbSet
                .Where(l => l.CreatedAt >= cutoffDate)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}

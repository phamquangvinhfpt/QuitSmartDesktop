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
    /// Repository implementation for HealthInfo entity operations
    /// </summary>
    public class HealthInfoRepository : BaseRepository<HealthInfo>, IHealthInfoRepository
    {
        public HealthInfoRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HealthInfo>> GetActiveHealthInfoAsync()
        {
            return await _dbSet
                .Where(h => h.IsActive == true)
                .OrderBy(h => h.InfoType)
                .ThenBy(h => h.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<HealthInfo>> GetHealthInfoByTypeAsync(string infoType)
        {
            return await _dbSet
                .Where(h => h.InfoType == infoType && h.IsActive == true)
                .OrderBy(h => h.Title)
                .ToListAsync();
        }

        public async Task<HealthInfo?> GetRandomHealthInfoAsync(string? infoType = null)
        {
            var query = _dbSet.Where(h => h.IsActive == true);
            
            if (!string.IsNullOrEmpty(infoType))
                query = query.Where(h => h.InfoType == infoType);
                
            var healthInfoList = await query.ToListAsync();
            
            if (!healthInfoList.Any())
                return null;
                
            var random = new Random();
            return healthInfoList[random.Next(healthInfoList.Count)];
        }
    }
}

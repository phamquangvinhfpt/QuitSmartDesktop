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
    /// Repository implementation for MotivationalMessage entity operations
    /// </summary>
    public class MotivationalMessageRepository : BaseRepository<MotivationalMessage>, IMotivationalMessageRepository
    {
        public MotivationalMessageRepository(QuitSmartDesktopContext context) : base(context)
        {
        }

        public async Task<MotivationalMessage?> GetRandomDailyMessageAsync()
        {
            var messages = await _dbSet
                .Where(m => m.MessageType == "Daily" && m.IsActive == true)
                .ToListAsync();
                
            if (!messages.Any())
                return null;
                
            var random = new Random();
            return messages[random.Next(messages.Count)];
        }

        public async Task<MotivationalMessage?> GetRandomMessageByTypeAsync(string messageType)
        {
            var messages = await _dbSet
                .Where(m => m.MessageType == messageType && m.IsActive == true)
                .ToListAsync();
                
            if (!messages.Any())
                return null;
                
            var random = new Random();
            return messages[random.Next(messages.Count)];
        }

        public async Task<IEnumerable<MotivationalMessage>> GetActiveMessagesAsync()
        {
            return await _dbSet
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.MessageType)
                .ThenBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<MotivationalMessage>> GetMessagesByTypeAsync(string messageType)
        {
            return await _dbSet
                .Where(m => m.MessageType == messageType && m.IsActive == true)
                .OrderBy(m => m.Title)
                .ToListAsync();
        }
    }
}

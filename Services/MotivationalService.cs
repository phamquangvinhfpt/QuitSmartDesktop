using QuitSmartApp.Models;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitSmartApp.Services
{
    public class MotivationalService : IMotivationalService
    {
        private readonly IMotivationalMessageRepository _motivationalMessageRepository;
        private readonly IHealthInfoRepository _healthInfoRepository;
        private readonly IUserStatisticRepository _userStatisticRepository;

        public MotivationalService(
            IMotivationalMessageRepository motivationalMessageRepository,
            IHealthInfoRepository healthInfoRepository,
            IUserStatisticRepository userStatisticRepository)
        {
            _motivationalMessageRepository = motivationalMessageRepository;
            _healthInfoRepository = healthInfoRepository;
            _userStatisticRepository = userStatisticRepository;
        }

        public async Task<MotivationalMessage?> GetDailyMotivationAsync()
        {
            return await _motivationalMessageRepository.GetRandomDailyMessageAsync();
        }

        public async Task<MotivationalMessage?> GetPersonalizedMessageAsync(Guid userId)
        {
            var statistics = await _userStatisticRepository.GetByUserIdAsync(userId);
            
            string messageType = statistics?.CurrentStreak switch
            {
                var s when s >= 30 => AppSettings.AchievementMessageType,
                var s when s >= 7 => AppSettings.EncouragementMessageType,
                _ => AppSettings.DailyMessageType
            };

            return await _motivationalMessageRepository.GetRandomMessageByTypeAsync(messageType);
        }

        public async Task<IEnumerable<MotivationalMessage>> GetEncouragementMessagesAsync()
        {
            return await _motivationalMessageRepository.GetMessagesByTypeAsync(AppSettings.EncouragementMessageType);
        }

        public async Task<IEnumerable<HealthInfo>> GetHealthInfoAsync(string? infoType = null)
        {
            if (string.IsNullOrEmpty(infoType))
                return await _healthInfoRepository.GetActiveHealthInfoAsync();
                
            return await _healthInfoRepository.GetHealthInfoByTypeAsync(infoType);
        }

        public async Task<HealthInfo?> GetRandomHealthTipAsync()
        {
            return await _healthInfoRepository.GetRandomHealthInfoAsync(AppSettings.TipsType);
        }
    }
}

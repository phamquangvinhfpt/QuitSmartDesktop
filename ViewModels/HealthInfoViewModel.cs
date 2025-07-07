using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace QuitSmartApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing health information content
    /// </summary>
    public class HealthInfoViewModel : BaseViewModel
    {
        private readonly IHealthInfoRepository _healthInfoRepository;

        private ObservableCollection<HealthInfo> _smokeEffects = new();
        private ObservableCollection<HealthInfo> _quitBenefits = new();
        private ObservableCollection<HealthInfo> _healthTips = new();
        private ObservableCollection<HealthTrackingOverview> _healthProgress = new();
        private bool _isLoading = true;

        public HealthInfoViewModel(IHealthInfoRepository healthInfoRepository)
        {
            _healthInfoRepository = healthInfoRepository;
            
            LoadHealthInfoAsync();
        }

        // Properties
        public ObservableCollection<HealthInfo> SmokeEffects
        {
            get => _smokeEffects;
            set => SetProperty(ref _smokeEffects, value);
        }

        public ObservableCollection<HealthInfo> QuitBenefits
        {
            get => _quitBenefits;
            set => SetProperty(ref _quitBenefits, value);
        }

        public ObservableCollection<HealthInfo> HealthTips
        {
            get => _healthTips;
            set => SetProperty(ref _healthTips, value);
        }

        public ObservableCollection<HealthTrackingOverview> HealthProgress
        {
            get => _healthProgress;
            set => SetProperty(ref _healthProgress, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Methods
        private async void LoadHealthInfoAsync()
        {
            try
            {
                IsLoading = true;

                var allHealthInfo = await _healthInfoRepository.GetActiveHealthInfoAsync();

                SmokeEffects = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "SmokeEffects"));
                
                QuitBenefits = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "QuitBenefits"));
                
                HealthTips = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "Tips"));
            }
            catch
            {
                // Handle error
                SmokeEffects = new ObservableCollection<HealthInfo>();
                QuitBenefits = new ObservableCollection<HealthInfo>();
                HealthTips = new ObservableCollection<HealthInfo>();
                HealthProgress = new ObservableCollection<HealthTrackingOverview>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RefreshHealthInfoAsync()
        {
            LoadHealthInfoAsync();
        }
    }
} 
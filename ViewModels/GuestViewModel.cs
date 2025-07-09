using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System;

namespace QuitSmartApp.ViewModels
{
    public class GuestViewModel : BaseViewModel
    {
        private readonly IHealthInfoRepository _healthInfoRepository;

        private IEnumerable<HealthInfo> _smokeEffects = new List<HealthInfo>();
        private IEnumerable<HealthInfo> _quitBenefits = new List<HealthInfo>();
        private IEnumerable<HealthInfo> _healthTips = new List<HealthInfo>();
        private bool _isLoading = true;

        // Events for navigation
        public event Action? NavigateToLoginRequested;
        public event Action? NavigateToRegisterRequested;

        public GuestViewModel(IHealthInfoRepository healthInfoRepository)
        {
            _healthInfoRepository = healthInfoRepository;

            // Initialize commands
            NavigateToLoginCommand = new RelayCommand(() => NavigateToLoginRequested?.Invoke());
            NavigateToRegisterCommand = new RelayCommand(() => NavigateToRegisterRequested?.Invoke());

            LoadHealthInfoAsync();
        }

        // Properties
        public IEnumerable<HealthInfo> SmokeEffects
        {
            get => _smokeEffects;
            set => SetProperty(ref _smokeEffects, value);
        }

        public IEnumerable<HealthInfo> QuitBenefits
        {
            get => _quitBenefits;
            set => SetProperty(ref _quitBenefits, value);
        }

        public IEnumerable<HealthInfo> HealthTips
        {
            get => _healthTips;
            set => SetProperty(ref _healthTips, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        // Methods
        private async void LoadHealthInfoAsync()
        {
            try
            {
                IsLoading = true;

                var allHealthInfo = await _healthInfoRepository.GetActiveHealthInfoAsync();

                SmokeEffects = allHealthInfo.Where(h => h.InfoType == "SmokeEffects").ToList();
                QuitBenefits = allHealthInfo.Where(h => h.InfoType == "QuitBenefits").ToList();
                HealthTips = allHealthInfo.Where(h => h.InfoType == "Tips").ToList();
            }
            catch
            {
                // Handle error silently for guest view
                SmokeEffects = new List<HealthInfo>();
                QuitBenefits = new List<HealthInfo>();
                HealthTips = new List<HealthInfo>();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

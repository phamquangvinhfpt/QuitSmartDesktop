using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace QuitSmartApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing user badge collection
    /// </summary>
    public class BadgeCollectionViewModel : BaseViewModel
    {
        private readonly IBadgeService _badgeService;
        private readonly Guid _currentUserId;

        private ObservableCollection<UserBadgeCollection> _userBadges = new();
        private ObservableCollection<BadgeDefinition> _availableBadges = new();
        private bool _isLoading = true;

        public BadgeCollectionViewModel(IBadgeService badgeService, IAuthenticationService authenticationService)
        {
            _badgeService = badgeService;
            _currentUserId = authenticationService.CurrentUserId ?? Guid.Empty;

            LoadBadgesAsync();
        }

        // Properties
        public ObservableCollection<UserBadgeCollection> UserBadges
        {
            get => _userBadges;
            set => SetProperty(ref _userBadges, value);
        }

        public ObservableCollection<BadgeDefinition> AvailableBadges
        {
            get => _availableBadges;
            set => SetProperty(ref _availableBadges, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Methods
        private async void LoadBadgesAsync()
        {
            try
            {
                IsLoading = true;

                // Load user badges
                var userBadges = await _badgeService.GetUserBadgeCollectionAsync(_currentUserId);
                UserBadges = new ObservableCollection<UserBadgeCollection>(userBadges);

                // Load available badges
                var availableBadges = await _badgeService.GetAllAvailableBadgesAsync();
                AvailableBadges = new ObservableCollection<BadgeDefinition>(availableBadges);
            }
            catch
            {
                // Handle error
                UserBadges = new ObservableCollection<UserBadgeCollection>();
                AvailableBadges = new ObservableCollection<BadgeDefinition>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RefreshBadgesAsync()
        {
            LoadBadgesAsync();
        }
    }
}
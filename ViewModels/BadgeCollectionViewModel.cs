using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Windows.Input;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for managing user badge collection
    public class BadgeCollectionViewModel : BaseViewModel
    {
        private readonly IBadgeService _badgeService;
        private readonly Guid _currentUserId;

        private ObservableCollection<UserBadgeCollection> _userBadges = new();
        private ObservableCollection<BadgeDefinition> _availableBadges = new();
        private bool _isLoading = true;

        // Navigation action
        public Action? NavigateBack { get; set; }

        public BadgeCollectionViewModel(IBadgeService badgeService, IAuthenticationService authenticationService)
        {
            _badgeService = badgeService;
            _currentUserId = authenticationService?.CurrentUserId ?? Guid.Empty;

            // Initialize commands
            BackCommand = new RelayCommand(() => NavigateBack?.Invoke());

            // Initialize collections to prevent null reference
            UserBadges = new ObservableCollection<UserBadgeCollection>();
            AvailableBadges = new ObservableCollection<BadgeDefinition>();

            LoadBadgesAsync();
        }

        // Properties
        public ObservableCollection<UserBadgeCollection> UserBadges
        {
            get => _userBadges;
            set
            {
                SetProperty(ref _userBadges, value);
                // Notify computed properties
                OnPropertyChanged(nameof(EarnedBadgeCount));
                OnPropertyChanged(nameof(InProgressBadgeCount));
            }
        }

        public ObservableCollection<BadgeDefinition> AvailableBadges
        {
            get => _availableBadges;
            set
            {
                SetProperty(ref _availableBadges, value);
                // Notify computed property
                OnPropertyChanged(nameof(TotalBadgeCount));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Computed properties for statistics  
        public int EarnedBadgeCount => UserBadges?.Count(b => b.IsEarned) ?? 0;

        public int InProgressBadgeCount => UserBadges?.Count(b => !b.IsEarned) ?? 0;

        public int TotalBadgeCount => AvailableBadges?.Count() ?? 0;

        // Commands
        public ICommand BackCommand { get; }

        // Methods
        private async void LoadBadgesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadBadgesAsync started");
                IsLoading = true;

                if (_badgeService != null && _currentUserId != Guid.Empty)
                {
                    // Load user badges from database
                    var userBadges = await _badgeService.GetUserBadgeCollectionAsync(_currentUserId);

                    if (userBadges != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Received {userBadges.Count()} user badges");
                        UserBadges = new ObservableCollection<UserBadgeCollection>(userBadges);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No user badges returned");
                        UserBadges = new ObservableCollection<UserBadgeCollection>();
                    }

                    // Load available badges from database
                    var availableBadges = await _badgeService.GetAllAvailableBadgesAsync();

                    if (availableBadges != null)
                    {
                        AvailableBadges = new ObservableCollection<BadgeDefinition>(availableBadges);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No available badges returned");
                        AvailableBadges = new ObservableCollection<BadgeDefinition>();
                    }
                }
                else
                {
                    // No user or service available
                    UserBadges = new ObservableCollection<UserBadgeCollection>();
                    AvailableBadges = new ObservableCollection<BadgeDefinition>();
                }

                System.Diagnostics.Debug.WriteLine("LoadBadgesAsync completed successfully");
            }
            catch (Exception ex)
            {
                // Handle error - log and show empty collections
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                UserBadges = new ObservableCollection<UserBadgeCollection>();
                AvailableBadges = new ObservableCollection<BadgeDefinition>();
            }
            finally
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine("LoadBadgesAsync finally block");
            }
        }



        public async Task RefreshBadgesAsync()
        {
            LoadBadgesAsync();
        }
    }
}
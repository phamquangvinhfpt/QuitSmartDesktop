using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;

namespace QuitSmartApp.ViewModels
{
    public class UserDashboardViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IBadgeService _badgeService;
        private readonly IMotivationalService _motivationalService;
        private readonly IAuthenticationService _authenticationService;

        // Navigation actions
        public Action? NavigateToHealthInfo { get; set; }
        public Action? NavigateToDailyTracking { get; set; }
        public Action? NavigateToBadges { get; set; }
        public Action? NavigateToProfile { get; set; }
        public Action? NavigateToLogin { get; set; }
        public Action? NavigateToStatistics { get; set; }

        private UserStatistic? _userStatistics;
        private UserProfile? _userProfile;
        private MotivationalMessage? _dailyMotivation;
        private string _welcomeMessage = string.Empty;
        private string _todayDate = string.Empty;
        private bool _isLoading = true;

        public UserDashboardViewModel(
            IUserService userService,
            IBadgeService badgeService,
            IMotivationalService motivationalService,
            IAuthenticationService authenticationService)
        {
            _userService = userService;
            _badgeService = badgeService;
            _motivationalService = motivationalService;
            _authenticationService = authenticationService;

            // Initialize commands
            ViewMoreMotivationCommand = new RelayCommand(ViewMoreMotivation);
            LogTodayCommand = new RelayCommand(LogToday);
            ViewStatsCommand = new RelayCommand(ViewStats);
            ViewBadgesCommand = new RelayCommand(ViewBadges);
            ViewProfileCommand = new RelayCommand(ViewProfile);
            LogoutCommand = new RelayCommand(Logout);

            _ = LoadDashboardDataAsync();
        }

        // Properties
        public UserStatistic? UserStatistics
        {
            get => _userStatistics;
            set
            {
                if (SetProperty(ref _userStatistics, value))
                {
                    // Notify computed properties when UserStatistics changes
                    OnPropertyChanged(nameof(DaysQuitText));
                    OnPropertyChanged(nameof(MoneySavedText));
                    OnPropertyChanged(nameof(CurrentStreakText));
                }
            }
        }

        public UserProfile? UserProfile
        {
            get => _userProfile;
            set
            {
                if (SetProperty(ref _userProfile, value))
                {
                    // Notify computed properties when UserProfile changes
                    OnPropertyChanged(nameof(DaysQuitText));
                    OnPropertyChanged(nameof(MoneySavedText));
                    OnPropertyChanged(nameof(CurrentStreakText));
                }
            }
        }

        public MotivationalMessage? DailyMotivation
        {
            get => _dailyMotivation;
            set
            {
                if (SetProperty(ref _dailyMotivation, value))
                {
                    // Notify computed property when DailyMotivation changes
                    OnPropertyChanged(nameof(MotivationText));
                }
            }
        }

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public string TodayDate
        {
            get => _todayDate;
            set => SetProperty(ref _todayDate, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Computed properties for UI binding
        public string DaysQuitText
        {
            get
            {
                if (UserProfile == null)
                    return "Chưa thiết lập";
                return UserStatistics?.TotalDaysQuit?.ToString() ?? "0";
            }
        }

        public string MoneySavedText
        {
            get
            {
                if (UserProfile == null)
                    return "Chưa thiết lập";
                var amount = UserStatistics?.TotalMoneySaved ?? 0;
                return amount.ToString("#,##0") + " ₫";
            }
        }

        public string CurrentStreakText
        {
            get
            {
                if (UserProfile == null)
                    return "Chưa thiết lập";
                return UserStatistics?.CurrentStreak?.ToString() ?? "0";
            }
        }

        public string MotivationText => DailyMotivation?.Content ?? "Hãy bắt đầu hành trình cai thuốc của bạn!";

        // Commands
        public ICommand ViewMoreMotivationCommand { get; }
        public ICommand LogTodayCommand { get; }
        public ICommand ViewStatsCommand { get; }
        public ICommand ViewBadgesCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        // Methods
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    // Always refresh statistics first to ensure latest data
                    await _userService.RefreshUserStatisticsAsync(userId);

                    UserProfile = await _userService.GetUserProfileAsync(userId);
                    UserStatistics = await _userService.GetUserStatisticsAsync(userId);

                    DailyMotivation = await _motivationalService.GetPersonalizedMessageAsync(userId);

                    WelcomeMessage = $"Chào mừng {_authenticationService.CurrentUsername} quay lại!";
                    TodayDate = DateTime.Today.ToString("dddd, dd MMMM yyyy");
                }
            }
            catch (Exception ex)
            {
                // Handle error - could show in a status bar or message
                WelcomeMessage = $"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ViewMoreMotivation()
        {
            // Navigate to motivation/health info view
            NavigateToHealthInfo?.Invoke();
        }

        private void LogToday()
        {
            // Navigate to daily tracking view
            NavigateToDailyTracking?.Invoke();
        }

        private void ViewStats()
        {
            // Navigate to detailed statistics view
            NavigateToStatistics?.Invoke();
        }

        private void ViewBadges()
        {
            // Navigate to badges view
            NavigateToBadges?.Invoke();
        }

        private void ViewProfile()
        {
            // Navigate to profile view
            NavigateToProfile?.Invoke();
        }

        private void Logout()
        {
            try
            {
                // Clear authentication data
                _authenticationService.LogoutAsync();

                // Navigate back to login
                NavigateToLogin?.Invoke();
            }
            catch (Exception ex)
            {
                // Handle logout error
                System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
            }
        }

        public async Task RefreshDataAsync()
        {
            await LoadDashboardDataAsync();
        }

        // Add method to manually refresh data when navigating back
        public async Task RefreshOnNavigationAsync()
        {
            await LoadDashboardDataAsync();
        }
    }
}

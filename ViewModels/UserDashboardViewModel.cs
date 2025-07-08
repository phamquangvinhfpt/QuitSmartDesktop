using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for User Dashboard displaying statistics and motivation
    public class UserDashboardViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IBadgeService _badgeService;
        private readonly IMotivationalService _motivationalService;
        private readonly IAuthenticationService _authenticationService;

        // Navigation actions - can be set after construction
        public Action? NavigateToHealthInfo { get; set; }
        public Action? NavigateToDailyTracking { get; set; }
        public Action? NavigateToBadges { get; set; }
        public Action? NavigateToProfile { get; set; }
        public Action? NavigateToLogin { get; set; }

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

            LoadDashboardDataAsync();
        }

        // Properties
        public UserStatistic? UserStatistics
        {
            get => _userStatistics;
            set => SetProperty(ref _userStatistics, value);
        }

        public UserProfile? UserProfile
        {
            get => _userProfile;
            set => SetProperty(ref _userProfile, value);
        }

        public MotivationalMessage? DailyMotivation
        {
            get => _dailyMotivation;
            set => SetProperty(ref _dailyMotivation, value);
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
        private async void LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    // Load user profile first
                    UserProfile = await _userService.GetUserProfileAsync(userId);

                    // Force refresh statistics if user has profile
                    if (UserProfile != null)
                    {
                        await _userService.RefreshUserStatisticsAsync(userId);
                    }

                    // Load user statistics after refresh
                    UserStatistics = await _userService.GetUserStatisticsAsync(userId);

                    // Load daily motivation
                    DailyMotivation = await _motivationalService.GetPersonalizedMessageAsync(userId);

                    // Set welcome message and date
                    WelcomeMessage = $"Chào mừng {_authenticationService.CurrentUsername} quay lại!";
                    TodayDate = DateTime.Today.ToString("dddd, dd MMMM yyyy");

                    // Notify UI of computed property changes
                    OnPropertyChanged(nameof(DaysQuitText));
                    OnPropertyChanged(nameof(MoneySavedText));
                    OnPropertyChanged(nameof(CurrentStreakText));
                    OnPropertyChanged(nameof(MotivationText));
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
            // Navigate to detailed statistics view (using daily tracking for now)
            NavigateToDailyTracking?.Invoke();
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
                _authenticationService.Logout();

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
            await Task.Run(() => LoadDashboardDataAsync());
        }
    }
}

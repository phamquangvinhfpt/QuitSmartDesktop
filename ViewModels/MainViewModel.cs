using Microsoft.Extensions.DependencyInjection;
using QuitSmartApp.Services.Interfaces;
using System;
using System.Windows.Input;

namespace QuitSmartApp.ViewModels
{
    /// <summary>
    /// Main ViewModel for application navigation and state management
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private BaseViewModel? _currentViewModel;
        private string _currentView = "Guest";

        public MainViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            // Initialize commands
            NavigateToLoginCommand = new RelayCommand(NavigateToLogin);
            NavigateToRegisterCommand = new RelayCommand(NavigateToRegister);
            NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard, () => _authenticationService.IsUserLoggedIn);
            NavigateToProfileCommand = new RelayCommand(NavigateToProfile, () => _authenticationService.IsUserLoggedIn);
            NavigateToDailyTrackingCommand = new RelayCommand(NavigateToDailyTracking, () => _authenticationService.IsUserLoggedIn);
            NavigateToBadgesCommand = new RelayCommand(NavigateToBadges, () => _authenticationService.IsUserLoggedIn);
            NavigateToHealthInfoCommand = new RelayCommand(NavigateToHealthInfo);
            NavigateToAdminCommand = new RelayCommand(NavigateToAdmin, () => _authenticationService.IsAdminLoggedIn);
            LogoutCommand = new RelayCommand(Logout, () => _authenticationService.IsUserLoggedIn || _authenticationService.IsAdminLoggedIn);

            // Set initial view
            LoadInitialView();
        }

        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        // Commands
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToDailyTrackingCommand { get; }
        public ICommand NavigateToBadgesCommand { get; }
        public ICommand NavigateToHealthInfoCommand { get; }
        public ICommand NavigateToAdminCommand { get; }
        public ICommand LogoutCommand { get; }

        // Properties for UI state
        public bool IsUserLoggedIn => _authenticationService.IsUserLoggedIn;
        public bool IsAdminLoggedIn => _authenticationService.IsAdminLoggedIn;
        public bool IsGuestMode => !IsUserLoggedIn && !IsAdminLoggedIn;
        public string? CurrentUsername => _authenticationService.CurrentUsername;

        // Navigation methods
        private void LoadInitialView()
        {
            if (_authenticationService.IsUserLoggedIn)
            {
                NavigateToDashboard();
            }
            else if (_authenticationService.IsAdminLoggedIn)
            {
                NavigateToAdmin();
            }
            else
            {
                NavigateToGuest();
            }
        }

        private void NavigateToGuest()
        {
            CurrentView = "Guest";
            var guestViewModel = App.ServiceProvider.GetService(typeof(GuestViewModel)) as GuestViewModel;
            if (guestViewModel != null)
            {
                // Subscribe to navigation events
                guestViewModel.NavigateToLoginRequested += () => NavigateToLogin();
                guestViewModel.NavigateToRegisterRequested += () => NavigateToRegister();
                CurrentViewModel = guestViewModel;
            }
        }

        private void NavigateToLogin()
        {
            CurrentView = "Login";
            var loginViewModel = App.ServiceProvider.GetService(typeof(LoginViewModel)) as LoginViewModel;
            if (loginViewModel != null)
            {
                // Subscribe to navigation events
                loginViewModel.NavigateToRegisterRequested += () => NavigateToRegister();
                loginViewModel.NavigateToGuestRequested += () => NavigateToGuest();
                loginViewModel.LoginSuccessful += () => { RefreshUIState(); NavigateToDashboard(); };
                loginViewModel.AdminLoginSuccessful += () => { RefreshUIState(); NavigateToAdmin(); };
                CurrentViewModel = loginViewModel;
            }
        }

        private void NavigateToRegister()
        {
            CurrentView = "Register";
            var registerViewModel = App.ServiceProvider.GetService(typeof(RegisterViewModel)) as RegisterViewModel;
            if (registerViewModel != null)
            {
                // Subscribe to navigation events
                registerViewModel.NavigateToLoginRequested += () => NavigateToLogin();
                registerViewModel.NavigateToGuestRequested += () => NavigateToGuest();
                registerViewModel.RegistrationSuccessful += () => { RefreshUIState(); NavigateToDashboard(); };
                CurrentViewModel = registerViewModel;
            }
        }

        private void NavigateToDashboard()
        {
            CurrentView = "Dashboard";

            // Get UserDashboardViewModel from DI container
            var dashboardViewModel = App.ServiceProvider.GetService(typeof(UserDashboardViewModel)) as UserDashboardViewModel;

            if (dashboardViewModel != null)
            {
                // Set navigation actions
                dashboardViewModel.NavigateToHealthInfo = () => NavigateToHealthInfo();
                dashboardViewModel.NavigateToDailyTracking = () => NavigateToDailyTracking();
                dashboardViewModel.NavigateToBadges = () => NavigateToBadges();
                dashboardViewModel.NavigateToProfile = () => NavigateToProfile();

                CurrentViewModel = dashboardViewModel;
            }
        }

        private void NavigateToProfile()
        {
            CurrentView = "Profile";
            var profileViewModel = App.ServiceProvider.GetService(typeof(ProfileViewModel)) as ProfileViewModel;
            if (profileViewModel != null)
            {
                // Set navigation back action based on current user type
                if (_authenticationService.IsUserLoggedIn)
                {
                    profileViewModel.NavigateBack = () => NavigateToDashboard();
                }
                else if (_authenticationService.IsAdminLoggedIn)
                {
                    profileViewModel.NavigateBack = () => NavigateToAdmin();
                }
                else
                {
                    profileViewModel.NavigateBack = () => NavigateToGuest();
                }
                CurrentViewModel = profileViewModel;
            }
        }

        private void NavigateToDailyTracking()
        {
            CurrentView = "DailyTracking";
            var dailyTrackingViewModel = App.ServiceProvider.GetService(typeof(DailyTrackingViewModel)) as DailyTrackingViewModel;
            if (dailyTrackingViewModel != null)
            {
                // Set navigation back action - always go back to dashboard for daily tracking
                dailyTrackingViewModel.NavigateBack = () => NavigateToDashboard();
                CurrentViewModel = dailyTrackingViewModel;
            }
        }

        private void NavigateToBadges()
        {
            CurrentView = "Badges";
            var badgeCollectionViewModel = App.ServiceProvider.GetService(typeof(BadgeCollectionViewModel)) as BadgeCollectionViewModel;
            if (badgeCollectionViewModel != null)
            {
                // Set navigation back action - always go back to dashboard for badges
                badgeCollectionViewModel.NavigateBack = () => NavigateToDashboard();
                CurrentViewModel = badgeCollectionViewModel;
            }
        }

        private void NavigateToHealthInfo()
        {
            CurrentView = "HealthInfo";
            var healthInfoViewModel = App.ServiceProvider.GetService(typeof(HealthInfoViewModel)) as HealthInfoViewModel;
            if (healthInfoViewModel != null)
            {
                // Set navigation back action based on context
                if (_authenticationService.IsUserLoggedIn)
                {
                    healthInfoViewModel.NavigateBack = () => NavigateToDashboard();
                }
                else
                {
                    healthInfoViewModel.NavigateBack = () => NavigateToGuest();
                }
                CurrentViewModel = healthInfoViewModel;
            }
        }

        private async void NavigateToAdmin()
        {
            try
            {
                CurrentView = "Admin";

                // Get AdminDashboardViewModel from DI container
                var adminViewModel = App.ServiceProvider.GetService(typeof(AdminDashboardViewModel)) as AdminDashboardViewModel;

                if (adminViewModel != null)
                {
                    // Set navigation actions
                    adminViewModel.NavigateToGuest = () => NavigateToGuest();

                    CurrentViewModel = adminViewModel;

                    // Initialize the view model after setting it as current
                    await adminViewModel.InitializeAsync();
                }
                else
                {
                    System.Windows.MessageBox.Show("Không thể tạo AdminDashboardViewModel. Vui lòng kiểm tra cấu hình DI.",
                        "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi điều hướng đến Admin Dashboard: {ex.Message}",
                    "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                // Fallback to guest view
                NavigateToGuest();
            }
        }

        private async void Logout()
        {
            await _authenticationService.LogoutAsync();
            RefreshUIState();
            NavigateToGuest();
        }

        public void RefreshUIState()
        {
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(IsAdminLoggedIn));
            OnPropertyChanged(nameof(IsGuestMode));
            OnPropertyChanged(nameof(CurrentUsername));
        }
    }
}

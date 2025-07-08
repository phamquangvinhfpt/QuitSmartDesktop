using QuitSmartApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for Login view handling authentication
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _rememberMe = false;

        // Events for navigation
        public event Action? NavigateToRegisterRequested;
        public event Action? NavigateToGuestRequested;
        public event Action? LoginSuccessful;
        public event Action? AdminLoginSuccessful;

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            // Initialize commands
            LoginCommand = new RelayCommand(async () => await LoginAsync(), () => !IsLoading && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
            NavigateToRegisterCommand = new RelayCommand(() => NavigateToRegisterRequested?.Invoke());
            NavigateToGuestCommand = new RelayCommand(() => NavigateToGuestRequested?.Invoke());
        }

        // Properties
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToGuestCommand { get; }

        // Methods
        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Try user login first
                bool userLoginSuccess = await _authenticationService.LoginUserAsync(Username, Password);
                
                if (userLoginSuccess)
                {
                    LoginSuccessful?.Invoke();
                    return;
                }

                // If user login fails, try admin login
                bool adminLoginSuccess = await _authenticationService.LoginAdminAsync(Username, Password);
                
                if (adminLoginSuccess)
                {
                    AdminLoginSuccessful?.Invoke();
                    return;
                }

                // Both failed
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

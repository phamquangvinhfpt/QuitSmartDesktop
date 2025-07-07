using QuitSmartApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuitSmartApp.ViewModels
{
    /// <summary>
    /// ViewModel for Login view handling authentication
    /// </summary>
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
            AdminLoginCommand = new RelayCommand(async () => await AdminLoginAsync(), () => !IsLoading && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
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
        public ICommand AdminLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToGuestCommand { get; }

        // Methods
        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                bool success = await _authenticationService.LoginUserAsync(Username, Password);

                if (success)
                {
                    LoginSuccessful?.Invoke();
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                }
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

        private async Task AdminLoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                bool success = await _authenticationService.LoginAdminAsync(Username, Password);

                if (success)
                {
                    AdminLoginSuccessful?.Invoke();
                }
                else
                {
                    ErrorMessage = "Thông tin admin không đúng.";
                }
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

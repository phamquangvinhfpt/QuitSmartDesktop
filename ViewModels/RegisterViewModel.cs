using QuitSmartApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace QuitSmartApp.ViewModels
{
    /// <summary>
    /// ViewModel for Register view handling user registration
    /// </summary>
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _fullName = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _agreeToTerms = false;

        // Events for navigation
        public event Action? NavigateToLoginRequested;
        public event Action? NavigateToGuestRequested;
        public event Action? RegistrationSuccessful;

        public RegisterViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            // Initialize commands
            RegisterCommand = new RelayCommand(async () => await RegisterAsync(), CanRegister);
            NavigateToLoginCommand = new RelayCommand(() => NavigateToLoginRequested?.Invoke());
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

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                SetProperty(ref _fullName, value);
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

        public bool AgreeToTerms
        {
            get => _agreeToTerms;
            set
            {
                SetProperty(ref _agreeToTerms, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Commands
        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToGuestCommand { get; }

        // Validation properties
        public bool IsFormValid =>
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword) &&
            !string.IsNullOrWhiteSpace(FullName) &&
            Password == ConfirmPassword &&
            IsValidEmail(Email) &&
            AgreeToTerms;

        // Methods
        private bool CanRegister()
        {
            return !IsLoading && IsFormValid;
        }

        private async Task RegisterAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate form
                if (!ValidateForm())
                    return;

                bool success = await _authenticationService.RegisterUserAsync(Username, Email, Password, FullName);

                if (success)
                {
                    // Auto login after successful registration
                    bool loginSuccess = await _authenticationService.LoginUserAsync(Username, Password);

                    if (loginSuccess)
                    {
                        RegistrationSuccessful?.Invoke();
                    }
                    else
                    {
                        ErrorMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                        NavigateToLoginRequested?.Invoke();
                    }
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc email đã tồn tại.";
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

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui lòng nhập email.";
                return false;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu.";
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                return false;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Vui lòng nhập họ tên.";
                return false;
            }

            if (!AgreeToTerms)
            {
                ErrorMessage = "Vui lòng đồng ý với điều khoản sử dụng.";
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch
            {
                return false;
            }
        }
    }
}

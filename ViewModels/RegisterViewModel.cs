using QuitSmartApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for Register view handling user registration with smoking profile
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        // Basic user information
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _fullName = string.Empty;
        private DateTime? _dateOfBirth;
        private string _gender = "Male";

        // Smoking profile information
        private DateTime? _quitStartDate = DateTime.Today;
        private int _cigarettesPerDay = 10;
        private decimal _pricePerPack = 25000;
        private int _cigarettesPerPack = 20;
        private int? _smokingYears;
        private string _quitReason = string.Empty;

        // UI state
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _agreeToTerms = false;
        private int _currentStep = 1;

        // Events for navigation
        public event Action? NavigateToLoginRequested;
        public event Action? NavigateToGuestRequested;
        public event Action? RegistrationSuccessful;

        public RegisterViewModel(IAuthenticationService authenticationService, IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;

            // Initialize commands
            RegisterCommand = new RelayCommand(async () => await RegisterAsync(), CanRegister);
            NavigateToLoginCommand = new RelayCommand(() => NavigateToLoginRequested?.Invoke());
            NavigateToGuestCommand = new RelayCommand(() => NavigateToGuestRequested?.Invoke());
            NextStepCommand = new RelayCommand(NextStep, CanGoToNextStep);
            PreviousStepCommand = new RelayCommand(PreviousStep, CanGoToPreviousStep);
        }

        // Basic Properties
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

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        // Smoking Profile Properties
        public DateTime? QuitStartDate
        {
            get => _quitStartDate;
            set
            {
                SetProperty(ref _quitStartDate, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int CigarettesPerDay
        {
            get => _cigarettesPerDay;
            set
            {
                SetProperty(ref _cigarettesPerDay, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public decimal PricePerPack
        {
            get => _pricePerPack;
            set
            {
                SetProperty(ref _pricePerPack, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int CigarettesPerPack
        {
            get => _cigarettesPerPack;
            set => SetProperty(ref _cigarettesPerPack, value);
        }

        public int? SmokingYears
        {
            get => _smokingYears;
            set => SetProperty(ref _smokingYears, value);
        }

        public string QuitReason
        {
            get => _quitReason;
            set => SetProperty(ref _quitReason, value);
        }

        // UI State Properties
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

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                SetProperty(ref _currentStep, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Commands
        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToGuestCommand { get; }
        public ICommand NextStepCommand { get; }
        public ICommand PreviousStepCommand { get; }

        // Validation properties
        public bool IsStep1Valid =>
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(Password) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword) &&
            !string.IsNullOrWhiteSpace(FullName) &&
            Password == ConfirmPassword &&
            IsValidEmail(Email);

        public bool IsStep2Valid =>
            QuitStartDate.HasValue &&
            CigarettesPerDay > 0 &&
            PricePerPack > 0 &&
            CigarettesPerPack > 0 &&
            AgreeToTerms;

        public bool IsFormValid => IsStep1Valid && IsStep2Valid;

        // Methods
        private bool CanGoToNextStep()
        {
            return CurrentStep == 1 && IsStep1Valid && !IsLoading;
        }

        private void NextStep()
        {
            if (CurrentStep == 1 && IsStep1Valid)
            {
                CurrentStep = 2;
                ErrorMessage = string.Empty;
            }
        }

        private bool CanGoToPreviousStep()
        {
            return CurrentStep == 2 && !IsLoading;
        }

        private void PreviousStep()
        {
            if (CurrentStep == 2)
            {
                CurrentStep = 1;
                ErrorMessage = string.Empty;
            }
        }

        private bool CanRegister()
        {
            return !IsLoading && CurrentStep == 2 && IsFormValid;
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

                // Register user
                bool success = await _authenticationService.RegisterUserAsync(Username, Email, Password, FullName, DateOfBirth, Gender);

                if (success)
                {
                    // Auto login after successful registration
                    bool loginSuccess = await _authenticationService.LoginUserAsync(Username, Password);

                    if (loginSuccess && _authenticationService.CurrentUserId.HasValue)
                    {
                        // Create user profile with smoking information
                        await CreateUserProfileAsync(_authenticationService.CurrentUserId.Value);

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
                    CurrentStep = 1; // Go back to step 1 to fix the issue
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

        private async Task CreateUserProfileAsync(Guid userId)
        {
            try
            {
                var profile = new Models.UserProfile
                {
                    UserId = userId,
                    QuitStartDate = DateOnly.FromDateTime(QuitStartDate!.Value),
                    CigarettesPerDay = CigarettesPerDay,
                    PricePerPack = PricePerPack,
                    CigarettesPerPack = CigarettesPerPack,
                    SmokingYears = SmokingYears,
                    QuitReason = string.IsNullOrWhiteSpace(QuitReason) ? null : QuitReason
                };

                await _userService.CreateOrUpdateProfileAsync(userId, profile);
            }
            catch (Exception ex)
            {
                // Profile creation failed, but user registration succeeded
                ErrorMessage = $"Tài khoản đã được tạo nhưng có lỗi khi lưu thông tin cai thuốc: {ex.Message}";
            }
        }

        private bool ValidateForm()
        {
            // Step 1 validation
            if (!IsStep1Valid)
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Vui lòng nhập tên đăng nhập.";
                    CurrentStep = 1;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Vui lòng nhập email.";
                    CurrentStep = 1;
                    return false;
                }

                if (!IsValidEmail(Email))
                {
                    ErrorMessage = "Email không hợp lệ.";
                    CurrentStep = 1;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Vui lòng nhập mật khẩu.";
                    CurrentStep = 1;
                    return false;
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                    CurrentStep = 1;
                    return false;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Mật khẩu xác nhận không khớp.";
                    CurrentStep = 1;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(FullName))
                {
                    ErrorMessage = "Vui lòng nhập họ và tên.";
                    CurrentStep = 1;
                    return false;
                }
            }

            // Step 2 validation
            if (!IsStep2Valid)
            {
                if (!QuitStartDate.HasValue)
                {
                    ErrorMessage = "Vui lòng chọn ngày bắt đầu cai thuốc.";
                    return false;
                }

                if (QuitStartDate > DateTime.Today)
                {
                    ErrorMessage = "Ngày bắt đầu cai thuốc không thể là tương lai.";
                    return false;
                }

                if (CigarettesPerDay <= 0)
                {
                    ErrorMessage = "Số điếu mỗi ngày phải lớn hơn 0.";
                    return false;
                }

                if (PricePerPack <= 0)
                {
                    ErrorMessage = "Giá gói thuốc phải lớn hơn 0.";
                    return false;
                }

                if (CigarettesPerPack <= 0)
                {
                    ErrorMessage = "Số điếu mỗi gói phải lớn hơn 0.";
                    return false;
                }

                if (!AgreeToTerms)
                {
                    ErrorMessage = "Vui lòng đồng ý với điều khoản sử dụng.";
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}

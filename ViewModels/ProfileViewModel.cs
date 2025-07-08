using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for Profile view handling user profile management
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IBadgeService _badgeService;

        private UserProfile? _userProfile;
        private UserStatistic? _userStatistic;
        private ObservableCollection<UserBadge> _recentBadges = new();
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private DateTime? _dateOfBirth;
        private string _gender = "Male";
        private DateOnly _quitStartDate = DateOnly.FromDateTime(DateTime.Today);
        private DateOnly? _quitGoalDate;
        private int _cigarettesPerDay = 20;
        private decimal _pricePerPack = 50000;
        private int _cigarettesPerPack = 20;
        private int? _smokingYears;
        private string _quitReason = string.Empty;
        private string _successMessage = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _hasChanges = false;

        // Navigation actions
        public Action? NavigateBack { get; set; }
        public Action? NavigateToChangePassword { get; set; }

        public ProfileViewModel(IUserService userService, IAuthenticationService authenticationService, IBadgeService badgeService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _badgeService = badgeService;

            // Initialize commands
            SaveCommand = new RelayCommand(async () => await SaveProfileAsync(), () => _hasChanges && !_isLoading);
            CancelCommand = new RelayCommand(CancelChanges);
            BackCommand = new RelayCommand(() => NavigateBack?.Invoke());
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            DeleteAccountCommand = new RelayCommand(DeleteAccount);

            // Initialize gender options
            GenderOptions = new List<string> { "Male", "Female", "Other" };

            LoadProfileAsync();
        }

        // Properties
        public UserProfile? UserProfile
        {
            get => _userProfile;
            set => SetProperty(ref _userProfile, value);
        }

        public UserStatistic? UserStatistic
        {
            get => _userStatistic;
            set => SetProperty(ref _userStatistic, value);
        }

        public ObservableCollection<UserBadge> RecentBadges
        {
            get => _recentBadges;
            set => SetProperty(ref _recentBadges, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                    OnProfileChanged();
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (SetProperty(ref _dateOfBirth, value))
                    OnProfileChanged();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                if (SetProperty(ref _gender, value))
                    OnProfileChanged();
            }
        }

        public DateOnly QuitStartDate
        {
            get => _quitStartDate;
            set
            {
                if (SetProperty(ref _quitStartDate, value))
                    OnProfileChanged();
            }
        }

        public DateTime QuitStartDateTime
        {
            get => QuitStartDate.ToDateTime(TimeOnly.MinValue);
            set
            {
                var dateOnly = DateOnly.FromDateTime(value);
                if (QuitStartDate != dateOnly)
                {
                    QuitStartDate = dateOnly;
                    OnPropertyChanged();
                }
            }
        }

        public DateOnly? QuitGoalDate
        {
            get => _quitGoalDate;
            set
            {
                if (SetProperty(ref _quitGoalDate, value))
                    OnProfileChanged();
            }
        }

        public int CigarettesPerDay
        {
            get => _cigarettesPerDay;
            set
            {
                if (SetProperty(ref _cigarettesPerDay, value))
                    OnProfileChanged();
            }
        }

        public decimal PricePerPack
        {
            get => _pricePerPack;
            set
            {
                if (SetProperty(ref _pricePerPack, value))
                    OnProfileChanged();
            }
        }

        public int CigarettesPerPack
        {
            get => _cigarettesPerPack;
            set
            {
                if (SetProperty(ref _cigarettesPerPack, value))
                    OnProfileChanged();
            }
        }

        public int? SmokingYears
        {
            get => _smokingYears;
            set
            {
                if (SetProperty(ref _smokingYears, value))
                    OnProfileChanged();
            }
        }

        public string QuitReason
        {
            get => _quitReason;
            set
            {
                if (SetProperty(ref _quitReason, value))
                    OnProfileChanged();
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set => SetProperty(ref _hasChanges, value);
        }

        public List<string> GenderOptions { get; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SaveProfileCommand => SaveCommand; // Alias for SaveCommand
        public ICommand ChangePasswordCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        // Methods
        private async void LoadProfileAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    // Load basic user info from authentication service
                    Username = _authenticationService.CurrentUsername ?? string.Empty;

                    // Load user from database to get email and other info
                    try
                    {
                        var user = await _userService.GetUserAsync(userId);
                        if (user != null)
                        {
                            Email = user.Email;
                            FullName = user.FullName;
                            DateOfBirth = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue);
                            Gender = user.Gender ?? "Male";
                        }
                    }
                    catch (Exception)
                    {
                        // If user lookup fails, at least set email from auth service
                        Email = _authenticationService.CurrentUsername ?? string.Empty;
                    }

                    // Load user profile
                    UserProfile = await _userService.GetUserProfileAsync(userId);

                    if (UserProfile != null)
                    {
                        // Load profile data into form
                        QuitStartDate = UserProfile.QuitStartDate;
                        QuitGoalDate = UserProfile.QuitGoalDate;
                        CigarettesPerDay = UserProfile.CigarettesPerDay;
                        PricePerPack = UserProfile.PricePerPack;
                        CigarettesPerPack = UserProfile.CigarettesPerPack ?? 20;
                        SmokingYears = UserProfile.SmokingYears;
                        QuitReason = UserProfile.QuitReason ?? string.Empty;
                    }

                    // Load user statistics
                    try
                    {
                        UserStatistic = await _userService.GetUserStatisticsAsync(userId);
                    }
                    catch (Exception)
                    {
                        // Create default statistic if not found
                        UserStatistic = new UserStatistic
                        {
                            UserId = userId,
                            TotalDaysQuit = UserProfile?.QuitStartDate != null ?
        (DateTime.Today - UserProfile.QuitStartDate.ToDateTime(TimeOnly.MinValue)).Days : 0,
                            TotalMoneySaved = 0,
                            CurrentStreak = 0
                        };
                    }

                    // Load recent badges
                    try
                    {
                        var badges = await _badgeService.GetNewlyEarnedBadgesAsync(userId);
                        RecentBadges = new ObservableCollection<UserBadge>(badges.Take(5)); // Get latest 5 badges
                    }
                    catch (Exception)
                    {
                        RecentBadges = new ObservableCollection<UserBadge>();
                    }

                    // Reset change tracking
                    HasChanges = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveProfileAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (!ValidateForm())
                    return;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    // Update user basic info
                    var user = await _userService.GetUserAsync(userId);
                    if (user != null)
                    {
                        user.FullName = FullName;
                        user.DateOfBirth = DateOfBirth.HasValue ? DateOnly.FromDateTime(DateOfBirth.Value) : null;
                        user.Gender = Gender;
                        // Note: Username and Email are not editable in this view
                        await _userService.UpdateUserAsync(user);
                    }

                    // Update user profile (smoking info)
                    var profile = new UserProfile
                    {
                        UserId = userId,
                        QuitStartDate = QuitStartDate,
                        QuitGoalDate = QuitGoalDate,
                        CigarettesPerDay = CigarettesPerDay,
                        PricePerPack = PricePerPack,
                        CigarettesPerPack = CigarettesPerPack,
                        SmokingYears = SmokingYears,
                        QuitReason = QuitReason
                    };

                    await _userService.CreateOrUpdateProfileAsync(userId, profile);

                    SuccessMessage = "Hồ sơ đã được lưu thành công!";
                    HasChanges = false;

                    // Refresh statistics after profile update
                    await _userService.RefreshUserStatisticsAsync(userId);
                    UserStatistic = await _userService.GetUserStatisticsAsync(userId);

                    // Clear success message after 3 seconds
                    await Task.Delay(3000);
                    SuccessMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra khi lưu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CancelChanges()
        {
            LoadProfileAsync();
        }

        private void OnProfileChanged()
        {
            HasChanges = true;
        }

        private bool ValidateForm()
        {
            if (QuitStartDate > DateOnly.FromDateTime(DateTime.Today))
            {
                ErrorMessage = "Ngày bắt đầu cai thuốc không thể là tương lai.";
                return false;
            }

            if (QuitGoalDate.HasValue && QuitGoalDate <= QuitStartDate)
            {
                ErrorMessage = "Ngày mục tiêu phải sau ngày bắt đầu cai thuốc.";
                return false;
            }

            if (CigarettesPerDay <= 0 || CigarettesPerDay > 100)
            {
                ErrorMessage = "Số điếu mỗi ngày phải từ 1 đến 100.";
                return false;
            }

            if (PricePerPack <= 0)
            {
                ErrorMessage = "Giá gói thuốc phải lớn hơn 0.";
                return false;
            }

            if (CigarettesPerPack <= 0 || CigarettesPerPack > 50)
            {
                ErrorMessage = "Số điếu mỗi gói phải từ 1 đến 50.";
                return false;
            }

            if (SmokingYears.HasValue && (SmokingYears < 0 || SmokingYears > 80))
            {
                ErrorMessage = "Số năm hút thuốc phải từ 0 đến 80.";
                return false;
            }

            return true;
        }

        public async Task RefreshAsync()
        {
            await Task.Run(() => LoadProfileAsync());
        }

        private void ChangePassword()
        {
            NavigateToChangePassword?.Invoke();
        }

        private void DeleteAccount()
        {
            var result = System.Windows.MessageBox.Show(
                "Bạn có chắc chắn muốn xóa tài khoản? Hành động này không thể hoàn tác!",
                "Xác nhận xóa tài khoản",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // TODO: Implement delete account functionality
                System.Windows.MessageBox.Show("Chức năng xóa tài khoản sẽ được triển khai trong phiên bản tiếp theo.",
                    "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}

using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for Daily Tracking view handling daily logs
    public class DailyTrackingViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
        private bool _hasSmoked = false;
        private string _healthStatus = "Good";
        private string _notes = string.Empty;
        private DailyLog? _currentLog;
        private IEnumerable<DailyLog> _recentLogs = new List<DailyLog>();
        private string _successMessage = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _isToday = true;

        // Navigation action
        public Action? NavigateBack { get; set; }

        public DailyTrackingViewModel(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;

            // Initialize commands
            SaveLogCommand = new RelayCommand(async () => await SaveLogAsync(), () => !_isLoading);
            PreviousDayCommand = new RelayCommand(PreviousDay);
            NextDayCommand = new RelayCommand(NextDay, () => _selectedDate < DateOnly.FromDateTime(DateTime.Today));
            TodayCommand = new RelayCommand(GoToToday);
            BackCommand = new RelayCommand(() => NavigateBack?.Invoke());

            // Initialize health status options
            HealthStatusOptions = new List<string> { "Good", "Average", "Poor" };

            LoadDataAsync();
        }

        // Properties
        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    IsToday = value == DateOnly.FromDateTime(DateTime.Today);
                    _ = LoadLogForDateAsync();
                }
            }
        }

        public bool HasSmoked
        {
            get => _hasSmoked;
            set => SetProperty(ref _hasSmoked, value);
        }

        public string HealthStatus
        {
            get => _healthStatus;
            set => SetProperty(ref _healthStatus, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public DailyLog? CurrentLog
        {
            get => _currentLog;
            set => SetProperty(ref _currentLog, value);
        }

        public IEnumerable<DailyLog> RecentLogs
        {
            get => _recentLogs;
            set => SetProperty(ref _recentLogs, value);
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

        public bool IsToday
        {
            get => _isToday;
            set => SetProperty(ref _isToday, value);
        }

        public List<string> HealthStatusOptions { get; }

        // Computed properties
        public string SelectedDateDisplay => SelectedDate.ToString("dddd, dd MMMM yyyy");
        public string TodayButtonText => IsToday ? "Hôm nay" : "Về hôm nay";

        // Commands
        public ICommand SaveLogCommand { get; }
        public ICommand PreviousDayCommand { get; }
        public ICommand NextDayCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand BackCommand { get; }

        // Methods
        private async void LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    // Load today's log
                    await LoadLogForDateAsync();

                    // Load recent logs (last 7 days)
                    var logs = await _userService.GetUserDailyLogsAsync(userId, 7);
                    RecentLogs = logs.OrderByDescending(l => l.LogDate);
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

        private async Task LoadLogForDateAsync()
        {
            try
            {
                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;
                    var logs = await _userService.GetUserDailyLogsAsync(userId);
                    CurrentLog = logs.FirstOrDefault(l => l.LogDate == SelectedDate);

                    if (CurrentLog != null)
                    {
                        // Load existing log data
                        HasSmoked = CurrentLog.HasSmoked ?? false;
                        HealthStatus = CurrentLog.HealthStatus ?? "Good";
                        Notes = CurrentLog.Notes ?? string.Empty;
                    }
                    else
                    {
                        // Reset form for new log
                        HasSmoked = false;
                        HealthStatus = "Good";
                        Notes = string.Empty;
                    }

                    // Update computed properties
                    OnPropertyChanged(nameof(SelectedDateDisplay));
                    OnPropertyChanged(nameof(TodayButtonText));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra khi tải nhật ký: {ex.Message}";
            }
        }

        private async Task SaveLogAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;

                    await _userService.LogDailyStatusAsync(userId, SelectedDate, HasSmoked, HealthStatus, Notes);

                    SuccessMessage = "Nhật ký đã được lưu thành công!";

                    // Refresh data
                    await LoadLogForDateAsync();
                    await RefreshRecentLogsAsync();

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

        private void PreviousDay()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        private void NextDay()
        {
            if (SelectedDate < DateOnly.FromDateTime(DateTime.Today))
                SelectedDate = SelectedDate.AddDays(1);
        }

        private void GoToToday()
        {
            SelectedDate = DateOnly.FromDateTime(DateTime.Today);
        }

        private async Task RefreshRecentLogsAsync()
        {
            try
            {
                if (_authenticationService.CurrentUserId.HasValue)
                {
                    var userId = _authenticationService.CurrentUserId.Value;
                    var logs = await _userService.GetUserDailyLogsAsync(userId, 7);
                    RecentLogs = logs.OrderByDescending(l => l.LogDate);
                }
            }
            catch
            {
                // Handle error silently for refresh
            }
        }

        public async Task RefreshAsync()
        {
            await Task.Run(() => LoadDataAsync());
        }
    }
}

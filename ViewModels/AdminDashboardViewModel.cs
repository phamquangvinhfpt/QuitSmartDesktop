using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace QuitSmartApp.ViewModels
{
    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Color { get; set; } = "#007ACC";
    }

    public class LineChartDataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class MonthlyStatistic
    {
        public string Month { get; set; } = string.Empty;
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal MoneySaved { get; set; }
    }

    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;
        private readonly IAuthenticationService _authenticationService;

        // Navigation actions
        public Action? NavigateToGuest { get; set; }
        public Action<UserOverview>? NavigateToUserDetails { get; set; }
        public Action<UserOverview>? NavigateToUserLogs { get; set; }

        public Action? BackToDashboard { get; set; }

        private ObservableCollection<UserOverview> _users = new();
        private ObservableCollection<AdminLog> _adminLogs = new();
        private UserOverview? _selectedUser;
        private int _totalUsers;
        private int _activeUsers;
        private int _totalSessions;
        private decimal _totalMoneySaved;
        private int _averageDaysQuit;
        private bool _isLoading = true;

        // Advanced statistics properties
        private ObservableCollection<ChartDataPoint> _genderDistribution = new();
        private ObservableCollection<ChartDataPoint> _ageGroupDistribution = new();
        private ObservableCollection<MonthlyStatistic> _monthlyStats = new();
        private ObservableCollection<ChartDataPoint> _successRateByDays = new();
        private ObservableCollection<LineChartDataPoint> _successRateLineChartData = new();
        private int _totalBadgesAwarded;
        private decimal _averageMoneySavedPerUser;
        private int _newUsersThisMonth;
        private int _usersQuitOver30Days;
        private int _usersQuitOver90Days;
        private int _usersQuitOver365Days;

        // Tab management
        private int _selectedTabIndex = 0;
        private UserOverview? _selectedUserForDetails;
        private string _userDetailsContent = string.Empty;
        private string _userLogsContent = string.Empty;

        // UserLogs View specific fields
        private string _searchText = string.Empty;
        private DateTime? _filterStartDate;
        private DateTime? _filterEndDate;
        private ObservableCollection<DailyLog> _userDailyLogs = new();
        private int _totalLogEntries;
        private int _successfulDays;
        private int _failedDays;
        private double _successRate;
        private bool _hasMoreLogs;

        // Pagination fields
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<DailyLog> _allLogs = new();
        private bool _isLoadingMore = false;
        private const int PAGE_SIZE = 10;

        private DispatcherTimer? _searchTimer;
        private const int SEARCH_DELAY_MS = 500;

        public AdminDashboardViewModel(IAdminService adminService, IAuthenticationService authenticationService)
        {
            _adminService = adminService;
            _authenticationService = authenticationService;

            // Initialize commands
            LogoutCommand = new RelayCommand(() => LogoutAsync());
            RefreshDataCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
            ViewUserDetailsCommand = new RelayCommand<UserOverview>(ViewUserDetails);
            DeleteUserCommand = new RelayCommand<UserOverview>(DeleteUser, CanDeleteUser);

            ViewUserLogsCommand = new RelayCommand<UserOverview>(ViewUserLogs);

            CloseTabCommand = new RelayCommand<string>(CloseTab);

            // New navigation commands
            OpenUserDetailsCommand = new RelayCommand<UserOverview>(OpenUserDetails);
            OpenUserLogsCommand = new RelayCommand<UserOverview>(OpenUserLogs);

            BackToDashboardCommand = new RelayCommand(() => BackToDashboard?.Invoke());

            // UserLogs View specific commands  
            RefreshLogsCommand = new RelayCommand(RefreshLogs);
            LoadMoreLogsCommand = new RelayCommand(LoadMoreLogs);
            ExportLogsCommand = new RelayCommand(ExportLogs);

            // Pagination commands
            PreviousPageCommand = new RelayCommand(PreviousPage, CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanGoToNextPage);
        }

        public async Task InitializeAsync()
        {
            try
            {
                Users = new ObservableCollection<UserOverview>();
                AdminLogs = new ObservableCollection<AdminLog>();
                TotalUsers = 0;
                ActiveUsers = 0;
                TotalSessions = 0;
                TotalMoneySaved = 0;
                AverageDaysQuit = 0;
                IsLoading = false;

                await LoadDataSafely();

                try
                {
                    if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                    {
                        await _adminService.LogAdminActionAsync(
                            _authenticationService.CurrentUserId.Value,
                            "Truy cập Dashboard Admin",
                            null,
                            "Admin đã truy cập vào bảng điều khiển quản trị"
                        );
                    }
                }
                catch (Exception logEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi ghi log admin: {logEx.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi khởi tạo Admin Dashboard:\n{ex.Message}\n\nInner Exception: {ex.InnerException?.Message}",
                    "Lỗi khởi tạo",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);

                Users = new ObservableCollection<UserOverview>();
                AdminLogs = new ObservableCollection<AdminLog>();
                TotalUsers = 0;
                ActiveUsers = 0;
                TotalSessions = 0;
                TotalMoneySaved = 0;
                AverageDaysQuit = 0;
                IsLoading = false;
            }
        }

        private async Task LoadDataSafely()
        {
            try
            {
                await LoadDashboardDataAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Properties
        public ObservableCollection<UserOverview> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ObservableCollection<AdminLog> AdminLogs
        {
            get => _adminLogs;
            set => SetProperty(ref _adminLogs, value);
        }

        public UserOverview? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        public int ActiveUsers
        {
            get => _activeUsers;
            set => SetProperty(ref _activeUsers, value);
        }

        public int TotalSessions
        {
            get => _totalSessions;
            set => SetProperty(ref _totalSessions, value);
        }

        public decimal TotalMoneySaved
        {
            get => _totalMoneySaved;
            set => SetProperty(ref _totalMoneySaved, value);
        }

        public int AverageDaysQuit
        {
            get => _averageDaysQuit;
            set => SetProperty(ref _averageDaysQuit, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public double ActiveUsersPercentage => TotalUsers > 0 ? (double)ActiveUsers / TotalUsers * 100 : 0;
        public double InactiveUsersPercentage => 100 - ActiveUsersPercentage;
        public int InactiveUsers => TotalUsers - ActiveUsers;

        public ObservableCollection<ChartDataPoint> GenderDistribution
        {
            get => _genderDistribution;
            set => SetProperty(ref _genderDistribution, value);
        }

        public ObservableCollection<ChartDataPoint> AgeGroupDistribution
        {
            get => _ageGroupDistribution;
            set => SetProperty(ref _ageGroupDistribution, value);
        }

        public ObservableCollection<MonthlyStatistic> MonthlyStats
        {
            get => _monthlyStats;
            set => SetProperty(ref _monthlyStats, value);
        }

        public ObservableCollection<ChartDataPoint> SuccessRateByDays
        {
            get => _successRateByDays;
            set => SetProperty(ref _successRateByDays, value);
        }

        public ObservableCollection<LineChartDataPoint> SuccessRateLineChartData
        {
            get => _successRateLineChartData;
            set => SetProperty(ref _successRateLineChartData, value);
        }

        public int TotalBadgesAwarded
        {
            get => _totalBadgesAwarded;
            set => SetProperty(ref _totalBadgesAwarded, value);
        }

        public decimal AverageMoneySavedPerUser
        {
            get => _averageMoneySavedPerUser;
            set => SetProperty(ref _averageMoneySavedPerUser, value);
        }

        public int NewUsersThisMonth
        {
            get => _newUsersThisMonth;
            set => SetProperty(ref _newUsersThisMonth, value);
        }

        public int UsersQuitOver30Days
        {
            get => _usersQuitOver30Days;
            set => SetProperty(ref _usersQuitOver30Days, value);
        }

        public int UsersQuitOver90Days
        {
            get => _usersQuitOver90Days;
            set => SetProperty(ref _usersQuitOver90Days, value);
        }

        public int UsersQuitOver365Days
        {
            get => _usersQuitOver365Days;
            set => SetProperty(ref _usersQuitOver365Days, value);
        }

        // Tab management properties
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public UserOverview? SelectedUserForDetails
        {
            get => _selectedUserForDetails;
            set => SetProperty(ref _selectedUserForDetails, value);
        }

        public string UserDetailsContent
        {
            get => _userDetailsContent;
            set => SetProperty(ref _userDetailsContent, value);
        }

        public string UserLogsContent
        {
            get => _userLogsContent;
            set => SetProperty(ref _userLogsContent, value);
        }

        // UserLogs View specific properties
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _searchTimer?.Stop();
                    _searchTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(SEARCH_DELAY_MS)
                    };
                    _searchTimer.Tick += (s, e) =>
                    {
                        _searchTimer.Stop();
                        try
                        {
                            FilterLogsAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                        }
                    };
                    _searchTimer.Start();
                }
            }
        }

        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                if (SetProperty(ref _filterStartDate, value))
                {
                    _searchTimer?.Stop();
                    _searchTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(SEARCH_DELAY_MS)
                    };
                    _searchTimer.Tick += (s, e) =>
                    {
                        _searchTimer.Stop();
                        try
                        {
                            FilterLogsAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Date filter error: {ex.Message}");
                        }
                    };
                    _searchTimer.Start();
                }
            }
        }

        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                if (SetProperty(ref _filterEndDate, value))
                {
                    _searchTimer?.Stop();
                    _searchTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(SEARCH_DELAY_MS)
                    };
                    _searchTimer.Tick += (s, e) =>
                    {
                        _searchTimer.Stop();
                        try
                        {
                            FilterLogsAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Date filter error: {ex.Message}");
                        }
                    };
                    _searchTimer.Start();
                }
            }
        }

        public ObservableCollection<DailyLog> UserDailyLogs
        {
            get => _userDailyLogs;
            set => SetProperty(ref _userDailyLogs, value);
        }

        public int TotalLogEntries
        {
            get => _totalLogEntries;
            set => SetProperty(ref _totalLogEntries, value);
        }

        public int SuccessfulDays
        {
            get => _successfulDays;
            set => SetProperty(ref _successfulDays, value);
        }

        public int FailedDays
        {
            get => _failedDays;
            set => SetProperty(ref _failedDays, value);
        }

        public double SuccessRate
        {
            get => _successRate;
            set => SetProperty(ref _successRate, value);
        }

        public bool HasMoreLogs
        {
            get => _hasMoreLogs;
            set => SetProperty(ref _hasMoreLogs, value);
        }

        // Pagination properties
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            set => SetProperty(ref _isLoadingMore, value);
        }

        public string PageInfo => $"Trang {CurrentPage} / {TotalPages} ({TotalLogEntries} bản ghi)";

        // Commands
        public ICommand LogoutCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ICommand ViewUserDetailsCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ICommand ViewUserLogsCommand { get; }

        public ICommand CloseTabCommand { get; }

        // New navigation commands for separate views
        public ICommand OpenUserDetailsCommand { get; }
        public ICommand OpenUserLogsCommand { get; }

        public ICommand BackToDashboardCommand { get; }

        // UserLogs View specific commands
        public ICommand RefreshLogsCommand { get; }
        public ICommand LoadMoreLogsCommand { get; }
        public ICommand ExportLogsCommand { get; }

        // Pagination commands
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        // Methods
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;

                Users?.Clear();

                var usersOverview = await _adminService.GetAllUsersOverviewAsync();
                Users = new ObservableCollection<UserOverview>(usersOverview ?? new List<UserOverview>());

                // Load admin logs
                var logs = await _adminService.GetAdminLogsAsync();
                AdminLogs = new ObservableCollection<AdminLog>(logs?.Take(50) ?? new List<AdminLog>());

                // Calculate statistics
                TotalUsers = Users.Count;
                ActiveUsers = Users.Count(u => u.IsActive == true);
                TotalSessions = Users.Count;
                TotalMoneySaved = Users.Sum(u => u.TotalMoneySaved ?? 0);
                AverageDaysQuit = Users.Any() ? (int)Users.Average(u => u.TotalDaysQuit ?? 0) : 0;

                // Calculate advanced statistics
                CalculateAdvancedStatistics();

                // Notify chart properties
                OnPropertyChanged(nameof(ActiveUsersPercentage));
                OnPropertyChanged(nameof(InactiveUsersPercentage));
                OnPropertyChanged(nameof(InactiveUsers));

                // Force refresh all properties
                OnPropertyChanged(nameof(Users));
                OnPropertyChanged(nameof(TotalUsers));
                OnPropertyChanged(nameof(ActiveUsers));
                OnPropertyChanged(nameof(TotalMoneySaved));
                OnPropertyChanged(nameof(AverageDaysQuit));
            }
            catch (Exception ex)
            {
                // Handle error - log it or show message
                Users = new ObservableCollection<UserOverview>();
                AdminLogs = new ObservableCollection<AdminLog>();
                TotalUsers = 0;
                ActiveUsers = 0;
                TotalSessions = 0;
                TotalMoneySaved = 0;
                AverageDaysQuit = 0;

                // Show error message
                System.Windows.MessageBox.Show(
                    $"Lỗi khi tải dữ liệu: {ex.Message}",
                    "Lỗi tải dữ liệu",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LogoutAsync()
        {
            try
            {
                // Log logout action
                if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                {
                    await _adminService.LogAdminActionAsync(
                        _authenticationService.CurrentUserId.Value,
                        "Đăng xuất Admin Dashboard",
                        null,
                        "Admin đã đăng xuất khỏi bảng điều khiển quản trị"
                    );
                }

                await _authenticationService.LogoutAsync();
                // Navigate to guest view
                NavigateToGuest?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ViewUserDetails(UserOverview? user)
        {
            if (user == null) return;

            SelectedUserForDetails = user;
            UserDetailsContent = $"Chi tiết người dùng: {user.FullName}\n\n" +
                         $"ID: {user.UserId}\n" +
                               $"Tên đăng nhập: {user.Username}\n" +
                         $"Email: {user.Email}\n" +
                               $"Giới tính: {user.Gender ?? "Không xác định"}\n" +
                         $"Trạng thái: {(user.IsActive == true ? "Hoạt động" : "Không hoạt động")}\n" +
                               $"Ngày tạo: {user.CreatedAt:dd/MM/yyyy HH:mm:ss}\n" +
                               $"Ngày bắt đầu cai: {user.QuitStartDate?.ToString("dd/MM/yyyy") ?? "Chưa xác định"}\n" +
                               $"Số ngày cai: {user.TotalDaysQuit ?? 0} ngày\n" +
                         $"Tiền tiết kiệm: {user.TotalMoneySaved:N0} VNĐ\n" +
                               $"Chuỗi ngày hiện tại: {user.CurrentStreak ?? 0} ngày\n" +
                               $"Chuỗi ngày dài nhất: {user.LongestStreak ?? 0} ngày\n" +
                               $"Số huy hiệu: {user.TotalBadges ?? 0}";

            SelectedTabIndex = 1; // Switch to details tab
        }

        private async void DeleteUser(UserOverview? user)
        {
            if (user == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa người dùng '{user.FullName}' không?\nHành động này không thể hoàn tác!",
                "Xác nhận xóa người dùng",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var deleteResult = await _adminService.DeleteUserAsync(user.UserId);
                    if (deleteResult)
                    {
                        Users.Remove(user);
                        if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                        {
                            await _adminService.LogAdminActionAsync(
                                    _authenticationService.CurrentUserId.Value,
                                "Xóa người dùng",
                                user.UserId,
                                $"Đã xóa người dùng {user.FullName} ({user.Username})"
                            );
                        }

                        // Recalculate statistics
                        await LoadDashboardDataAsync();

                        System.Windows.MessageBox.Show("Người dùng đã được xóa thành công!", "Thành công",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Không thể xóa người dùng. Vui lòng thử lại!", "Lỗi",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi khi xóa người dùng: {ex.Message}", "Lỗi",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteUser(UserOverview? user)
        {
            return user != null && user.IsActive == false;
        }



        private async void ViewUserLogs(UserOverview? user)
        {
            if (user == null) return;

            try
            {
                SelectedUserForDetails = user;

                // Log admin action
                if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                {
                    await _adminService.LogAdminActionAsync(
                        _authenticationService.CurrentUserId.Value,
                        "Xem nhật ký người dùng",
                        user.UserId,
                        $"Admin đã xem nhật ký hoạt động của người dùng {user.FullName} ({user.Username})"
                    );
                }

                // Always load data for UserLogsView first
                await LoadUserLogsAsync(user.UserId);

                if (NavigateToUserLogs != null)
                {
                    NavigateToUserLogs.Invoke(user);
                }
                else
                {
                    // Load logs data for tab view (fallback)
                    var userLogs = await _adminService.GetUserDailyLogsAsync(user.UserId);

                    if (userLogs?.Any() == true)
                    {
                        var logsText = string.Join("\n", userLogs.Take(20).Select(log =>
                        $"[{log.LogDate:dd/MM/yyyy}] Hút thuốc: {(log.HasSmoked == true ? "Có" : "Không")}, " +
                            $"Tình trạng: {log.HealthStatus ?? "Không ghi nhận"}, " +
                            $"Ghi chú: {(string.IsNullOrEmpty(log.Notes) ? "Không có" : log.Notes)}"));

                        UserLogsContent = $"Nhật ký hoạt động của {user.FullName}\n" +
                                        $"Tổng số bản ghi: {userLogs.Count()}\n\n" +
                                        "20 bản ghi gần nhất:\n" +
                                        "─────────────────────────────────────\n" +
                                        logsText;
                    }
                    else
                    {
                        UserLogsContent = $"Nhật ký hoạt động của {user.FullName}\n\n" +
                                        "Không có dữ liệu nhật ký nào được tìm thấy.";
                    }

                    SelectedTabIndex = 2; // Switch to logs tab (fallback)
                }
            }
            catch (Exception ex)
            {
                if (NavigateToUserLogs == null)
                {
                    UserLogsContent = $"Lỗi khi tải nhật ký của {user?.FullName}: {ex.Message}";
                    SelectedTabIndex = 2;
                }
                else
                {
                    System.Windows.MessageBox.Show($"Lỗi khi tải nhật ký: {ex.Message}", "Lỗi",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }



        private void CloseTab(string tabName)
        {
            SelectedTabIndex = 0;
            UserDetailsContent = string.Empty;
            UserLogsContent = string.Empty;
        }

        private void CalculateAdvancedStatistics()
        {
            if (Users?.Any() != true) return;

            // Gender distribution
            var genderGroups = Users.GroupBy(u => u.Gender ?? "Không xác định")
                                   .Select(g => new ChartDataPoint
                                   {
                                       Label = g.Key,
                                       Value = g.Count(),
                                       Color = GetGenderColor(g.Key)
                                   }).ToList();
            GenderDistribution = new ObservableCollection<ChartDataPoint>(genderGroups);

            // Age group distribution
            var ageGroups = Users.Where(u => u.CreatedAt.HasValue)
                                .GroupBy(u => GetAgeGroup(u.CreatedAt.Value))
                                .Select(g => new ChartDataPoint
                                {
                                    Label = g.Key,
                                    Value = g.Count(),
                                    Color = GetAgeGroupColor(g.Key)
                                }).ToList();
            AgeGroupDistribution = new ObservableCollection<ChartDataPoint>(ageGroups);

            // Success rate by days
            var successRates = new List<ChartDataPoint>
             {
                 new ChartDataPoint { Label = "1-7 ngày", Value = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 1 && (u.TotalDaysQuit ?? 0) <= 7), Color = "#FF6B6B" },
                 new ChartDataPoint { Label = "8-30 ngày", Value = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 8 && (u.TotalDaysQuit ?? 0) <= 30), Color = "#4ECDC4" },
                 new ChartDataPoint { Label = "31-90 ngày", Value = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 31 && (u.TotalDaysQuit ?? 0) <= 90), Color = "#45B7D1" },
                 new ChartDataPoint { Label = "91-365 ngày", Value = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 91 && (u.TotalDaysQuit ?? 0) <= 365), Color = "#96CEB4" },
                 new ChartDataPoint { Label = "> 365 ngày", Value = Users.Count(u => (u.TotalDaysQuit ?? 0) > 365), Color = "#FFEAA7" }
             };
            SuccessRateByDays = new ObservableCollection<ChartDataPoint>(successRates);

            // Success rate line chart data
            var lineChartData = new List<LineChartDataPoint>();
            var totalUsers = Users.Count();
            if (totalUsers > 0)
            {
                // Tính tỷ lệ thành công cho từng khoảng thời gian
                var users7Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 7);
                var users30Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 30);
                var users90Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 90);
                var users180Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 180);
                var users365Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 365);

                var chartHeight = 200.0;
                var chartWidth = 400.0;

                lineChartData.Add(new LineChartDataPoint { X = 0, Y = chartHeight - (users7Days / (double)totalUsers * chartHeight), Label = "7 ngày", Value = users7Days });
                lineChartData.Add(new LineChartDataPoint { X = chartWidth * 0.25, Y = chartHeight - (users30Days / (double)totalUsers * chartHeight), Label = "30 ngày", Value = users30Days });
                lineChartData.Add(new LineChartDataPoint { X = chartWidth * 0.5, Y = chartHeight - (users90Days / (double)totalUsers * chartHeight), Label = "90 ngày", Value = users90Days });
                lineChartData.Add(new LineChartDataPoint { X = chartWidth * 0.75, Y = chartHeight - (users180Days / (double)totalUsers * chartHeight), Label = "180 ngày", Value = users180Days });
                lineChartData.Add(new LineChartDataPoint { X = chartWidth, Y = chartHeight - (users365Days / (double)totalUsers * chartHeight), Label = "365 ngày", Value = users365Days });
            }
            SuccessRateLineChartData = new ObservableCollection<LineChartDataPoint>(lineChartData);

            // Additional statistics
            TotalBadgesAwarded = Users.Sum(u => u.TotalBadges ?? 0);
            AverageMoneySavedPerUser = Users.Any() ? Users.Average(u => u.TotalMoneySaved ?? 0) : 0;
            NewUsersThisMonth = Users.Count(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Month == DateTime.Now.Month && u.CreatedAt.Value.Year == DateTime.Now.Year);
            UsersQuitOver30Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 30);
            UsersQuitOver90Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 90);
            UsersQuitOver365Days = Users.Count(u => (u.TotalDaysQuit ?? 0) >= 365);

            // Monthly statistics (last 6 months)
            var monthlyData = new List<MonthlyStatistic>();
            for (int i = 5; i >= 0; i--)
            {
                var targetDate = DateTime.Now.AddMonths(-i);
                var monthUsers = Users.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Month == targetDate.Month && u.CreatedAt.Value.Year == targetDate.Year);

                monthlyData.Add(new MonthlyStatistic
                {
                    Month = targetDate.ToString("MM/yyyy"),
                    NewUsers = monthUsers.Count(),
                    ActiveUsers = monthUsers.Count(u => u.IsActive == true),
                    MoneySaved = monthUsers.Sum(u => u.TotalMoneySaved ?? 0)
                });
            }
            MonthlyStats = new ObservableCollection<MonthlyStatistic>(monthlyData);
        }

        private string GetGenderColor(string gender)
        {
            return gender switch
            {
                "Nam" => "#3498DB",
                "Nữ" => "#E91E63",
                _ => "#95A5A6"
            };
        }

        private string GetAgeGroup(DateTime createdDate)
        {
            var monthsOld = (DateTime.Now - createdDate).Days / 30;
            return monthsOld switch
            {
                < 3 => "Mới (< 3 tháng)",
                < 12 => "Trung bình (3-12 tháng)",
                _ => "Lâu năm (> 1 năm)"
            };
        }

        private string GetAgeGroupColor(string ageGroup)
        {
            return ageGroup switch
            {
                "Mới (< 3 tháng)" => "#2ECC71",
                "Trung bình (3-12 tháng)" => "#F39C12",
                _ => "#8E44AD"
            };
        }

        // New navigation methods for separate views
        private async void OpenUserDetails(UserOverview? user)
        {
            if (user == null) return;

            try
            {
                // Log admin action
                if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                {
                    await _adminService.LogAdminActionAsync(
                        _authenticationService.CurrentUserId.Value,
                        "Mở view chi tiết người dùng",
                        user.UserId,
                        $"Admin đã mở view chi tiết cho người dùng {user.FullName} ({user.Username})"
                    );
                }

                // Refresh user data trước khi navigate
                await LoadDashboardDataAsync();

                // Find the refreshed user data
                var refreshedUser = Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (refreshedUser != null)
                {
                    SelectedUserForDetails = refreshedUser;
                    NavigateToUserDetails?.Invoke(refreshedUser);
                }
                else
                {
                    SelectedUserForDetails = user;
                    NavigateToUserDetails?.Invoke(user);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi mở chi tiết người dùng: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void OpenUserLogs(UserOverview? user)
        {
            if (user == null) return;

            try
            {
                // Log admin action
                if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                {
                    await _adminService.LogAdminActionAsync(
                        _authenticationService.CurrentUserId.Value,
                        "Mở view nhật ký người dùng",
                        user.UserId,
                        $"Admin đã mở view nhật ký cho người dùng {user.FullName} ({user.Username})"
                    );
                }

                // Set selected user and always reload logs data
                SelectedUserForDetails = user;

                // Always reload data to ensure fresh information
                await LoadUserLogsAsync(user.UserId);

                NavigateToUserLogs?.Invoke(user);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi mở nhật ký người dùng: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }



        // UserLogs View specific methods
        private async void RefreshLogs()
        {
            if (SelectedUserForDetails == null) return;

            try
            {
                await LoadUserLogsAsync(SelectedUserForDetails.UserId);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi làm mới nhật ký: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void LoadMoreLogs()
        {
            if (SelectedUserForDetails == null || IsLoadingMore) return;

            try
            {
                IsLoadingMore = true;

                // Check if there are more pages to load
                if (CurrentPage < TotalPages)
                {
                    // Move to next page
                    CurrentPage++;

                    // Load the current page
                    LoadCurrentPage();
                }
                else
                {
                    // No more pages available
                    HasMoreLogs = false;
                    System.Windows.MessageBox.Show("Đã tải hết tất cả nhật ký!", "Thông báo",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải thêm nhật ký: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private void ExportLogs()
        {
            if (SelectedUserForDetails == null || _allLogs?.Any() != true)
            {
                System.Windows.MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create SaveFileDialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Xuất nhật ký hoạt động",
                    Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 1,
                    DefaultExt = "csv",
                    FileName = $"NhatKy_{SelectedUserForDetails.FullName}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Create CSV content with all logs
                    var csvContent = new System.Text.StringBuilder();

                    csvContent.AppendLine("Ngày,Hút thuốc,Tình trạng sức khỏe,Ghi chú");

                    // Data rows
                    foreach (var log in _allLogs)
                    {
                        var line = $"{log.LogDate:dd/MM/yyyy}," +
                                  $"\"{(log.HasSmoked == true ? "Có" : "Không")}\"," +
                                  $"\"{log.HealthStatus?.Replace("\"", "\"\"") ?? ""}\"," +
                                  $"\"{log.Notes?.Replace("\"", "\"\"") ?? ""}\"";
                        csvContent.AppendLine(line);
                    }

                    System.IO.File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), System.Text.Encoding.UTF8);

                    // Show success message
                    System.Windows.MessageBox.Show(
                        $"Đã xuất thành công {_allLogs.Count} bản ghi nhật ký!\n\nFile đã được lưu tại:\n{saveFileDialog.FileName}",
                        "Xuất nhật ký thành công",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);

                    // Log export action
                    if (_authenticationService.CurrentUserId.HasValue && _authenticationService.CurrentUserId.Value != Guid.Empty)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _adminService.LogAdminActionAsync(
                                    _authenticationService.CurrentUserId.Value,
                                    "Xuất nhật ký người dùng",
                                    SelectedUserForDetails.UserId,
                                    $"Admin đã xuất {_allLogs.Count} bản ghi nhật ký của người dùng {SelectedUserForDetails.FullName} ra file {System.IO.Path.GetFileName(saveFileDialog.FileName)}"
                                );
                            }
                            catch (Exception logEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to log export action: {logEx.Message}");
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi xuất nhật ký: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task LoadUserLogsAsync(Guid userId)
        {
            try
            {
                var logs = await _adminService.GetUserDailyLogsAsync(userId);

                if (logs?.Any() == true)
                {
                    var logsList = logs.OrderByDescending(l => l.LogDate).ToList();

                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        try
                        {
                            var searchTextTrimmed = SearchText.Trim();
                            logsList = logsList.Where(l =>
                            {
                                try
                                {
                                    return (l.Notes?.Contains(searchTextTrimmed, StringComparison.InvariantCultureIgnoreCase) == true) ||
                                           (l.HealthStatus?.Contains(searchTextTrimmed, StringComparison.InvariantCultureIgnoreCase) == true);
                                }
                                catch
                                {
                                    return false;
                                }
                            }).ToList();
                        }
                        catch (Exception searchEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Search filtering error: {searchEx.Message}");
                        }
                    }

                    if (FilterStartDate.HasValue)
                    {
                        try
                        {
                            logsList = logsList.Where(l => l.LogDate >= DateOnly.FromDateTime(FilterStartDate.Value)).ToList();
                        }
                        catch (Exception dateEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Start date filtering error: {dateEx.Message}");
                        }
                    }

                    if (FilterEndDate.HasValue)
                    {
                        try
                        {
                            logsList = logsList.Where(l => l.LogDate <= DateOnly.FromDateTime(FilterEndDate.Value)).ToList();
                        }
                        catch (Exception dateEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"End date filtering error: {dateEx.Message}");
                        }
                    }

                    // Store all filtered logs for pagination
                    _allLogs = logsList;
                    TotalLogEntries = logsList.Count;
                    SuccessfulDays = logsList.Count(l => l.HasSmoked != true);
                    FailedDays = logsList.Count(l => l.HasSmoked == true);
                    SuccessRate = TotalLogEntries > 0 ? (double)SuccessfulDays / TotalLogEntries * 100 : 0;

                    // Calculate pagination
                    TotalPages = (int)Math.Ceiling((double)TotalLogEntries / PAGE_SIZE);
                    if (TotalPages == 0) TotalPages = 1;

                    if (CurrentPage > TotalPages)
                    {
                        CurrentPage = 1;
                    }

                    // Load current page
                    LoadCurrentPage();

                    // Notify pagination related properties
                    OnPropertyChanged(nameof(PageInfo));
                }
                else
                {
                    // Reset UI khi không có data
                    _allLogs = new List<DailyLog>();
                    UserDailyLogs = new ObservableCollection<DailyLog>();
                    TotalLogEntries = 0;
                    SuccessfulDays = 0;
                    FailedDays = 0;
                    SuccessRate = 0;
                    HasMoreLogs = false;
                    CurrentPage = 1;
                    TotalPages = 1;
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUserLogsAsync error: {ex.Message}");

                _allLogs = new List<DailyLog>();
                UserDailyLogs = new ObservableCollection<DailyLog>();
                TotalLogEntries = 0;
                SuccessfulDays = 0;
                FailedDays = 0;
                SuccessRate = 0;
                HasMoreLogs = false;
                CurrentPage = 1;
                TotalPages = 1;
                OnPropertyChanged(nameof(PageInfo));

                throw new Exception($"Lỗi khi tải nhật ký người dùng: {ex.Message}", ex);
            }
        }

        private async void FilterLogsAsync()
        {
            if (SelectedUserForDetails == null) return;

            try
            {
                await LoadUserLogsAsync(SelectedUserForDetails.UserId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FilterLogsAsync error: {ex.Message}");

                System.Diagnostics.Debug.WriteLine("Search filtering encountered an error, showing original data");

                try
                {
                    var originalSearchText = SearchText;
                    var originalStartDate = FilterStartDate;
                    var originalEndDate = FilterEndDate;

                    _searchText = string.Empty;
                    _filterStartDate = null;
                    _filterEndDate = null;

                    await LoadUserLogsAsync(SelectedUserForDetails.UserId);

                    _searchText = originalSearchText;
                    _filterStartDate = originalStartDate;
                    _filterEndDate = originalEndDate;

                    // Notify UI of restored values
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(FilterStartDate));
                    OnPropertyChanged(nameof(FilterEndDate));
                }
                catch
                {
                    UserDailyLogs = new ObservableCollection<DailyLog>();
                    TotalLogEntries = 0;
                    SuccessfulDays = 0;
                    FailedDays = 0;
                    SuccessRate = 0;
                    HasMoreLogs = false;
                }
            }
        }

        // Pagination methods
        private void PreviousPage()
        {
            if (CanGoToPreviousPage())
            {
                CurrentPage--;
                LoadCurrentPage();
            }
        }

        private bool CanGoToPreviousPage()
        {
            return CurrentPage > 1;
        }

        private void NextPage()
        {
            if (CanGoToNextPage())
            {
                CurrentPage++;
                LoadCurrentPage();
            }
        }

        private bool CanGoToNextPage()
        {
            return CurrentPage < TotalPages;
        }

        private void LoadCurrentPage()
        {
            if (_allLogs?.Any() == true)
            {
                var skipCount = (CurrentPage - 1) * PAGE_SIZE;
                var pageData = _allLogs.Skip(skipCount).Take(PAGE_SIZE).ToList();

                UserDailyLogs = new ObservableCollection<DailyLog>(pageData);

                // Update pagination status
                HasMoreLogs = CurrentPage < TotalPages;

                // Notify UI about pagination changes
                OnPropertyChanged(nameof(PageInfo));
                OnPropertyChanged(nameof(HasMoreLogs));
            }
        }

        public void Dispose()
        {
            _searchTimer?.Stop();
            _searchTimer = null;
        }
    }
}
using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for Admin Dashboard functionality
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;
        private readonly IAuthenticationService _authenticationService;

        // Navigation actions
        public Action? NavigateToGuest { get; set; }

        private ObservableCollection<UserOverview> _users = new();
        private ObservableCollection<AdminLog> _adminLogs = new();
        private UserOverview? _selectedUser;
        private int _totalUsers;
        private int _activeUsers;
        private int _totalSessions;
        private decimal _totalMoneySaved;
        private int _averageDaysQuit;
        private bool _isLoading = true;

        public AdminDashboardViewModel(IAdminService adminService, IAuthenticationService authenticationService)
        {
            _adminService = adminService;
            _authenticationService = authenticationService;

            // Initialize commands
            LogoutCommand = new RelayCommand(() => LogoutAsync());
            RefreshDataCommand = new RelayCommand(async () => await LoadDashboardDataAsync());
            ViewUserDetailsCommand = new RelayCommand<UserOverview>(ViewUserDetails);
            DeleteUserCommand = new RelayCommand<UserOverview>(DeleteUser, CanDeleteUser);
            EditUserCommand = new RelayCommand<UserOverview>(EditUser);
            ViewUserLogsCommand = new RelayCommand<UserOverview>(ViewUserLogs);
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
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi khi khởi tạo Admin Dashboard:\n{ex.Message}",
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
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDashboardDataAsync();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("ex", ex);
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

        // Chart data for simple visualization
        public double ActiveUsersPercentage => TotalUsers > 0 ? (double)ActiveUsers / TotalUsers * 100 : 0;
        public double InactiveUsersPercentage => 100 - ActiveUsersPercentage;
        public int InactiveUsers => TotalUsers - ActiveUsers;

        // Commands
        public ICommand LogoutCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ICommand ViewUserDetailsCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand ViewUserLogsCommand { get; }

        // Methods
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;

                // Load users overview  
                var usersOverview = await _adminService.GetAllUsersOverviewAsync();
                Users = new ObservableCollection<UserOverview>(usersOverview ?? new List<UserOverview>());

                // Load admin logs
                var logs = await _adminService.GetAdminLogsAsync();
                AdminLogs = new ObservableCollection<AdminLog>(logs?.Take(50) ?? new List<AdminLog>());

                // Calculate statistics
                TotalUsers = Users.Count;
                ActiveUsers = Users.Count(u => u.IsActive == true);
                TotalSessions = Users.Count; // Placeholder - could be actual session count
                TotalMoneySaved = Users.Sum(u => u.TotalMoneySaved ?? 0);
                AverageDaysQuit = Users.Any() ? (int)Users.Average(u => u.TotalDaysQuit ?? 0) : 0;

                // Notify chart properties
                OnPropertyChanged(nameof(ActiveUsersPercentage));
                OnPropertyChanged(nameof(InactiveUsersPercentage));
                OnPropertyChanged(nameof(InactiveUsers));
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
            await _authenticationService.LogoutAsync();
            // Navigate to guest view
            NavigateToGuest?.Invoke();
        }

        private void ViewUserDetails(UserOverview? user)
        {
            if (user == null) return;

            var details = $"Chi tiết người dùng:\n\n" +
                         $"ID: {user.UserId}\n" +
                         $"Tên: {user.FullName}\n" +
                         $"Email: {user.Email}\n" +
                         $"Giới tính: {user.Gender}\n" +
                         $"Trạng thái: {(user.IsActive == true ? "Hoạt động" : "Không hoạt động")}\n" +
                         $"Ngày tạo: {user.CreatedAt:dd/MM/yyyy}\n" +
                         $"Ngày bắt đầu cai: {user.QuitStartDate:dd/MM/yyyy}\n" +
                         $"Số ngày cai: {user.TotalDaysQuit}\n" +
                         $"Tiền tiết kiệm: {user.TotalMoneySaved:N0} VNĐ\n" +
                         $"Chuỗi ngày hiện tại: {user.CurrentStreak}\n" +
                         $"Chuỗi ngày dài nhất: {user.LongestStreak}\n" +
                         $"Số huy hiệu: {user.TotalBadges}";

            System.Windows.MessageBox.Show(details, "Chi tiết người dùng",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
                        await _adminService.LogAdminActionAsync(
                            _authenticationService.CurrentUserId ?? Guid.Empty,
                            "Xóa người dùng",
                            user.UserId,
                            $"Đã xóa người dùng {user.FullName} ({user.Username})"
                        );

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
            return user != null && user.IsActive == false; // Only allow deleting inactive users
        }

        private void EditUser(UserOverview? user)
        {
            if (user == null) return;

            System.Windows.MessageBox.Show(
                "Chức năng sửa người dùng sẽ được triển khai trong phiên bản tiếp theo.",
                "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private async void ViewUserLogs(UserOverview? user)
        {
            if (user == null) return;

            try
            {
                var userLogs = await _adminService.GetUserDailyLogsAsync(user.UserId);
                var logsText = string.Join("\n", userLogs.Take(10).Select(log =>
                    $"[{log.LogDate:dd/MM/yyyy}] Hút thuốc: {(log.HasSmoked == true ? "Có" : "Không")}, " +
                    $"Tình trạng sức khỏe: {log.HealthStatus ?? "Không ghi nhận"}, Ghi chú: {log.Notes ?? "Không có"}"));

                var message = $"Nhật ký gần đây của {user.FullName}:\n\n" +
                             (string.IsNullOrEmpty(logsText) ? "Không có nhật ký nào." : logsText);

                System.Windows.MessageBox.Show(message, "Nhật ký người dùng",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải nhật ký: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
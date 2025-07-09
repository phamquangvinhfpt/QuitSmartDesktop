using QuitSmartApp.Services.Interfaces;
using QuitSmartApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace QuitSmartApp.ViewModels
{
    public class StatChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Color { get; set; } = "#007ACC";
        public string DisplayValue { get; set; } = string.Empty;
        public string DoughnutPath { get; set; } = string.Empty; // For doughnut chart segments
    }

    public class DailyProgressPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime Date { get; set; }
        public int DaysQuit { get; set; }
        public bool HasSmoked { get; set; }
    }

    public class WeeklyStatistic
    {
        public string Week { get; set; } = string.Empty;
        public int SuccessfulDays { get; set; }
        public int TotalDays { get; set; }
        public decimal MoneySaved { get; set; }
        public double SuccessRate => TotalDays > 0 ? (double)SuccessfulDays / TotalDays * 100 : 0;
    }

    public class UserStatisticsViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        // Navigation action
        public Action? NavigateBack { get; set; }

        private UserStatistic? _userStatistics;
        private UserProfile? _userProfile;
        private bool _isLoading = true;
        private string _errorMessage = string.Empty;

        // Chart data
        private ObservableCollection<StatChartDataPoint> _weeklySuccessRate = new();
        private ObservableCollection<StatChartDataPoint> _monthlyMoneySaved = new();
        private ObservableCollection<DailyProgressPoint> _dailyProgressLine = new();
        private ObservableCollection<WeeklyStatistic> _weeklyStats = new();
        private ObservableCollection<DailyLog> _recentLogs = new();

        // Summary statistics
        private int _totalDaysTracked;
        private int _successfulDays;
        private int _failedDays;
        private double _overallSuccessRate;
        private decimal _totalMoneySaved;
        private decimal _averageDailySavings;
        private int _longestStreak;
        private int _currentStreak;

        // New properties for advanced charts
        private double _successRateOffset;
        private double _streakProgressOffset;
        private string _successPieSegment = string.Empty;
        private string _failedPieSegment = string.Empty;
        private string _dailyProgressAreaPath = string.Empty;
        private string _dailyProgressLinePath = string.Empty;

        public UserStatisticsViewModel(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;

            // Initialize commands
            BackCommand = new RelayCommand(() => NavigateBack?.Invoke());
            RefreshCommand = new RelayCommand(async () => await LoadStatisticsDataAsync());

            // Load data
            _ = LoadStatisticsDataAsync();
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

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Chart data properties
        public ObservableCollection<StatChartDataPoint> WeeklySuccessRate
        {
            get => _weeklySuccessRate;
            set => SetProperty(ref _weeklySuccessRate, value);
        }

        public ObservableCollection<StatChartDataPoint> MonthlyMoneySaved
        {
            get => _monthlyMoneySaved;
            set => SetProperty(ref _monthlyMoneySaved, value);
        }

        public ObservableCollection<DailyProgressPoint> DailyProgressLine
        {
            get => _dailyProgressLine;
            set => SetProperty(ref _dailyProgressLine, value);
        }

        public ObservableCollection<WeeklyStatistic> WeeklyStats
        {
            get => _weeklyStats;
            set => SetProperty(ref _weeklyStats, value);
        }

        public ObservableCollection<DailyLog> RecentLogs
        {
            get => _recentLogs;
            set => SetProperty(ref _recentLogs, value);
        }

        // Summary statistics properties
        public int TotalDaysTracked
        {
            get => _totalDaysTracked;
            set => SetProperty(ref _totalDaysTracked, value);
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

        public double OverallSuccessRate
        {
            get => _overallSuccessRate;
            set => SetProperty(ref _overallSuccessRate, value);
        }

        public decimal TotalMoneySaved
        {
            get => _totalMoneySaved;
            set => SetProperty(ref _totalMoneySaved, value);
        }

        public decimal AverageDailySavings
        {
            get => _averageDailySavings;
            set => SetProperty(ref _averageDailySavings, value);
        }

        public int LongestStreak
        {
            get => _longestStreak;
            set => SetProperty(ref _longestStreak, value);
        }

        public int CurrentStreak
        {
            get => _currentStreak;
            set => SetProperty(ref _currentStreak, value);
        }

        // New chart properties
        public double SuccessRateOffset
        {
            get => _successRateOffset;
            set => SetProperty(ref _successRateOffset, value);
        }

        public double StreakProgressOffset
        {
            get => _streakProgressOffset;
            set => SetProperty(ref _streakProgressOffset, value);
        }

        public string SuccessPieSegment
        {
            get => _successPieSegment;
            set => SetProperty(ref _successPieSegment, value);
        }

        public string FailedPieSegment
        {
            get => _failedPieSegment;
            set => SetProperty(ref _failedPieSegment, value);
        }

        public string DailyProgressAreaPath
        {
            get => _dailyProgressAreaPath;
            set => SetProperty(ref _dailyProgressAreaPath, value);
        }

        public string DailyProgressLinePath
        {
            get => _dailyProgressLinePath;
            set => SetProperty(ref _dailyProgressLinePath, value);
        }

        // Commands
        public ICommand BackCommand { get; }
        public ICommand RefreshCommand { get; }

        // Methods
        private async Task LoadStatisticsDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (!_authenticationService.CurrentUserId.HasValue)
                {
                    ErrorMessage = "Không thể xác định người dùng";
                    return;
                }

                var userId = _authenticationService.CurrentUserId.Value;

                // Load basic data
                UserProfile = await _userService.GetUserProfileAsync(userId);
                UserStatistics = await _userService.GetUserStatisticsAsync(userId);

                // Load daily logs for analysis
                var allLogs = await _userService.GetUserDailyLogsAsync(userId);
                RecentLogs = new ObservableCollection<DailyLog>(allLogs.OrderByDescending(l => l.LogDate).Take(30));

                // Calculate summary statistics
                CalculateSummaryStatistics(allLogs);

                // Generate chart data
                GenerateWeeklySuccessRateChart(allLogs);
                GenerateMonthlyMoneySavedChart(allLogs);
                GenerateDailyProgressLineChart(allLogs);
                GenerateWeeklyStatistics(allLogs);

                // Generate new chart data
                GenerateRadialProgressData();
                GeneratePieChartData();
                GenerateAreaChartData();
                GenerateDoughnutChartData();
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

        private void CalculateSummaryStatistics(IEnumerable<DailyLog> logs)
        {
            var logsList = logs.ToList();

            TotalDaysTracked = logsList.Count;
            SuccessfulDays = logsList.Count(l => l.HasSmoked == false);
            FailedDays = logsList.Count(l => l.HasSmoked == true);
            OverallSuccessRate = TotalDaysTracked > 0 ? (double)SuccessfulDays / TotalDaysTracked * 100 : 0;

            // Money saved calculation
            TotalMoneySaved = UserStatistics?.TotalMoneySaved ?? 0;
            AverageDailySavings = TotalDaysTracked > 0 ? TotalMoneySaved / TotalDaysTracked : 0;

            // Streaks
            LongestStreak = UserStatistics?.LongestStreak ?? 0;
            CurrentStreak = UserStatistics?.CurrentStreak ?? 0;
        }

        private void GenerateRadialProgressData()
        {
            var circumference = 314.159;

            SuccessRateOffset = circumference - (circumference * OverallSuccessRate / 100);

            var maxStreakForProgress = Math.Max(365, LongestStreak);
            var streakProgress = maxStreakForProgress > 0 ? (double)CurrentStreak / maxStreakForProgress * 100 : 0;
            StreakProgressOffset = circumference - (circumference * streakProgress / 100);
        }

        private void GeneratePieChartData()
        {
            if (TotalDaysTracked == 0)
            {
                SuccessPieSegment = "";
                FailedPieSegment = "";
                return;
            }

            var centerX = 100; // Canvas center X
            var centerY = 100; // Canvas center Y  
            var radius = 80;

            var successAngle = (double)SuccessfulDays / TotalDaysTracked * 360;
            var failedAngle = (double)FailedDays / TotalDaysTracked * 360;

            var successStartAngle = 0;
            var successEndAngle = successAngle;
            SuccessPieSegment = CreatePieSegmentPath(centerX, centerY, radius, successStartAngle, successEndAngle);

            var failedStartAngle = successAngle;
            var failedEndAngle = 360;
            FailedPieSegment = CreatePieSegmentPath(centerX, centerY, radius, failedStartAngle, failedEndAngle);
        }

        private void GenerateAreaChartData()
        {
            if (DailyProgressLine.Count == 0) return;

            var chartWidth = 500.0;
            var chartHeight = 200.0;
            var points = DailyProgressLine.OrderBy(p => p.Date).ToList();

            if (points.Count < 2)
            {
                DailyProgressAreaPath = "";
                DailyProgressLinePath = "";
                return;
            }

            // Create line path
            var linePath = $"M {points[0].X},{points[0].Y}";
            for (int i = 1; i < points.Count; i++)
            {
                linePath += $" L {points[i].X},{points[i].Y}";
            }
            DailyProgressLinePath = linePath;

            var areaPath = linePath;
            areaPath += $" L {points.Last().X},{chartHeight} L {points.First().X},{chartHeight} Z";
            DailyProgressAreaPath = areaPath;
        }

        private void GenerateDoughnutChartData()
        {
            if (MonthlyMoneySaved.Count == 0) return;

            var centerX = 90; // Canvas center X
            var centerY = 90; // Canvas center Y
            var outerRadius = 80;
            var innerRadius = 50; // For doughnut hole

            var totalValue = MonthlyMoneySaved.Sum(m => m.Value);
            if (totalValue == 0) return;

            var currentAngle = 0.0;

            var updatedMonthlyData = new List<StatChartDataPoint>();

            for (int i = 0; i < MonthlyMoneySaved.Count; i++)
            {
                var item = MonthlyMoneySaved[i];
                var segmentAngle = (item.Value / totalValue) * 360;

                var doughnutPath = CreateDoughnutSegmentPath(centerX, centerY, innerRadius, outerRadius, currentAngle, currentAngle + segmentAngle);

                var updatedItem = new StatChartDataPoint
                {
                    Label = item.Label,
                    Value = item.Value,
                    Color = GetMonthColor(i),
                    DisplayValue = item.DisplayValue,
                    DoughnutPath = doughnutPath
                };

                updatedMonthlyData.Add(updatedItem);
                currentAngle += segmentAngle;
            }

            MonthlyMoneySaved = new ObservableCollection<StatChartDataPoint>(updatedMonthlyData);
        }

        private string CreatePieSegmentPath(double centerX, double centerY, double radius, double startAngle, double endAngle)
        {
            var startRadians = (startAngle - 90) * Math.PI / 180; // -90 to start from top
            var endRadians = (endAngle - 90) * Math.PI / 180;

            var startX = centerX + radius * Math.Cos(startRadians);
            var startY = centerY + radius * Math.Sin(startRadians);
            var endX = centerX + radius * Math.Cos(endRadians);
            var endY = centerY + radius * Math.Sin(endRadians);

            var largeArcFlag = (endAngle - startAngle) > 180 ? 1 : 0;

            return $"M {centerX},{centerY} L {startX:F2},{startY:F2} A {radius},{radius} 0 {largeArcFlag},1 {endX:F2},{endY:F2} Z";
        }

        private string CreateDoughnutSegmentPath(double centerX, double centerY, double innerRadius, double outerRadius, double startAngle, double endAngle)
        {
            var startRadians = (startAngle - 90) * Math.PI / 180;
            var endRadians = (endAngle - 90) * Math.PI / 180;

            var startOuterX = centerX + outerRadius * Math.Cos(startRadians);
            var startOuterY = centerY + outerRadius * Math.Sin(startRadians);
            var endOuterX = centerX + outerRadius * Math.Cos(endRadians);
            var endOuterY = centerY + outerRadius * Math.Sin(endRadians);

            var startInnerX = centerX + innerRadius * Math.Cos(startRadians);
            var startInnerY = centerY + innerRadius * Math.Sin(startRadians);
            var endInnerX = centerX + innerRadius * Math.Cos(endRadians);
            var endInnerY = centerY + innerRadius * Math.Sin(endRadians);

            var largeArcFlag = (endAngle - startAngle) > 180 ? 1 : 0;

            return $"M {startOuterX:F2},{startOuterY:F2} " +
                   $"A {outerRadius},{outerRadius} 0 {largeArcFlag},1 {endOuterX:F2},{endOuterY:F2} " +
                   $"L {endInnerX:F2},{endInnerY:F2} " +
                   $"A {innerRadius},{innerRadius} 0 {largeArcFlag},0 {startInnerX:F2},{startInnerY:F2} Z";
        }

        private string GetMonthColor(int index)
        {
            var colors = new[]
            {
                "#4CAF50", "#2196F3", "#FF9800", "#9C27B0",
                "#F44336", "#00BCD4", "#FFEB3B", "#795548"
            };
            return colors[index % colors.Length];
        }

        private void GenerateWeeklySuccessRateChart(IEnumerable<DailyLog> logs)
        {
            var weeklyData = logs
                .GroupBy(l => GetWeekOfYear(l.LogDate.ToDateTime(TimeOnly.MinValue)))
                .OrderBy(g => g.Key)
                .Take(12) // Last 12 weeks
                .Select(g =>
                {
                    var successfulDays = g.Count(l => l.HasSmoked == false);
                    var totalDays = g.Count();
                    var successRate = totalDays > 0 ? (double)successfulDays / totalDays * 100 : 0;

                    return new StatChartDataPoint
                    {
                        Label = $"Tuần {g.Key}",
                        Value = successRate,
                        DisplayValue = $"{successRate:F1}%",
                        Color = successRate >= 80 ? "#4CAF50" : successRate >= 60 ? "#FF9800" : "#F44336"
                    };
                })
                .ToList();

            WeeklySuccessRate = new ObservableCollection<StatChartDataPoint>(weeklyData);
        }

        private void GenerateMonthlyMoneySavedChart(IEnumerable<DailyLog> logs)
        {
            if (UserProfile == null) return;

            var dailyCost = CalculateDailyCost();

            var monthlyData = logs
                .GroupBy(l => new { l.LogDate.Year, l.LogDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Take(6) // Last 6 months
                .Select(g =>
                {
                    var successfulDays = g.Count(l => l.HasSmoked == false);
                    var moneySaved = successfulDays * dailyCost;

                    return new StatChartDataPoint
                    {
                        Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Value = (double)moneySaved,
                        DisplayValue = $"{moneySaved:C0}",
                        Color = "#2196F3"
                    };
                })
                .ToList();

            MonthlyMoneySaved = new ObservableCollection<StatChartDataPoint>(monthlyData);
        }

        private void GenerateDailyProgressLineChart(IEnumerable<DailyLog> logs)
        {
            var progressData = logs
                .OrderBy(l => l.LogDate)
                .Take(30) // Last 30 days
                .Select((log, index) =>
                {
                    var chartWidth = 460.0; // Adjusted for new canvas size
                    var chartHeight = 180.0;
                    var x = (index / 29.0) * chartWidth + 20; // Add margin
                    var y = log.HasSmoked == false ? chartHeight * 0.3 : chartHeight * 0.9; // Success higher, failed lower

                    return new DailyProgressPoint
                    {
                        X = x,
                        Y = y,
                        Date = log.LogDate.ToDateTime(TimeOnly.MinValue),
                        HasSmoked = log.HasSmoked ?? false
                    };
                })
                .ToList();

            DailyProgressLine = new ObservableCollection<DailyProgressPoint>(progressData);
        }

        private void GenerateWeeklyStatistics(IEnumerable<DailyLog> logs)
        {
            var dailyCost = CalculateDailyCost();

            var weeklyData = logs
                .GroupBy(l => GetWeekOfYear(l.LogDate.ToDateTime(TimeOnly.MinValue)))
                .OrderByDescending(g => g.Key)
                .Take(8) // Last 8 weeks
                .Select(g =>
                {
                    var successfulDays = g.Count(l => l.HasSmoked == false);
                    var totalDays = g.Count();
                    var moneySaved = successfulDays * dailyCost;

                    return new WeeklyStatistic
                    {
                        Week = $"Tuần {g.Key}",
                        SuccessfulDays = successfulDays,
                        TotalDays = totalDays,
                        MoneySaved = moneySaved
                    };
                })
                .ToList();

            WeeklyStats = new ObservableCollection<WeeklyStatistic>(weeklyData);
        }

        private decimal CalculateDailyCost()
        {
            if (UserProfile == null) return 0;

            var cigarettesPerDay = UserProfile.CigarettesPerDay;
            var pricePerPack = UserProfile.PricePerPack;
            var cigarettesPerPack = UserProfile.CigarettesPerPack ?? 20;

            if (cigarettesPerDay > 0 && pricePerPack > 0 && cigarettesPerPack > 0)
            {
                return (decimal)cigarettesPerDay / cigarettesPerPack * pricePerPack;
            }

            return 0;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }

        public async Task RefreshAsync()
        {
            await LoadStatisticsDataAsync();
        }
    }
}
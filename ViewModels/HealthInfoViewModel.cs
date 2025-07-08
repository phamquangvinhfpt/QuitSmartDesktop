using QuitSmartApp.Repositories.Interfaces;
using QuitSmartApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using System;

namespace QuitSmartApp.ViewModels
{
    // ViewModel for managing health information content
    public class HealthInfoViewModel : BaseViewModel
    {
        private readonly IHealthInfoRepository _healthInfoRepository;

        private ObservableCollection<HealthInfo> _smokeEffects = new();
        private ObservableCollection<HealthInfo> _quitBenefits = new();
        private ObservableCollection<HealthInfo> _healthTips = new();
        private ObservableCollection<HealthTrackingOverview> _healthProgress = new();
        private bool _isLoading = true;

        // Navigation action
        public Action? NavigateBack { get; set; }

        public HealthInfoViewModel(IHealthInfoRepository healthInfoRepository)
        {
            _healthInfoRepository = healthInfoRepository;

            // Initialize commands
            BackCommand = new RelayCommand(() => NavigateBack?.Invoke());

            LoadHealthInfoAsync();
        }

        // Properties
        public ObservableCollection<HealthInfo> SmokeEffects
        {
            get => _smokeEffects;
            set => SetProperty(ref _smokeEffects, value);
        }

        public ObservableCollection<HealthInfo> QuitBenefits
        {
            get => _quitBenefits;
            set => SetProperty(ref _quitBenefits, value);
        }

        public ObservableCollection<HealthInfo> HealthTips
        {
            get => _healthTips;
            set => SetProperty(ref _healthTips, value);
        }

        public ObservableCollection<HealthTrackingOverview> HealthProgress
        {
            get => _healthProgress;
            set => SetProperty(ref _healthProgress, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand BackCommand { get; }

        // Methods
        private async void LoadHealthInfoAsync()
        {
            try
            {
                IsLoading = true;

                var allHealthInfo = await _healthInfoRepository.GetActiveHealthInfoAsync();

                SmokeEffects = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "SmokeEffects"));

                QuitBenefits = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "QuitBenefits"));

                HealthTips = new ObservableCollection<HealthInfo>(
                    allHealthInfo.Where(h => h.InfoType == "Tips"));

                // If no data from repository, load sample data
                if (!SmokeEffects.Any() && !QuitBenefits.Any() && !HealthTips.Any())
                {
                    LoadSampleData();
                }
            }
            catch
            {
                // Load sample data on error
                LoadSampleData();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RefreshHealthInfoAsync()
        {
            LoadHealthInfoAsync();
        }

        private void LoadSampleData()
        {
            // Sample smoking effects data
            SmokeEffects = new ObservableCollection<HealthInfo>
            {
                new HealthInfo { Title = "Ung thư phổi", Content = "Thuốc lá là nguyên nhân hàng đầu gây ung thư phổi. Nguy cơ mắc bệnh tăng 15-30 lần ở người hút thuốc." },
                new HealthInfo { Title = "Bệnh tim mạch", Content = "Nicotine làm tăng huyết áp và nhịp tim, gây căng thẳng cho hệ tim mạch và tăng nguy cơ đau tim." },
                new HealthInfo { Title = "Lão hóa da", Content = "Thuốc lá làm giảm lưu thông máu đến da, gây nhăn da sớm và làm da xỉn màu, mất độ đàn hồi." },
                new HealthInfo { Title = "Giảm khả năng sinh sản", Content = "Thuốc lá ảnh hưởng đến khả năng sinh sản ở cả nam và nữ, gây khó thụ thai và ảnh hưởng đến thai nhi." }
            };

            // Sample quit benefits data
            QuitBenefits = new ObservableCollection<HealthInfo>
            {
                new HealthInfo { Title = "Cải thiện hô hấp", Content = "Sau 24 giờ không hút thuốc, phổi bắt đầu thải độc tố. Sau 1 tuần, khả năng hô hấp cải thiện đáng kể." },
                new HealthInfo { Title = "Giảm nguy cơ bệnh tim", Content = "Sau 1 năm cai thuốc, nguy cơ mắc bệnh tim giảm 50%. Sau 5 năm, nguy cơ đột quỵ giảm xuống mức bình thường." },
                new HealthInfo { Title = "Tiết kiệm tiền bạc", Content = "Một người hút 1 bao thuốc/ngày có thể tiết kiệm hơn 18 triệu đồng mỗi năm khi cai thuốc." },
                new HealthInfo { Title = "Cải thiện ngoại hình", Content = "Da trở nên sáng khỏe hơn, răng trắng hơn, hơi thở thơm tho, không còn mùi thuốc lá khó chịu." }
            };

            // Sample health tips data
            HealthTips = new ObservableCollection<HealthInfo>
            {
                new HealthInfo { Title = "Uống nhiều nước", Content = "Uống ít nhất 8 ly nước mỗi ngày giúp thải độc tố nhanh hơn và giảm cảm giác thèm thuốc." },
                new HealthInfo { Title = "Tập thể dục đều đặn", Content = "Hoạt động thể chất giúp giải tỏa căng thẳng, cải thiện tâm trạng và giảm cảm giác thèm thuốc." },
                new HealthInfo { Title = "Ăn nhiều trái cây", Content = "Trái cây chứa vitamin C giúp phục hồi hệ miễn dịch và chống oxy hóa, hỗ trợ quá trình cai thuốc." },
                new HealthInfo { Title = "Tìm hoạt động thay thế", Content = "Khi có cảm giác thèm thuốc, hãy đánh răng, nhai kẹo không đường, hoặc làm điều gì đó bằng tay." }
            };
        }
    }
}
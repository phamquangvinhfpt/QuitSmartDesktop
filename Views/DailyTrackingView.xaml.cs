using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for DailyTrackingView.xaml
    /// </summary>
    public partial class DailyTrackingView : UserControl
    {
        public DailyTrackingView()
        {
            InitializeComponent();
        }

        public DailyTrackingView(DailyTrackingViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

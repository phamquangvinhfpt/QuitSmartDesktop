using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for DailyTrackingView.xaml
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

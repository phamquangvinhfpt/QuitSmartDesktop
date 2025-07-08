using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for HealthInfoView.xaml
    public partial class HealthInfoView : UserControl
    {
        public HealthInfoView()
        {
            InitializeComponent();
        }

        public HealthInfoView(HealthInfoViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

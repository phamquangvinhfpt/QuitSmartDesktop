using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for HealthInfoView.xaml
    /// </summary>
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

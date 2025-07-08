using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for UserDashboardView.xaml
    public partial class UserDashboardView : UserControl
    {
        public UserDashboardView()
        {
            InitializeComponent();
        }

        public UserDashboardView(UserDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

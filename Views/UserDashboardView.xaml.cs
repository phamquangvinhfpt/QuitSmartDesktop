using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for UserDashboardView.xaml
    /// </summary>
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

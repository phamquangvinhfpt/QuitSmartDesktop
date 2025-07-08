using System.Windows.Controls;
using QuitSmartApp.ViewModels;

namespace QuitSmartApp.Views
{
    public partial class UserLogsView : UserControl
    {
        public UserLogsView()
        {
            InitializeComponent();
        }

        public UserLogsView(AdminDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
using System.Windows.Controls;
using QuitSmartApp.ViewModels;

namespace QuitSmartApp.Views
{
    public partial class UserDetailsView : UserControl
    {
        public UserDetailsView()
        {
            InitializeComponent();
        }

        public UserDetailsView(AdminDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
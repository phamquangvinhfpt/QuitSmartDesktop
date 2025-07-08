using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for AdminDashboardView.xaml
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }

        public AdminDashboardView(AdminDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

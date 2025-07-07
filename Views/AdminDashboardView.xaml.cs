using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for AdminDashboardView.xaml
    /// </summary>
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

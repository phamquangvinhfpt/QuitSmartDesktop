using System.Windows.Controls;
using QuitSmartApp.ViewModels;

namespace QuitSmartApp.Views
{
    public partial class EditUserView : UserControl
    {
        public EditUserView()
        {
            InitializeComponent();
        }

        public EditUserView(AdminDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
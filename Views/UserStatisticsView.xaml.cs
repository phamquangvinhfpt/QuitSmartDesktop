using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    public partial class UserStatisticsView : UserControl
    {
        public UserStatisticsView()
        {
            InitializeComponent();
        }

        public UserStatisticsView(UserStatisticsViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
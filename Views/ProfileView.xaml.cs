using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for ProfileView.xaml
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        public ProfileView(ProfileViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

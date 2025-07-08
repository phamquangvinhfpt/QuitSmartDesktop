using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for ChangePasswordView.xaml
    public partial class ChangePasswordView : UserControl
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        public ChangePasswordView(ChangePasswordViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
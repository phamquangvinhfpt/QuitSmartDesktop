using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for LoginView.xaml
    public partial class LoginView : UserControl
    {
        private LoginViewModel? _viewModel;

        public LoginView()
        {
            InitializeComponent();
        }

        public LoginView(LoginViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = viewModel;

            // Handle PasswordBox binding manually
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (_viewModel != null)
                    _viewModel.Password = PasswordBox.Password;
            };
        }
    }
}

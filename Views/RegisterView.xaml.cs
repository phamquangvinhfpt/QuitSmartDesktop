using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        private RegisterViewModel? _viewModel;

        public RegisterView()
        {
            InitializeComponent();
        }

        public RegisterView(RegisterViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = viewModel;

            // Handle PasswordBox binding manually
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (_viewModel != null)
                    _viewModel.Password = PasswordBox.Password;
            };

            ConfirmPasswordBox.PasswordChanged += (s, e) =>
            {
                if (_viewModel != null)
                    _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            };
        }
    }
}

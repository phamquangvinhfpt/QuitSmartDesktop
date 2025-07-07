using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for GuestView.xaml
    /// </summary>
    public partial class GuestView : UserControl
    {
        public GuestView()
        {
            InitializeComponent();
        }

        public GuestView(GuestViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

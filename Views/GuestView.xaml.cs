using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for GuestView.xaml
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

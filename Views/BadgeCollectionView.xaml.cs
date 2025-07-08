using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    // Interaction logic for BadgeCollectionView.xaml
    public partial class BadgeCollectionView : UserControl
    {
        public BadgeCollectionView()
        {
            InitializeComponent();
        }

        public BadgeCollectionView(BadgeCollectionViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

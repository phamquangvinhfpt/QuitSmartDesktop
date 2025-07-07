using QuitSmartApp.ViewModels;
using System.Windows.Controls;

namespace QuitSmartApp.Views
{
    /// <summary>
    /// Interaction logic for BadgeCollectionView.xaml
    /// </summary>
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

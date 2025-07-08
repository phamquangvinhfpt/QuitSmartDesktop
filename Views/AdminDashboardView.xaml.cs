using QuitSmartApp.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace QuitSmartApp.Views
{
    // Simple converter for chart bar width calculation
    public class ChartBarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val = System.Convert.ToDouble(value);
                // Scale the value for visual representation (multiply by 3 for better visibility)
                return Math.Max(10, val * 3);
            }
            catch
            {
                return 10.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for pie chart slices
    public class PieSliceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val = System.Convert.ToDouble(value);
                // Convert percentage to angle for pie slice
                return val * 3.6; // 360 degrees / 100 percent = 3.6
            }
            catch
            {
                return 0.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Interaction logic for AdminDashboardView.xaml
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }

        public AdminDashboardView(AdminDashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

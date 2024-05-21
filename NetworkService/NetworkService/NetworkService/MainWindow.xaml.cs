using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowViewModel = new MainWindowViewModel(this);
            DataContext = _mainWindowViewModel;
        }

        private void EntityButton_Click(object sender, RoutedEventArgs e)
        {
            Fa5_User.Foreground = (SolidColorBrush)Application.Current.FindResource("BackgroundS");
            Fa5_ChartBar.Foreground = (SolidColorBrush)Application.Current.FindResource("BackgroundP");
        }

        private void MeasurementButton_Click(object sender, RoutedEventArgs e)
        {
            Fa5_User.Foreground = (SolidColorBrush)Application.Current.FindResource("BackgroundP");
            Fa5_ChartBar.Foreground = (SolidColorBrush)Application.Current.FindResource("BackgroundS");
        }
    }
}

using NetworkService.Model;
using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for NetworkEntity.xaml
    /// </summary>
    public partial class NetworkEntity : UserControl
    {
        private NetworkEntityViewModel _networkEntityViewModel;
        public NetworkEntity()
        {
            InitializeComponent();
            _networkEntityViewModel = new NetworkEntityViewModel();
            DataContext = _networkEntityViewModel;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _networkEntityViewModel.SelectedItems = new ObservableCollection<Entity>(EntityListView.SelectedItems.Cast<Entity>());
        }
    }
}

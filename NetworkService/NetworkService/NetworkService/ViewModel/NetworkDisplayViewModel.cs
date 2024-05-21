using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public static ObservableCollection<Entity> Interval_Meter_entities { get; set; } = MainWindowViewModel.Interval_Meter_entities;
        public static ObservableCollection<Entity> Smart_Meter_entities { get; set; } = MainWindowViewModel.Smart_Meter_entities;

        public NetworkDisplayViewModel()
        {
            
        }
    }
}

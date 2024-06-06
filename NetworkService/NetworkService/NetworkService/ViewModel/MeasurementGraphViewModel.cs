using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        private string _selectedEntity;
        public string SelectedEntity { get { return _selectedEntity; } set { _selectedEntity = value; OnPropertyChanged(nameof(SelectedEntity)); } }
        public static ObservableCollection<Entity> MeasurementGraph_entities { get; set; } = NetworkEntityViewModel.NetowrkEntities;
        public static ObservableCollection<DateTime> Timeline_Values { get; set; } = new ObservableCollection<DateTime>();


        private double _ellipse1 = 0;
        private string _ellipse1Color1 = "White";
        private double _ellipse2 = 0;
        private string _ellipse1Color2 = "White";
        private double _ellipse3 = 0;
        private string _ellipse1Color3 = "White";
        private double _ellipse4 = 0;
        private string _ellipse1Color4 = "White";
        private double _ellipse5 = 0;
        private string _ellipse1Color5 = "White";

        private string _timeLine_TextBox1;
        private string _timeLine_TextBox2;
        private string _timeLine_TextBox3;
        private string _timeLine_TextBox4;
        private string _timeLine_TextBox5;



        public double Ellipse1 { get { return _ellipse1; } set { _ellipse1 = value; OnPropertyChanged(nameof(Ellipse1)); } }
        public string Ellipse1Color1 { get { return _ellipse1Color1; } set { _ellipse1Color1 = value; OnPropertyChanged(nameof(Ellipse1Color1)); } }
        public double Ellipse2 { get { return _ellipse2; } set { _ellipse2 = value; OnPropertyChanged(nameof(Ellipse2)); } }
        public string Ellipse1Color2 { get { return _ellipse1Color2; } set { _ellipse1Color2 = value; OnPropertyChanged(nameof(Ellipse1Color2)); } }
        public double Ellipse3 { get { return _ellipse3; } set { _ellipse3 = value; OnPropertyChanged(nameof(Ellipse3)); } }
        public string Ellipse1Color3 { get { return _ellipse1Color3; } set { _ellipse1Color3 = value; OnPropertyChanged(nameof(Ellipse1Color3)); } }
        public double Ellipse4 { get { return _ellipse4; } set { _ellipse4 = value; OnPropertyChanged(nameof(Ellipse4)); } }
        public string Ellipse1Color4 { get { return _ellipse1Color4; } set { _ellipse1Color4 = value; OnPropertyChanged(nameof(Ellipse1Color4)); } }
        public double Ellipse5 { get { return _ellipse5; } set { _ellipse5 = value; OnPropertyChanged(nameof(Ellipse5)); } }
        public string Ellipse1Color5 { get { return _ellipse1Color5; } set { _ellipse1Color5 = value; OnPropertyChanged(nameof(Ellipse1Color5)); } }


        public string TimeLine_TextBox1 { get { return _timeLine_TextBox1; } set { _timeLine_TextBox1 = value; OnPropertyChanged(nameof(TimeLine_TextBox1)); } }
        public string TimeLine_TextBox2 { get { return _timeLine_TextBox2; } set { _timeLine_TextBox2 = value; OnPropertyChanged(nameof(TimeLine_TextBox2)); } }
        public string TimeLine_TextBox3 { get { return _timeLine_TextBox3; } set { _timeLine_TextBox3 = value; OnPropertyChanged(nameof(TimeLine_TextBox3)); } }
        public string TimeLine_TextBox4 { get { return _timeLine_TextBox4; } set { _timeLine_TextBox4 = value; OnPropertyChanged(nameof(TimeLine_TextBox4)); } }
        public string TimeLine_TextBox5 { get { return _timeLine_TextBox5; } set { _timeLine_TextBox5 = value; OnPropertyChanged(nameof(TimeLine_TextBox5)); } }


        private ObservableCollection<double> LastFiveValues { get; set; } = new ObservableCollection<double>();
        public static ObservableCollection<string> LastFiveDateTime { get; set; } = new ObservableCollection<string>();



        public MyICommand ShowEntityCommand { get; private set; }

        public MeasurementGraphViewModel()
        {
            ShowEntityCommand = new MyICommand(ShowEntity);
        }

        public void NotificationShowe()
        {
            if (SelectedEntity == null || SelectedEntity.Equals(""))
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No selected entity for showing!", NotificationType.Error));
                return;
            }
        }

        public void ShowEntity()
        {
            if (SelectedEntity == null || SelectedEntity.Equals(""))
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No selected entity for showing!", NotificationType.Error));
                return;
            }
            foreach (var entity in MeasurementGraph_entities)
            {
                string id = "";
                if (SelectedEntity != null)
                {
                    id = SelectedEntity.Split(':')[1].TrimStart();
                    id = id.Split(' ')[0].TrimStart();
                }
                if (entity.Id.ToString().Equals(id))
                {
                    UpdateValues(entity.Id); 

                    if (LastFiveValues[0] / 22.7 < 0.34 || LastFiveValues[0] / 22.7 > 2.73)
                    {
                        Ellipse1Color1 = "Red";
                    }
                    else
                    {
                        Ellipse1Color1 = "White";
                    }
                    Ellipse1 = LastFiveValues[0];

                    if (LastFiveValues[1] / 22.7 < 0.34 || LastFiveValues[1] / 22.7 > 2.73)
                    {
                        Ellipse1Color2 = "Red";
                    }
                    else
                    {
                        Ellipse1Color2 = "White";
                    }
                    Ellipse2 = LastFiveValues[1];

                    if (LastFiveValues[2] / 22.7 < 0.34 || LastFiveValues[2] / 22.7 > 2.73)
                    {
                        Ellipse1Color3 = "Red";
                    }
                    else
                    {
                        Ellipse1Color3 = "White";
                    }
                    Ellipse3 = LastFiveValues[2];

                    if (LastFiveValues[3] / 22.7 < 0.34 || LastFiveValues[3] / 22.7 > 2.73)
                    {
                        Ellipse1Color4 = "Red";
                    }
                    else
                    {
                        Ellipse1Color4 = "White";
                    }
                    Ellipse4 = LastFiveValues[3];

                    if (LastFiveValues[4] / 22.7 < 0.34 || LastFiveValues[4] / 22.7 > 2.73)
                    {
                        Ellipse1Color5 = "Red";
                    }
                    else
                    {
                        Ellipse1Color5 = "White";
                    }
                    Ellipse5 = LastFiveValues[4];


                    if (LastFiveDateTime[0] != DateTime.MinValue.ToString())
                        TimeLine_TextBox1 = LastFiveDateTime[0];
                    else
                        TimeLine_TextBox1 = "";
                    if (LastFiveDateTime[1] != DateTime.MinValue.ToString())
                        TimeLine_TextBox2 = LastFiveDateTime[1];
                    else
                        TimeLine_TextBox2 = "";
                    if (LastFiveDateTime[2] != DateTime.MinValue.ToString())
                        TimeLine_TextBox3 = LastFiveDateTime[2];
                    else
                        TimeLine_TextBox3 = "";
                    if (LastFiveDateTime[3] != DateTime.MinValue.ToString())
                        TimeLine_TextBox4 = LastFiveDateTime[3];
                    else
                        TimeLine_TextBox4 = "";
                    if (LastFiveDateTime[4] != DateTime.MinValue.ToString())
                        TimeLine_TextBox5 = LastFiveDateTime[4];
                    else
                        TimeLine_TextBox5 = "";
                }
            }
        }
        public void UpdateEntity()
        {
            foreach (var entity in MeasurementGraph_entities)
            {
                string id = "";
                if (SelectedEntity != null)
                {
                    id = SelectedEntity.Split(':')[1].TrimStart();
                    id = id.Split(' ')[0].TrimStart();
                }
                if (entity.Id.ToString().Equals(id))
                {
                    UpdateValues(entity.Id); 

                    if (LastFiveValues[0] / 22.7 < 0.34 || LastFiveValues[0] / 22.7 > 2.73)
                    {
                        Ellipse1Color1 = "Red";
                    }
                    else
                    {
                        Ellipse1Color1 = "White";
                    }
                    Ellipse1 = LastFiveValues[0];

                    if (LastFiveValues[1] / 22.7 < 0.34 || LastFiveValues[1] / 22.7 > 2.73)
                    {
                        Ellipse1Color2 = "Red";
                    }
                    else
                    {
                        Ellipse1Color2 = "White";
                    }
                    Ellipse2 = LastFiveValues[1];

                    if (LastFiveValues[2] / 22.7 < 0.34 || LastFiveValues[2] / 22.7 > 2.73)
                    {
                        Ellipse1Color3 = "Red";
                    }
                    else
                    {
                        Ellipse1Color3 = "White";
                    }
                    Ellipse3 = LastFiveValues[2];

                    if (LastFiveValues[3] / 22.7 < 0.34 || LastFiveValues[3] / 22.7 > 2.73)
                    {
                        Ellipse1Color4 = "Red";
                    }
                    else
                    {
                        Ellipse1Color4 = "White";
                    }
                    Ellipse4 = LastFiveValues[3];

                    if (LastFiveValues[4] / 22.7 < 0.34 || LastFiveValues[4] / 22.7 > 2.73)
                    {
                        Ellipse1Color5 = "Red";
                    }
                    else
                    {
                        Ellipse1Color5 = "White";
                    }
                    Ellipse5 = LastFiveValues[4];


                    if (LastFiveDateTime[0] != DateTime.MinValue.ToString())
                        TimeLine_TextBox1 = LastFiveDateTime[0];
                    if (LastFiveDateTime[1] != DateTime.MinValue.ToString())
                        TimeLine_TextBox2 = LastFiveDateTime[1];
                    if (LastFiveDateTime[2] != DateTime.MinValue.ToString())
                        TimeLine_TextBox3 = LastFiveDateTime[2];
                    if (LastFiveDateTime[3] != DateTime.MinValue.ToString())
                        TimeLine_TextBox4 = LastFiveDateTime[3];
                    if (LastFiveDateTime[4] != DateTime.MinValue.ToString())
                        TimeLine_TextBox5 = LastFiveDateTime[4];
                }
            }
        }

        public void UpdateValues(int id)
        {
            foreach (var entity in MeasurementGraph_entities)
            {
                if (entity.Id == id)
                {
                    LastFiveValues.Clear();
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);
                    LastFiveValues.Add(0);

                    LastFiveDateTime.Clear();
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());
                    LastFiveDateTime.Add(DateTime.MinValue.ToString());

                    int i = entity.Value.Count - 1;
                    for (int j = 4; j >= 0; j--)
                    {
                        LastFiveValues[j] = entity.Value[i] * 22.7;
                        i--;
                    }

                    i = entity.TimelineValues.Count - 1;
                    for (int j = 4; j >= 0; j--)
                    {
                        LastFiveDateTime[j] = entity.TimelineValues[i];
                        i--;
                    }
                }
            }
        }
    }
}

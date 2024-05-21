using NetworkService.Model;
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


        private ObservableCollection<double> LastFiveValues { get; set; } = new ObservableCollection<double>();



        public MyICommand ShowEntityCommand { get; private set; }

        public MeasurementGraphViewModel()
        {
            ShowEntityCommand = new MyICommand(ShowEntity);
        }

        public void ShowEntity()
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

                    //int startIndex = Math.Max(0, entity.Value.Count - 5); // Calculate the starting index for the last five values

                    // Add zeros to pad the list if there are fewer than five values
                    //int count = entity.Value.Count;

                    /*int j = 4;
                    for (int i = entity.Value.Count - 1; i <= 0; i--)
                    {

                        LastFiveValues[j--] = entity.Value[i] * 22.7;
                    }*/

                    int j = 4;
                    int i = entity.Value.Count - 1;
                    while (true)
                    {
                        if (j < 0)
                        {
                            break;
                        }
                        else
                        {
                            LastFiveValues[j] = entity.Value[i] * 22.7;
                            j--;
                            i--;
                        }
                    }
                }
            }
        }
    }
}

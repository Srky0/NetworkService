using NetworkService.Model;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
         // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public static ObservableCollection<Entity> entities { get; set; } = new ObservableCollection<Entity>();
        public static ObservableCollection<Entity> Filter_entities { get; set; } = new ObservableCollection<Entity>();
        public static ObservableCollection<Entity> Interval_Meter_entities { get; set; } = new ObservableCollection<Entity>();
        public static ObservableCollection<Entity> Smart_Meter_entities { get; set; } = new ObservableCollection<Entity>();

        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> ExitWindowCommand { get; private set; }

        public NetworkDisplayViewModel networkDisplay;
        public NetworkEntityViewModel networkEntity;
        public MeasurementGraphViewModel measurementGraph;
        public MainWindow window;

        private BindableBase currentViewModel;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            createListener(); //Povezivanje sa serverskom aplikacijom


            NavCommand = new MyICommand<string>(OnNav);
            ExitWindowCommand = new MyICommand<Window>(CloseWindow);


            window = mainWindow;
            networkDisplay = new NetworkDisplayViewModel();
            networkEntity = new NetworkEntityViewModel();
            measurementGraph = new MeasurementGraphViewModel();
            CurrentViewModel = networkEntity;
        }


        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "networkEntity":
                    CurrentViewModel = networkEntity;
                    
                    //window.MeasurementButton.IsEnabled = true;
                    //window.EntityButton.IsEnabled = false;
                    break;
                case "measurementGraph":
                    CurrentViewModel = measurementGraph;
                    //window.EntityButton.IsEnabled = true;
                    //window.MeasurementButton.IsEnabled = false;
                    break;
            }
        }

        private void CloseWindow(Window MainWindow)
        {
            //networkEntityViewModel.SaveDataAsXML();
            MainWindow.Close();
        }

        private void createListener()
        {
            File.Create("Log.txt");
            var tcp = new TcpListener(IPAddress.Any, 25657);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntityViewModel.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            //Console.WriteLine(NetworkEntityViewModel.NetowrkEntities.Count.ToString());
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            if (NetworkEntityViewModel.NetowrkEntities.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                //DateTime dt = DateTime.Now;
                                //using (StreamWriter sw = File.AppendText("Log.txt"))
                                    //sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);

                                int id = Int32.Parse(splited[0].Split('_')[1]);
                                NetworkEntityViewModel.NetowrkEntities[id].LastValue = Math.Round(Double.Parse(splited[1]), 2);
                                NetworkEntityViewModel.NetowrkEntities[id].Value.Add(Math.Round(Double.Parse(splited[1]), 2));
                                NetworkEntityViewModel.NetowrkEntities[id].TimelineValues.Add(DateTime.Now.ToString());

                                UpdateCanvasValue(id);

                                //Console.WriteLine(splited);
                                //OnPropertyChanged(nameof(NetworkEntityViewModel.NetowrkEntities));
                                //OnPropertyChanged(nameof(measurementGraph));

                                //NetworkDisplayViewModel.UpdateEntityOnCanvas(MainWindowViewModel.entities[id]);
                                //NetworkDisplayViewModel.AutoShow();
                                measurementGraph.UpdateValues(id);
                                measurementGraph.ShowEntity();
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }


        public static void UpdateCanvasValue(int id)
        {
            int tmpIndex = 0;
            foreach (var tmp in NetworkDisplayViewModel.IDCanvasCollection)
            {
                if (tmp.Equals(id.ToString()))
                {
                    NetworkDisplayViewModel.ValueCanvasCollection[tmpIndex] = NetworkEntityViewModel.NetowrkEntities[id].LastValue.ToString();
                }
                tmpIndex++;
            }
        }

    }
}

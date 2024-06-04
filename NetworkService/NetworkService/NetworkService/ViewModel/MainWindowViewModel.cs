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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
         // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public static ObservableCollection<Entity> entities { get; set; } = new ObservableCollection<Entity>();
        public static ObservableCollection<Entity> Filter_entities { get; set; } = new ObservableCollection<Entity>();

        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> ExitWindowCommand { get; private set; }

        public NetworkDisplay networkDisplay;
        public NetworkEntity networkEntity;
        public MeasurementGraph measurementGraph;
        public MainWindow window;

        private UserControl currentView;
        private UserControl display;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            createListener(); //Povezivanje sa serverskom aplikacijom


            NavCommand = new MyICommand<string>(OnNav);
            ExitWindowCommand = new MyICommand<Window>(CloseWindow);


            window = mainWindow;
            networkDisplay = new NetworkDisplay(this);
            networkEntity = new NetworkEntity(networkDisplay);
            measurementGraph = new MeasurementGraph();
            Display = networkDisplay;
            CurrentView = networkEntity;
        }


        public UserControl CurrentView
        {
            get { return currentView; }
            set
            {
                SetProperty(ref currentView, value);
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        public UserControl Display
        {
            get { return display; }
            set
            {
                SetProperty(ref display, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "networkEntity":
                    CurrentView = networkEntity;
                    break;
                case "measurementGraph":
                    CurrentView = measurementGraph;
                    break;
            }
        }

        private void CloseWindow(Window MainWindow)
        {
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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntityViewModel.NetowrkEntities.Count.ToString());
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

                            if (entities.Count > 0)
                            {
                                var splited = incomming.Split(':');
                                //DateTime dt = DateTime.Now;
                                //using (StreamWriter sw = File.AppendText("Log.txt"))
                                    //sw.WriteLine(dt + "; " + splited[0] + ", " + splited[1]);

                                int id = Int32.Parse(splited[0].Split('_')[1]);
                                entities[id].LastValue = Math.Round(Double.Parse(splited[1]), 2);
                                entities[id].Value.Add(Math.Round(Double.Parse(splited[1]), 2));
                                entities[id].TimelineValues.Add(DateTime.Now.ToString());

                                //prodji kkroz canvas Id
                                /*foreach (var item in NetworkDisplayViewModel.IDCanvasCollection)
                                {
                                    if(item.Equals(id.ToString()))
                                    {
                                        NetworkDisplayViewModel.ValueCanvasCollection[Int32.Parse(item)] = entities[id].LastValue.ToString();
                                    }
                                }    */


                                for(int j = 0; j < NetworkDisplayViewModel.IDCanvasCollection.Count(); j++)
                                {
                                    if (NetworkDisplayViewModel.IDCanvasCollection[j].Equals(entities[id].Id.ToString()))
                                    {
                                        NetworkDisplayViewModel.ValueCanvasCollection[j] = entities[id].LastValue.ToString();
                                    }
                                }


                                //UpdateCanvasValue(id);
                                //int index = networkEntity._networkEntityViewModel.GetCanvasIndexForEntityId(id);
                                //UpdateCanvasValue(index);

                                //Console.WriteLine(splited);
                                //OnPropertyChanged(nameof(NetworkEntityViewModel.NetowrkEntities));
                                //OnPropertyChanged(nameof(measurementGraph));

                                //NetworkDisplayViewModel.UpdateEntityOnCanvas(MainWindowViewModel.entities[id]);
                                //NetworkDisplayViewModel.AutoShow();
                                measurementGraph._measurementGraph.UpdateValues(id);
                                measurementGraph._measurementGraph.ShowEntity();
                            }

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }


        public void UpdateCanvasValue(int id)
        {
            foreach (var tmp in NetworkDisplayViewModel.IDCanvasCollection)
            {
                int canvasId = networkEntity._networkEntityViewModel.GetCanvasIndexForEntityId(id);
                if (tmp.Equals(canvasId.ToString()))
                {
                    try
                    {
                        NetworkDisplayViewModel.ValueCanvasCollection[canvasId] = NetworkEntityViewModel.NetowrkEntities[id].LastValue.ToString();
                        if(NetworkEntityViewModel.NetowrkEntities[id].LastValue > 0.34 && NetworkEntityViewModel.NetowrkEntities[id].LastValue < 2.73)
                        {

                        }else
                        {

                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
    }
}

using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private static ObservableCollection<string> _iDCanvasCollection = new ObservableCollection<string> { "", "", "", "", "", "", "", "", "", "", "", ""};
        private static ObservableCollection<string> _valueCanvasCollection = new ObservableCollection<string> { "", "", "", "", "", "", "", "", "", "", "", ""};

        public static ObservableCollection<Entity> Interval_Meter_entities { get; set; } = MainWindowViewModel.Interval_Meter_entities;
        public static ObservableCollection<Entity> Smart_Meter_entities { get; set; } = MainWindowViewModel.Smart_Meter_entities;
        public static ObservableCollection<Entity> entities { get; set; } = MainWindowViewModel.entities;
        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        public static ObservableCollection<string> IDCanvasCollection { get { return _iDCanvasCollection; } set { _iDCanvasCollection = value; } }
        public static ObservableCollection<string> ValueCanvasCollection { get { return _valueCanvasCollection; } set { _valueCanvasCollection = value; } }

        public MyICommand<object> SelectionChanged_TreeView {  get; set; }
        public MyICommand MouseLeftButtonUp_TreeView { get; set; }
        public MyICommand<object> DropEntityOnCanvas { get; set; }
        public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public MyICommand MouseLeftButtonUpCanvas { get; set; }
        public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }
        public MyICommand<object> FreeCanvas { get; set; }

        private Entity selectedEntity;

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;

       
        public Entity SelectedEntity { get { return selectedEntity; } set { selectedEntity = value; OnPropertyChanged("SelectedEntity"); } }

        public NetworkDisplayViewModel()
        {
            InitializeCanvases();
            MouseLeftButtonUp_TreeView = new MyICommand(OnMouseLeftButtonUp);
            SelectionChanged_TreeView = new MyICommand<object>(OnSelectionChanged);
            DropEntityOnCanvas = new MyICommand<object>(OnDrop);
            MouseLeftButtonUpCanvas = new MyICommand(OnMouseLeftButtonUp);
            LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
            RightMouseButtonDownOnCanvas = new MyICommand<object>(OnRightMouseButtonDown);
            FreeCanvas = new MyICommand<object>(OnFreeCanvas);
        }

        private void InitializeCanvases()
        {
            CanvasCollection = new ObservableCollection<Canvas>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas
                {
                    Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E")),
                    AllowDrop = true
                });
            }
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }
        private void OnSelectionChanged(object selectedItem)
        {
            if (!dragging && selectedItem is Entity selectedEntity)
            {
                dragging = true;
                draggedItem = selectedEntity;
                DragDrop.DoDragDrop(Application.Current.MainWindow, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private void OnRightMouseButtonDown(object entity)
        {
            /*int index = Convert.ToInt32(entity);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (!isLineSourceSelected)
                {
                    sourceCanvasIndex = index;

                    //linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

                    *//*currentLine.X1 = linePoint1.X;
                    currentLine.Y1 = linePoint1.Y;
                    currentLine.Source = sourceCanvasIndex;*//*

                    isLineSourceSelected = true;
                }
                else
                {
                    destinationCanvasIndex = index;

                    if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
                    {*//*
                        linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

                        currentLine.X2 = linePoint2.X;
                        currentLine.Y2 = linePoint2.Y;
                        currentLine.Destination = destinationCanvasIndex;*/

                        /*LineCollection.Add(new MyLine
                        {
                            X1 = currentLine.X1,
                            Y1 = currentLine.Y1,
                            X2 = currentLine.X2,
                            Y2 = currentLine.Y2,
                            Source = currentLine.Source,
                            Destination = currentLine.Destination
                        });

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();*//*
                    }
                    else
                    {
                        // Pocetak i kraj linije su u istom canvasu

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new MyLine();
                    }
                }
            }
            else
            {
                // Canvas na koji se postavlja tacka nije zauzet

                isLineSourceSelected = false;

                linePoint1 = new Point();
                linePoint2 = new Point();
                currentLine = new MyLine();
            }*/
        }

        private void OnLeftMouseButtonDown(object entity)
        {
            if (!dragging)
            {
                int index = Convert.ToInt32(entity);

                if (CanvasCollection[index].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (Entity)(CanvasCollection[index].Resources["data"]);
                    draggingSourceIndex = index;
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void OnDrop(object entity)
        {
            bool intervalMeter = false;
            if (draggedItem != null)
            {
                int index = Convert.ToInt32(entity);

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    IDCanvasCollection[index] = draggedItem.Id.ToString();
                    ValueCanvasCollection[index] = draggedItem.LastValue.ToString();

                    MainWindowViewModel.UpdateCanvasValue(draggedItem.Id);

                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    if (draggedItem.Type.ToString() == "Interval_Meter")
                    {
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/Interval_Meter.jpg");
                        intervalMeter = true;
                    }
                    else
                    {
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/Smart_Meter.jpg");
                    }
                    logo.EndInit();

                    CanvasCollection[index].Background = new ImageBrush(logo);
                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", draggedItem);
                    //BorderBrushCollection[index] = (draggedItem.IsValueValidForType()) ? Brushes.Green : Brushes.Red;

                    // PREVLACENJE IZ DRUGOG CANVASA
                    if (draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");
                        IDCanvasCollection[draggingSourceIndex] = "";
                        ValueCanvasCollection[draggingSourceIndex] = "";
                        //BorderBrushCollection[draggingSourceIndex] = Brushes.DarkGray;

                        //UpdateLinesForCanvas(draggingSourceIndex, index);

                        // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet pomeren na drugo polje
                        /*if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new MyLine();
                        }*/

                        draggingSourceIndex = -1;
                    }
                    if (intervalMeter)
                    {
                        Interval_Meter_entities.Remove(draggedItem);
                    }
                    else
                    {
                        Smart_Meter_entities.Remove(draggedItem);
                    }
                }
            }
        }

        private void OnFreeCanvas(object entity)
        {
            int index = Convert.ToInt32(entity);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet uklonjen sa canvas-a
                /*if (sourceCanvasIndex != -1)
                {
                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    currentLine = new MyLine();
                }*/

                //DeleteLinesForCanvas(index);
                Entity tmpEntity = (Entity)CanvasCollection[index].Resources["data"];
                if (tmpEntity.Type.ToString().Equals("Interval_Meter"))
                    Interval_Meter_entities.Add(tmpEntity);
                else
                    Smart_Meter_entities.Add(tmpEntity);
                CanvasCollection[index].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                CanvasCollection[index].Resources.Remove("taken");
                CanvasCollection[index].Resources.Remove("data");
                IDCanvasCollection[index] = "";
                ValueCanvasCollection[index] = "";
                //BorderBrushCollection[index] = Brushes.DarkGray;
            }
        }

    }
}

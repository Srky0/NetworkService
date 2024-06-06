using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Line = NetworkService.Model.Line;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private static ObservableCollection<string> _iDCanvasCollection = new ObservableCollection<string> { "", "", "", "", "", "", "", "", "", "", "", ""};
        private static ObservableCollection<string> _valueCanvasCollection = new ObservableCollection<string> { "", "", "", "", "", "", "", "", "", "", "", ""};
        private static ObservableCollection<string> _borderBrushValues = new ObservableCollection<string> { "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D", "#A09D9D" };

        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        public static ObservableCollection<string> IDCanvasCollection { get { return _iDCanvasCollection; } set { _iDCanvasCollection = value; } }
        public static ObservableCollection<string> ValueCanvasCollection { get { return _valueCanvasCollection; } set { _valueCanvasCollection = value; } }
        public static ObservableCollection<string> BorderBrushValues { get { return _borderBrushValues; } set { _borderBrushValues = value; } }

        public MyICommand<object> SelectionChanged_TreeView {  get; set; }
        public MyICommand MouseLeftButtonUp_TreeView { get; set; }
        public MyICommand<object> DropEntityOnCanvas { get; set; }
        public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public MyICommand MouseLeftButtonUpCanvas { get; set; }
        public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }
        public MyICommand<object> FreeCanvas { get; set; }
        public MyICommand OrganizeAllCommand { get; set; }

        public ObservableCollection<Line> LineCollection { get; set; }

        private Entity _selectedEntity;

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;
        private Line currentLine = new Line();
        private Point linePoint1 = new Point();
        private Point linePoint2 = new Point();


        public Entity SelectedEntity { get { return _selectedEntity; } set { SetProperty(ref _selectedEntity, value); } }


        private ObservableCollection<EntityNode> _displayNodes;
        public ObservableCollection<EntityNode> DisplayNodes
        {
            get { return _displayNodes; }
            set { _displayNodes = value; OnPropertyChanged(nameof(DisplayNodes)); }
        }

        public MainWindowViewModel window;

        public NetworkDisplayViewModel(MainWindowViewModel mainWindow)
        {
            InitializeCanvases();
            MouseLeftButtonUp_TreeView = new MyICommand(OnMouseLeftButtonUp);
            SelectionChanged_TreeView = new MyICommand<object>(OnSelectionChanged);
            DropEntityOnCanvas = new MyICommand<object>(OnDrop);
            MouseLeftButtonUpCanvas = new MyICommand(OnMouseLeftButtonUp);
            LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
            RightMouseButtonDownOnCanvas = new MyICommand<object>(OnRightMouseButtonDown);
            FreeCanvas = new MyICommand<object>(OnFreeCanvas);
            LineCollection = new ObservableCollection<Line>();
            OrganizeAllCommand = new MyICommand(onOrganize);

            window = mainWindow;


            GenerateDisplayNodes();
        }

        public void OnFiledsFreeCanvas(int index)
        {
            CanvasCollection[index].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
            CanvasCollection[index].Resources.Remove("taken");
            CanvasCollection[index].Resources.Remove("data");
            IDCanvasCollection[index] = "";
            ValueCanvasCollection[index] = "";
            BorderBrushValues[index] = "#A09D9D";
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

        private void GenerateDisplayNodes()
        {
            DisplayNodes = new ObservableCollection<EntityNode>();
            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
                if(type != Type.All)
                {
                    EntityNode en = new EntityNode(type);
                    foreach (Entity entity in MainWindowViewModel.entities)
                    {
                        if (entity.Type == type)
                        {
                            en.EntitiesSameType.Add(entity);
                        }
                    }
                    DisplayNodes.Add(en);
                }
            }
        }

        public void AddEntityNode(Entity entity)
        {
            foreach (var node in DisplayNodes)
            {
                if (entity.Type.ToString() == node.Type)
                {
                    node.EntitiesSameType.Add(entity);
                }
            }
        }
        public void RemoveEntityNode(Entity entity)
        {
            foreach (var node in DisplayNodes)
            {
                if (entity.Type.ToString() == node.Type)
                {
                    node.EntitiesSameType.Remove(entity);
                }
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
            dragging = false;
            if (!dragging && selectedItem is Entity SelectedEntity)
            {
                dragging = true;
                draggedItem = SelectedEntity;
                if (draggedItem != null)
                    DragDrop.DoDragDrop(Application.Current.MainWindow, draggedItem, DragDropEffects.Move);
            }
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

                    window.UpdateCanvasValue(index);

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
                    if (draggedItem.LastValue > 0.34 && draggedItem.LastValue < 2.7)
                        BorderBrushValues[index] = "#A09D9D";
                    else
                        BorderBrushValues[index] = "Red";

                    // PREVLACENJE IZ DRUGOG CANVASA
                    if (draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");
                        IDCanvasCollection[draggingSourceIndex] = "";
                        ValueCanvasCollection[draggingSourceIndex] = "";
                        BorderBrushValues[draggingSourceIndex] = "#A09D9D";

                        UpdateLinesForCanvas(draggingSourceIndex, index);

                        // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet pomeren na drugo polje
                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new Line();
                        }

                        draggingSourceIndex = -1;
                    }
                    RemoveEntityNode(draggedItem);
                }
            }
            dragging = false;
        }

        #region CanvasLines

        private void OnRightMouseButtonDown(object entity)
        {
            int index = Convert.ToInt32(entity);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (!isLineSourceSelected)
                {
                    sourceCanvasIndex = index;

                    linePoint1 = GetPointForCanvasIndex(sourceCanvasIndex);

                    currentLine.X1 = linePoint1.X;
                    currentLine.Y1 = linePoint1.Y;
                    currentLine.Source = sourceCanvasIndex;

                    isLineSourceSelected = true;
                }
                else
                {
                    destinationCanvasIndex = index;

                    if ((sourceCanvasIndex != destinationCanvasIndex) && !DoesLineAlreadyExist(sourceCanvasIndex, destinationCanvasIndex))
                    {
                        linePoint2 = GetPointForCanvasIndex(destinationCanvasIndex);

                        currentLine.X2 = linePoint2.X;
                        currentLine.Y2 = linePoint2.Y;
                        currentLine.Destination = destinationCanvasIndex;

                        LineCollection.Add(new Line
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
                        currentLine = new Line();
                    }
                    else
                    {
                        // Pocetak i kraj linije su u istom canvasu

                        isLineSourceSelected = false;

                        linePoint1 = new Point();
                        linePoint2 = new Point();
                        currentLine = new Line();
                    }
                }
            }
            else
            {
                // Canvas na koji se postavlja tacka nije zauzet

                isLineSourceSelected = false;

                linePoint1 = new Point();
                linePoint2 = new Point();
                currentLine = new Line();
            }
        }

        private bool DoesLineAlreadyExist(int source, int destination)
        {
            foreach (Line line in LineCollection)
            {
                if ((line.Source == source) && (line.Destination == destination))
                {
                    return true;
                }
                if ((line.Source == destination) && (line.Destination == source))
                {
                    return true;
                }
            }
            return false;
        }

        private Point GetPointForCanvasIndex(int canvasIndex)
        {
            double x = 0, y = 0;

            for (int row = 0; row <= 3; row++)
            {
                for (int col = 0; col <= 2; col++)
                {
                    int currentIndex = row * 3 + col;

                    if (canvasIndex == currentIndex)
                    {
                        x = 85 + (col * 180);
                        y = 95 + (row * 180);

                        break;
                    }
                }
            }
            return new Point(x, y);
        }

        private void UpdateLinesForCanvas(int sourceCanvas, int destinationCanvas)
        {
            for (int i = 0; i < LineCollection.Count; i++)
            {
                if (LineCollection[i].Source == sourceCanvas)
                {
                    Point newSourcePoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X1 = newSourcePoint.X;
                    LineCollection[i].Y1 = newSourcePoint.Y;
                    LineCollection[i].Source = destinationCanvas;
                }
                else if (LineCollection[i].Destination == sourceCanvas)
                {
                    Point newDestinationPoint = GetPointForCanvasIndex(destinationCanvas);
                    LineCollection[i].X2 = newDestinationPoint.X;
                    LineCollection[i].Y2 = newDestinationPoint.Y;
                    LineCollection[i].Destination = destinationCanvas;
                }
            }
        }

        private void OnFreeCanvas(object entity)
        {
            int index = Convert.ToInt32(entity);
            List<Line> line = new List<Line>();

            Messenger.Default.Send<NotificationContent>(MainWindowViewModel.CreateYesNoToastNotification(response =>
            {
                if (response)
                {
                    if (CanvasCollection[index].Resources["taken"] != null)
                    {
                        // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet uklonjen sa canvas-a
                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new Line();
                        }

                        DeleteLinesForCanvas(index);
                        Entity tmpEntity = (Entity)CanvasCollection[index].Resources["data"];
                        AddEntityNode(tmpEntity);
                        CanvasCollection[index].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                        CanvasCollection[index].Resources.Remove("taken");
                        CanvasCollection[index].Resources.Remove("data");
                        IDCanvasCollection[index] = "";
                        ValueCanvasCollection[index] = "";
                        BorderBrushValues[index] = "#A09D9D";
                        foreach (var linija in LineCollection)
                        {
                            if (linija.Destination == GetCanvasIndexForEntityId(tmpEntity.Id) || linija.Source == GetCanvasIndexForEntityId(tmpEntity.Id))
                            {
                                if (!line.Contains(linija))
                                    line.Add(linija);
                            }
                        }

                        foreach (var lineDelete in line)
                        {
                            LineCollection.Remove(lineDelete);
                        }
                    }
                    else
                    {
                        MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No entity in canvas to delete!", NotificationType.Error));
                    }
                }
            }));
        }
        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }
            return -1;
        }

        private void DeleteLinesForCanvas(int canvasIndex)
        {
            List<Line> linesToDelete = new List<Line>();

            for (int i = 0; i < LineCollection.Count; i++)
            {
                if ((LineCollection[i].Source == canvasIndex) || (LineCollection[i].Destination == canvasIndex))
                {
                    linesToDelete.Add(LineCollection[i]);
                }
            }

            foreach (Line line in linesToDelete)
            {
                LineCollection.Remove(line);
            }
        }
        #endregion

        private void onOrganize()
        {
            dragging = false;
            List<Entity> addedEntities = new List<Entity>();
            try
            {
                int index = 0;
                foreach(var node in DisplayNodes)
                {
                    foreach (var item in node.EntitiesSameType)
                    {
                        while (index < CanvasCollection.Count)
                        {
                            if (CanvasCollection[index].Resources != null && CanvasCollection[index].Resources["taken"] == null)
                            {

                                BitmapImage logo = new BitmapImage();
                                logo.BeginInit();
                                if (item.Type.ToString() == "Interval_Meter")
                                {
                                    logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/Interval_Meter.jpg");
                                }
                                else
                                {
                                    logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/Smart_Meter.jpg");
                                }
                                logo.EndInit();

                                CanvasCollection[index].Background = new ImageBrush(logo);
                                CanvasCollection[index].Resources.Add("taken", true);
                                CanvasCollection[index].Resources.Add("data", item);
                                IDCanvasCollection[index] = item.Id.ToString();
                                ValueCanvasCollection[index] = item.LastValue.ToString();
                                if (item.LastValue > 0.34 && item.LastValue < 2.7)
                                    BorderBrushValues[index] = "#A09D9D";
                                else
                                    BorderBrushValues[index] = "Red";

                                addedEntities.Add(item);

                                break;
                            }
                            index++;
                        }
                        index = 0;
                    }
                    if (MainWindowViewModel.entities.Count == 0)
                    {
                        MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No entities to organise!", NotificationType.Error));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            foreach (var entity in addedEntities)
            {
                RemoveEntityNode(entity);
            }
        }

    }
}

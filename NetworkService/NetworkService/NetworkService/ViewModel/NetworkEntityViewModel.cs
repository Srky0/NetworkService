using Game_Client.Helpers;
using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using NetworkService.Views;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkEntityViewModel : BindableBase
    {
        public static int Count { get; set; } = 0;

        private static int _idI = 0;
        private static int _idS = 0;
        private static int _id = 0;
        private string _selecteItemAdd;
        private string _selecteItemFilter;
        private string _selectedTypeFilter;
        private Entity _selectedItemForDelete;
        private string _iDText;
        private bool _isLowerChecked;
        private bool _isEqualsChecked;
        private bool _isHigherChecked;
        private ObservableCollection<Entity> _showedCollection;
        private ObservableCollection<Entity> _selectedItems;
        private List<string> _filterOptions;
        private List<string> _addTypes;

        public List<string> FilterOptions { get { return _filterOptions; } set { _filterOptions = value; OnPropertyChanged(nameof(FilterOptions)); } }
        public List<string> AddType { get { return _addTypes; } set { _addTypes = value; OnPropertyChanged(nameof(AddType)); } }
        public ObservableCollection<Entity> SelectedItems { get { return _selectedItems; } set { _selectedItems = value; OnPropertyChanged(nameof(SelectedItems)); } }
        public ObservableCollection<Entity> ShowedCollection { get { return _showedCollection; } set { _showedCollection = value; OnPropertyChanged(nameof(ShowedCollection)); } }
        public static ObservableCollection<Entity> NetowrkEntities { get; set; } = MainWindowViewModel.entities;
        public static ObservableCollection<Filter> Filters { get; set; } = new ObservableCollection<Filter>();
        public Entity SelectedItemForDelete { get { return _selectedItemForDelete; } set { _selectedItemForDelete = value; OnPropertyChanged(nameof(SelectedItemForDelete)); } }
        public string SelectedItemAdd { get { return _selecteItemAdd; } set { _selecteItemAdd = value; OnPropertyChanged(nameof(SelectedItemAdd)); } }
        public string SelectedTypeFilter { get { return _selectedTypeFilter; } set { _selectedTypeFilter = value; OnPropertyChanged(nameof(SelectedTypeFilter)); } }
        public string SelectedItemFilter { get { return _selecteItemFilter; } set { _selecteItemFilter = value; OnPropertyChanged(nameof(SelectedItemFilter)); } }
        public string IDText { get { return _iDText; } set { _iDText = value; OnPropertyChanged(nameof(IDText)); } }
        public bool IsLowerChecked { get { return _isLowerChecked; } set { if (value) { IsHigherChecked = false; IsEqualsChecked = false; }  _isLowerChecked = value; OnPropertyChanged(nameof(IsLowerChecked)); } }
        public bool IsEqualsChecked { get { return _isEqualsChecked; } set { if (value) { IsLowerChecked = false; IsHigherChecked = false; } _isEqualsChecked = value; OnPropertyChanged(nameof(IsEqualsChecked)); } }
        public bool IsHigherChecked { get { return _isHigherChecked; } set { if (value) { IsLowerChecked = false; IsEqualsChecked = false; } _isHigherChecked = value; OnPropertyChanged(nameof(IsHigherChecked)); } }


        public MyICommand AddCommand { get; private set; }
        public MyICommand DeleteCommand { get; private set; }
        public MyICommand FilterCommand { get; private set; }
        public MyICommand ResetCommand { get; private set; }


        private NetworkDisplay _networkDisplay;

        public NetworkEntityViewModel(NetworkDisplay networkDisplay)
        {
            _networkDisplay = networkDisplay;

            FilterOptions = new List<string> { "All", "Interval_Meter", "Smart_Meter" };

            AddType = new List<string> { "Interval_Meter", "Smart_Meter" };

            AddCommand = new MyICommand(AddEntity);

            DeleteCommand = new MyICommand(DeleteEntity);

            FilterCommand = new MyICommand(FilterEntity);
            
            ResetCommand = new MyICommand(ResetEntity);

            SelectedItemAdd = "Interval_Meter";
            IsEqualsChecked = true;
            Filters.Clear();
            Filters.Add(new Filter());
            SelectedItemFilter = "None"; ;
            SelectedTypeFilter = "All";
            ShowedCollection = NetowrkEntities;
        }

        public void DeleteEntity()
        {
            Messenger.Default.Send<NotificationContent>(MainWindowViewModel.CreateYesNoToastNotification(response =>
            {
                if (response)
                {    
                    List<Line> line = new List<Line>();
                    if (SelectedItems != null && SelectedItems.Any())
                    {
                        foreach (var item in SelectedItems.ToList())
                        {
                            if (MainWindowViewModel.entities.Any(entity => entity.Id == item.Id))
                            {
                                MainWindowViewModel.entities.Remove(item);
                                _networkDisplay._networkDisplayViewModel.RemoveEntityNode(item);


                                foreach(var linija in _networkDisplay._networkDisplayViewModel.LineCollection)
                                {
                                    if(linija.Destination == GetCanvasIndexForEntityId(item.Id) || linija.Source == GetCanvasIndexForEntityId(item.Id))
                                    {
                                        if(!line.Contains(linija))
                                            line.Add(linija);
                                    }
                                }

                                foreach(var lineDelete in line)
                                {
                                    _networkDisplay._networkDisplayViewModel.LineCollection.Remove(lineDelete);
                                }


                                DeleteEntityFromCanvas(item);
                            }
                        }

                        RestartOtherApplication("C:\\Users\\Srky\\Desktop\\PSI IUIS - PZ2 - NetworkService i Metering Simulator\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");
                        ResetEntity();
                        SelectedItems.Clear();
                    }
                    else
                    {
                        MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No selected item/items for delete!", NotificationType.Error));
                    }
                }
            }));
        }

        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < _networkDisplay._networkDisplayViewModel.CanvasCollection.Count; i++)
            {
                Entity entity = (_networkDisplay._networkDisplayViewModel.CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }
            return -1;
        }

        public void DeleteEntityFromCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Resources.Remove("taken");
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Resources.Remove("data");
                _networkDisplay._networkDisplayViewModel.CanvasCollection[canvasIndex].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3E3E3E"));
                _networkDisplay._networkDisplayViewModel.OnFiledsFreeCanvas(canvasIndex);
            }
        }

        public void AddEntityShortcut(string type)
        {
            SelectedItemAdd = type;
            AddEntity();
        }

        private void AddEntity()
        {
            string type = "";
            
            type = SelectedItemAdd;

            if(type == null)
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No selected type of entity to add new entity!", NotificationType.Error));
                return;
            }

            if (type.Equals("Interval_Meter"))
            {
                string name = $"Interval_Meter {_idI}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                Entity newEntity = new Entity(_id, name, Type.Interval_Meter, values);
                MainWindowViewModel.entities.Add(newEntity);
                _networkDisplay._networkDisplayViewModel.AddEntityNode(newEntity);
                Count = MainWindowViewModel.entities.Count;
                _idI++;
                _id++;
                RestartOtherApplication("C:\\Users\\Srky\\Desktop\\PSI IUIS - PZ2 - NetworkService i Metering Simulator\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");
                ResetEntity();
            }
            else if (type.Equals("Smart_Meter"))
            {
                string name = $"Smart_Meter {_idS}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                Entity newEntity = new Entity(_id, name, Type.Smart_Meter, values);
                MainWindowViewModel.entities.Add(newEntity);
                _networkDisplay._networkDisplayViewModel.AddEntityNode(newEntity);
                Count = MainWindowViewModel.entities.Count;
                _idS++;
                _id++;
                RestartOtherApplication("C:\\Users\\Srky\\Desktop\\PSI IUIS - PZ2 - NetworkService i Metering Simulator\\MeteringSimulator\\MeteringSimulator\\bin\\Debug\\MeteringSimulator.exe");
                ResetEntity();
            }
            else
            {
                MainWindowViewModel.ShowToastNotification(new ToastNotification("Error", "No selected type of entity to add new entity!", NotificationType.Error));
            }
        }

        private void FilterEntity()
        {
            string type = "";
            MainWindowViewModel.Filter_entities.Clear();
            if (IDText == null)
            {
                IDText = "";
            }
            if ((!SelectedItemFilter.Equals("None")) && (!SelectedItemFilter.Equals("")))
            {
                foreach (var item in Filters)
                {
                    if (item.Name.Equals(SelectedItemFilter))
                    {
                        int idSaved = item.ID;
                        bool LowerSaved = item.IsLowerChecked;
                        bool EqualsSaved = item.IsEqualsChecked;
                        bool HigherSaved = item.IsHigherChecked;
                        string typeSaved = item.Type.ToString();
                        if(typeSaved.Equals(""))
                        {
                            typeSaved = "All";
                        }
                        if (idSaved == -1)
                        {
                            IDText = "";
                        }
                        else
                        {
                            IDText = idSaved.ToString();
                        }
                        IsLowerChecked = LowerSaved;
                        IsEqualsChecked = EqualsSaved;
                        IsHigherChecked = HigherSaved;
                        SelectedTypeFilter = typeSaved;



                        if (idSaved < 0)
                        {
                            if (typeSaved.Equals("All"))
                            {
                                ResetEntity();
                            }
                            else
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (typeSaved.Equals(entity.Type.ToString()))
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                                ShowedCollection = MainWindowViewModel.Filter_entities;
                                OnPropertyChanged(nameof(ShowedCollection));
                            }
                        }
                        else if (typeSaved.Equals("All"))
                        {
                            if (LowerSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved < entity.Id)
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            else if (EqualsSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved == entity.Id)
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            else if (HigherSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved > entity.Id)
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            ShowedCollection = MainWindowViewModel.Filter_entities;
                            OnPropertyChanged(nameof(ShowedCollection));
                        }
                        else
                        {
                            if (LowerSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved < entity.Id && typeSaved.Equals(entity.Type.ToString()))
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            else if (EqualsSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved == entity.Id && typeSaved.Equals(entity.Type.ToString()))
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            else if (HigherSaved)
                            {
                                foreach (var entity in MainWindowViewModel.entities)
                                {
                                    if (idSaved > entity.Id && typeSaved.Equals(entity.Type.ToString()))
                                    {
                                        MainWindowViewModel.Filter_entities.Add(entity);
                                    }
                                }
                            }
                            ShowedCollection = MainWindowViewModel.Filter_entities;
                            OnPropertyChanged(nameof(ShowedCollection));
                        }
                    }
                }
            }
            else
            {
                try
                {
                    type = SelectedTypeFilter;
                    if (SelectedTypeFilter.Equals("All") || SelectedTypeFilter.Equals("System.Windows.Controls.ComboBoxItem: All"))
                    {
                        type = "";
                        int tmp = Int32.Parse(type);
                    }

                    if (Regex.IsMatch(IDText, @"^\d+$"))
                    {
                        int id = Int32.Parse(IDText);
                        if (IsLowerChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id < id && type.Equals(item.Type.ToString()))
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }
                        else if (IsEqualsChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id == id && type.Equals(item.Type.ToString()))
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }
                        else if (IsHigherChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id > id && type.Equals(item.Type.ToString()))
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }
                        ShowedCollection = MainWindowViewModel.Filter_entities;
                        AddToFilterList(id, IsLowerChecked, IsEqualsChecked, IsHigherChecked, type);
                        OnPropertyChanged(nameof(ShowedCollection));
                    }
                    else
                    {
                        foreach (var item in MainWindowViewModel.entities)
                        {
                            if (type.Equals(item.Type.ToString()))
                            {
                                MainWindowViewModel.Filter_entities.Add(item);
                            }
                        }
                        ShowedCollection = MainWindowViewModel.Filter_entities;
                        OnPropertyChanged(nameof(ShowedCollection));
                        int id = -1;
                        AddToFilterList(id, IsLowerChecked, IsEqualsChecked, IsHigherChecked, type);
                    }
                }
                catch (Exception ex)
                {
                    if (Regex.IsMatch(IDText, @"^\d+$"))
                    {
                        int id = Int32.Parse(IDText);
                        MainWindowViewModel.Filter_entities.Clear();
                        if (IsLowerChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id < id)
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }
                        else if (IsEqualsChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id == id)
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }
                        else if (IsHigherChecked)
                        {
                            foreach (var item in MainWindowViewModel.entities)
                            {
                                if (item.Id > id)
                                {
                                    MainWindowViewModel.Filter_entities.Add(item);
                                }
                            }
                        }

                        ShowedCollection = MainWindowViewModel.Filter_entities;
                        AddToFilterList(id, IsLowerChecked, IsEqualsChecked, IsHigherChecked, type);
                        OnPropertyChanged(nameof(ShowedCollection));
                    }
                    else
                    {
                        //Greska
                        if (type.Equals(""))
                        {
                            ResetEntity();
                        }
                        else
                        {
                            MessageBox.Show("Id must be positive number");
                        }
                    }
                }
            }
        }

        private void AddToFilterList(int id, bool IsLowerChecked, bool IsEqualsChecked, bool IsHigherChecked, string type)
        {
            
                Filter filter = new Filter(id, IsLowerChecked, IsEqualsChecked, IsHigherChecked, type);
                foreach (var item in Filters)
                {
                    if (item.Name.Equals(filter.Name))
                    {
                        return;
                    }
                }
                Filters.Add(filter);

        }

        public void ResetEntity()
        {
            if (ShowedCollection != NetowrkEntities)
            {
                ShowedCollection = NetowrkEntities;
                OnPropertyChanged(nameof(ShowedCollection));
            }
            IDText = "";
            IsLowerChecked = false;
            IsEqualsChecked = true;
            IsHigherChecked = false;
            SelectedTypeFilter = "All";
            SelectedItemFilter = "None";
        }

        public void RestartOtherApplication(string otherAppExecutablePath)
        {
            // Pronađi sve instance druge aplikacije i ugasi ih
            foreach (var process in Process.GetProcessesByName("MeteringSimulator")) // Zameni sa stvarnim imenom aplikacije bez ekstenzije
            {
                process.Kill();
                process.WaitForExit();
            }

            // Pokreni novu instancu aplikacije
            Process.Start(otherAppExecutablePath);
        }
    }
}

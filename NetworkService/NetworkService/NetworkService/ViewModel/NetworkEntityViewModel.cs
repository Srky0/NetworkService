using Game_Client.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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


        

        public NetworkEntityViewModel()
        {
            AddCommand = new MyICommand(AddEntity);

            DeleteCommand = new MyICommand(DeleteEntity);

            FilterCommand = new MyICommand(FilterEntity);
            
            ResetCommand = new MyICommand(ResetEntity);

            IsEqualsChecked = true;
            Filters.Clear();
            Filters.Add(new Filter());
            SelectedItemFilter = "None";
            SelectedTypeFilter = "All";
            ShowedCollection = NetowrkEntities;
        }

        private void DeleteEntity()
        {
            if (SelectedItems != null && SelectedItems.Any())
            {
                foreach (var item in SelectedItems.ToList())
                {
                    if (MainWindowViewModel.entities.Any(entity => entity.Id == item.Id))
                    {
                        MainWindowViewModel.entities.Remove(item);
                    }
                }

                NetowrkEntities = MainWindowViewModel.entities;
                MainWindowViewModel.Interval_Meter_entities.Clear();
                MainWindowViewModel.Smart_Meter_entities.Clear();
                foreach (var i in NetowrkEntities)
                {
                    if (i.Type.Equals(Type.Interval_Meter))
                    {
                        MainWindowViewModel.Interval_Meter_entities.Add(i);
                    }
                    else
                    {
                        MainWindowViewModel.Smart_Meter_entities.Add(i);
                    }
                }
                ResetEntity();
                SelectedItems.Clear();
            }
            else
            {
                //Gresaka: Izaberite sta se brise
            }
        }

        private void AddEntity()
        {
            string type = "";

            try
            {
                type = SelectedItemAdd.Split(':')[1].TrimStart();
            }
            catch (Exception)
            {
                //GRESKU IZBACITI
            }

            if (type.Equals("Interval_Meter"))
            {
                string name = $"Interval_Meter {_idI}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                MainWindowViewModel.entities.Add(new Entity(_id, name, Type.Interval_Meter, values));
                Count = NetowrkEntities.Count;
                MainWindowViewModel.Interval_Meter_entities.Add(new Entity(_id, name, Type.Interval_Meter, values));
                //MessageBox.Show(name);
                _idI++;
                _id++;
            }
            else if (type.Equals("Smart_Meter"))
            {
                string name = $"Smart_Meter {_idS}";
                List<double> values = new List<double> { 0, 0, 0, 0, 0 };
                MainWindowViewModel.entities.Add(new Entity(_id, name, Type.Smart_Meter, values));
                Count = NetowrkEntities.Count;
                MainWindowViewModel.Smart_Meter_entities.Add(new Entity(_id, name, Type.Smart_Meter, values));
                //MessageBox.Show(name);
                _idS++;
                _id++;
            }
            ResetEntity();
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

                        IDText = idSaved.ToString();
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
                    }
                    type = type.Split(':')[1].TrimStart();

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
            foreach(var item in Filters)
            {
                if(item.Name.Equals(filter.Name))
                {
                   return;
                }
            }
            Filters.Add(filter);
        }

        private void ResetEntity()
        {
            if (ShowedCollection != NetowrkEntities)
            {
                ShowedCollection = NetowrkEntities;
                OnPropertyChanged(nameof(ShowedCollection));
            }
        }
    }
}

using NetworkService.ViewModel;
using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum Type { Interval_Meter, Smart_Meter, All }
    public class Entity : BindableBase
    {
        private int _id;
        private string _name;
        private Type _type;
        private List<double> _value;
        private List<string> _timeline;
        private double _lastValue;

        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(nameof(Name)); } }
        public Type Type { get { return _type; } set { _type = value; OnPropertyChanged(nameof(Type)); } }
        public List<double> Value { get { return _value; } set { _value = value; OnPropertyChanged(nameof(Value)); } }
        public List<string> TimelineValues { get { return _timeline; } set { _timeline = value; OnPropertyChanged(nameof(TimelineValues)); } }
        public double LastValue { get { return _lastValue; } set { _lastValue = value; OnPropertyChanged(nameof(LastValue)); } }

        public Entity() { }

        public Entity(int id, string name, Type type, List<double> value)
        {
            this._id = id;
            this._name = name;
            this._type = type;
            this._value = value;
            this._timeline = new List<string> { DateTime.MinValue.ToString(), DateTime.MinValue.ToString(), DateTime.MinValue.ToString(), DateTime.MinValue.ToString(), DateTime.MinValue.ToString() };
        }


        public override string ToString()
        {
            return $"ID: {_id} Name: {_name} Type: {_type} Value: {_value}";
        }
    }
}

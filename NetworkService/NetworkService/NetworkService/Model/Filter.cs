using Notifications.Wpf.Annotations;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Filter
    {
        public int ID { get; set; }
        public bool IsLowerChecked { get; set; }
        public bool IsEqualsChecked { get; set; }
        public bool IsHigherChecked { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public Filter() 
        {
            Name = "None";
        }

        public Filter(int iD, bool isLowerChecked, bool isEqualsChecked, bool isHigherChecked, string type)
        {
            ID = iD;
            IsLowerChecked = isLowerChecked;
            IsEqualsChecked = isEqualsChecked;
            IsHigherChecked = isHigherChecked;
            Type = type;
            Name = ToString();
        }


        public override string ToString()
        {
            if(ID == -1)
            {
                return$"{Type}";
            }else
            {
                if(IsLowerChecked)
                {
                    if(Type.Equals(string.Empty))
                    {
                        return $"ID < {ID}";
                    }
                    else
                    {
                        return $"ID < {ID} {Type}";
                    }
                }else if(IsEqualsChecked)
                {
                    if (Type.Equals(string.Empty))
                    {
                        return $"ID = {ID}";
                    }
                    else
                    {
                        return $"ID = {ID} {Type}";
                    }
                }
                else if(IsHigherChecked)
                {
                    if (Type.Equals(string.Empty))
                    {
                        return $"ID > {ID}";
                    }
                    else
                    {
                        return $"ID > {ID} {Type}";
                    }
                }
                else
                {
                    return string.Empty;   
                }
            }
        }
    }
}

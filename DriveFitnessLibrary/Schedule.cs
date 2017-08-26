//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DriveFitnessLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedule : System.ComponentModel.INotifyPropertyChanged
    {
    
     #region Implement INotifyPropertyChanged
     
     public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
     
     protected virtual void OnPropertyChanged(string propertyName)
     {
      if (PropertyChanged != null)
      {
       PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
     }
     
     #endregion
    
        private int _ScheduleId;
        public int ScheduleId
        {
            get
            {
                return _ScheduleId;
            }
            set
            {
                if (_ScheduleId != value)
                {
                    _ScheduleId = value;
                    OnPropertyChanged("ScheduleId");
                }
            }
        }
     
        private int _GroupId;
        public int GroupId
        {
            get
            {
                return _GroupId;
            }
            set
            {
                if (_GroupId != value)
                {
                    _GroupId = value;
                    OnPropertyChanged("GroupId");
                }
            }
        }
     
        private string _Day;
        public string Day
        {
            get
            {
                return _Day;
            }
            set
            {
                if (_Day != value)
                {
                    _Day = value;
                    OnPropertyChanged("Day");
                }
            }
        }
     
        private string _Time;
        public string Time
        {
            get
            {
                return _Time;
            }
            set
            {
                if (_Time != value)
                {
                    _Time = value;
                    OnPropertyChanged("Time");
                }
            }
        }
     
    
        private Group _Group;
        public virtual Group Group
        {
            get
            {
                return _Group;
            }
            set
            {
                if (_Group != value)
                {
                    _Group = value;
                    OnPropertyChanged("Group");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Day, Time);
        }

        public Schedule Clone(Schedule schedule)
        {
            Schedule clone = new Schedule();
            clone.Group = schedule.Group;
            clone.Day = schedule.Day;
            clone.Time = schedule.Time;

            return clone;
        }

    }
}

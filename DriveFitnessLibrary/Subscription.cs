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
    
    public partial class Subscription : System.ComponentModel.INotifyPropertyChanged
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subscription()
        {
            this.Client = new HashSet<Client>();
        }
    
        private int _SubscriptionId;
     public int SubscriptionId 
     { 
      get
      {
       return _SubscriptionId;
      } 
      set
      {
       if(_SubscriptionId != value)
       {
        _SubscriptionId = value;
        OnPropertyChanged("SubscriptionId");
       }
      }
     }
     
        private int _SubscriptionCountExcercise;
     public int SubscriptionCountExcercise 
     { 
      get
      {
       return _SubscriptionCountExcercise;
      } 
      set
      {
       if(_SubscriptionCountExcercise != value)
       {
        _SubscriptionCountExcercise = value;
        OnPropertyChanged("SubscriptionCountExcercise");
       }
      }
     }
     
        private float _SubscriptionPrice;
     public float SubscriptionPrice 
     { 
      get
      {
       return _SubscriptionPrice;
      } 
      set
      {
       if(_SubscriptionPrice != value)
       {
        _SubscriptionPrice = value;
        OnPropertyChanged("SubscriptionPrice");
       }
      }
     }
     
        private System.DateTime _SubscriptionDate;
     public System.DateTime SubscriptionDate 
     { 
      get
      {
       return _SubscriptionDate;
      } 
      set
      {
       if(_SubscriptionDate != value)
       {
        _SubscriptionDate = value;
        OnPropertyChanged("SubscriptionDate");
       }
      }
     }
     
        private int _ClientSubscriptionId;
     public int ClientSubscriptionId 
     { 
      get
      {
       return _ClientSubscriptionId;
      } 
      set
      {
       if(_ClientSubscriptionId != value)
       {
        _ClientSubscriptionId = value;
        OnPropertyChanged("ClientSubscriptionId");
       }
      }
     }
     
        private Nullable<System.DateTime> _SubscriptionCloseDate;
     public Nullable<System.DateTime> SubscriptionCloseDate 
     { 
      get
      {
       return _SubscriptionCloseDate;
      } 
      set
      {
       if(_SubscriptionCloseDate != value)
       {
        _SubscriptionCloseDate = value;
        OnPropertyChanged("SubscriptionCloseDate");
       }
      }
     }
     
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        private ICollection<Client> _Client;
     public virtual ICollection<Client> Client 
     { 
      get
      {
       return _Client;
      } 
      set
      {
       if(_Client != value)
       {
        _Client = value;
        OnPropertyChanged("Client");
       }
      }
     }
     
    }
}
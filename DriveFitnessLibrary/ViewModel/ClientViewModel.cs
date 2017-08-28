using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DriveFitnessLibrary.ViewModel
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        #region Managers
        GroupManager groupManager;
        ClientManager clientManager;
        Messager messager;
        DialogManager dialogManager;
        #endregion

        public ClientViewModel()
        {
            groupManager = new GroupManager();
            clientManager = new ClientManager();
            dialogManager = new DialogManager();
            messager = new Messager();
            Groups = clientManager.GetGroups();
            NewClient = new Client();
            SelectedGroup = Groups[0];
            SubscriptionVisibility = Visibility.Hidden;
            AddSubscritionVisibility = Visibility.Hidden;
            NewSubscriptionDate = DateTime.Today;
            IsDataChanged = true;
            groupChanged = false;
        }

        #region Collections
        public ObservableCollection<Group> Groups { get; set; }

        private ObservableCollection<Client> clients;
        public ObservableCollection<Client> Clients
        {
            get { return clients; }
            set { clients = value; OnPropertyChanged(); }
        }
        #endregion

        #region Properties
        private Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;
                Clients = clientManager.GetClients(SelectedGroup);
                OnPropertyChanged();
            }
        }

        private Client selectedClient;
        public Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;

                if (SelectedClient == null) return;

                if (SelectedClient != null)
                {
                    NewGroup = SelectedClient.Group;

                    SelectedClient.PropertyChanged -= SelectedClient_PropertyChanged;
                    SelectedClient.PropertyChanged += SelectedClient_PropertyChanged;

                    if (SelectedClient.Subscription != null)
                    {
                        SubscriptionVisibility = Visibility.Visible;
                        AddSubscritionVisibility = Visibility.Hidden;

                        SelectedClient.Subscription.PropertyChanged -= Subscription_PropertyChanged;
                        SelectedClient.Subscription.PropertyChanged += Subscription_PropertyChanged;
                    }
                    else
                    {
                        SubscriptionVisibility = Visibility.Hidden;
                        AddSubscritionVisibility = Visibility.Visible;
                    }
                }
                OnPropertyChanged();
            }
        }

        private int newSubscriptionCount;
        public int NewSubscriptionCount
        {
            get { return newSubscriptionCount; }
            set { newSubscriptionCount = value; OnPropertyChanged(); }
        }

        private float newSubscriptionPrice;
        public float NewSubscriptionPrice
        {
            get { return newSubscriptionPrice; }
            set { newSubscriptionPrice = value; OnPropertyChanged(); }
        }

        private DateTime newSubscriptionDate;
        public DateTime NewSubscriptionDate
        {
            get { return newSubscriptionDate; }
            set { newSubscriptionDate = value; OnPropertyChanged(); }
        }

        private Group newGroup;
        public Group NewGroup
        {
            get { return newGroup; }
            set { newGroup = value; OnPropertyChanged(); }
        }

        private Client newClient;
        public Client NewClient
        {
            get { return newClient; }
            set { newClient = value; OnPropertyChanged(); }
        }

        private Client TempClient;
        #endregion;

        #region View Binding values
        private bool isDataChanged;
        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set { isDataChanged = value; OnPropertyChanged(); }
        }

        public bool IsSaveActivated
        {
            get { return !isDataChanged; }
        }

        private bool groupChanged;

        private Visibility subscriptionVisibility;
        public Visibility SubscriptionVisibility
        {
            get { return subscriptionVisibility; }
            set { subscriptionVisibility = value; OnPropertyChanged(); }
        }

        private Visibility addSubscriptionVisibility;
        public Visibility AddSubscritionVisibility
        {
            get
            {
                return addSubscriptionVisibility;
            }
            set
            {
                addSubscriptionVisibility = value;
                OnPropertyChanged();
            }

        }

        #endregion

        private void Subscription_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionCountExcercise" ||
                e.PropertyName == "SubscriptionPrice" ||
                    e.PropertyName == "SubscriptionDate"
                    )
            {
                if (!IsDataChanged) return;

                if (clientManager.CheckChanges() == true)
                    IsDataChanged = false;
            }
        }
        private void SelectedClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ClientName" || 
                e.PropertyName == "ClientLastname" ||
                    e.PropertyName == "ClientBirthday" ||
                    e.PropertyName == "ClientTelephone" ||
                    e.PropertyName == "ClientEmail"
                    )
            {
                if (!IsDataChanged) return;

                if (clientManager.CheckChanges() == true)
                    IsDataChanged = false;
            }

            if (e.PropertyName == "Subscription")
            {
                if (SelectedClient.Subscription == null)
                {
                    SubscriptionVisibility = Visibility.Hidden;
                    AddSubscritionVisibility = Visibility.Visible;
                    return;
                }
                
                SubscriptionVisibility = Visibility.Visible;
                AddSubscritionVisibility = Visibility.Hidden;

                if (!IsDataChanged) return;

                if (clientManager.CheckChanges() == true)
                    IsDataChanged = false;
            }
        }

        #region Commands

        private BCommand saveDataChangesCommand;
        public BCommand SaveDataChangesCommand
        {
            get
            {
                return saveDataChangesCommand ?? (saveDataChangesCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите сохранить изменение информации о клиенте: \"{ SelectedClient}\"?"))
                    {
                        clientManager.SaveDataContext();
                        if (groupChanged) Clients.Remove(SelectedClient);
                        messager.SuccessMessage("Изменения успешно сохранены!");
                        IsDataChanged = true;
                    }
                },
                (obj) =>
                {
                    return IsSaveActivated;
                }
                ));
            }
        }

        private BCommand discardChangesCommandv;
        public BCommand DiscardChangesCommand
        {
            get
            {
                return discardChangesCommandv ?? (discardChangesCommandv = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите отменить изменения о клиенте: \"{ SelectedClient}\"?"))
                    {
                        clientManager.DiscardChanges();
                        messager.SuccessMessage("Все изменения отменены.");
                        if (TempClient != null)
                            Clients.Remove(TempClient);
                        IsDataChanged = true;
                    }
                },
                (obj) =>
                {
                    return !IsDataChanged;
                }
                ));
            }
        }

        private BCommand createSubscriptionCommand;
        public BCommand CreateSubscriptionCommand
        {
            get
            {
                return createSubscriptionCommand ?? (createSubscriptionCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Открыть абонемент клиенту: \"{ SelectedClient}\"?"))
                    {
                        Popup pop = obj as Popup;
                        Subscription temp = new Subscription();
                        temp.SubscriptionCountExcercise = NewSubscriptionCount;
                        temp.SubscriptionPrice = NewSubscriptionPrice;
                        temp.SubscriptionDate = NewSubscriptionDate;
                        temp.ClientSubscriptionId = SelectedClient.ClientId;

                        clientManager.AddSubscription(SelectedClient, temp);

                        NewSubscriptionCount = 0;
                        NewSubscriptionPrice = 0;
                        NewSubscriptionDate = DateTime.Today;

                        pop.IsOpen = false;
                    }
                },
                (obj) =>
                {
                    if (NewSubscriptionCount == 0)
                        return false;

                    return true;
                }

                ));
            }
        }

        private BCommand showSubscriptionWindowCommand;
        public BCommand ShowSubscriptionWindowCommand
        {
            get
            {
                return showSubscriptionWindowCommand ?? (showSubscriptionWindowCommand = new BCommand((obj)=>
                {
                    Popup pop = obj as Popup;
                    pop.IsOpen = true;
                }
                ));
            }
        }

        private BCommand removeSubscriptionCommand;
        public BCommand RemoveSubscriptionCommand
        {
            get
            {
                return removeSubscriptionCommand ?? (removeSubscriptionCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите полностью удалить абонемент клиента: \"{ SelectedClient}\"?"))
                    {
                        clientManager.RemoveSubscription(SelectedClient.Subscription);
                        IsDataChanged = false;
                    }
                }
                ));
            }
        }

        private BCommand closeSubscriptionCommand;
        public BCommand CloseSubscriptionCommand
        {
            get
            {
                return closeSubscriptionCommand ?? (closeSubscriptionCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите закрыть абонемент клиента: \"{ SelectedClient}\"?"))
                    {
                        clientManager.CloseSubscription(SelectedClient);
                        IsDataChanged = false;
                    }
                }
                ));
            }
        }

        private BCommand showChangeGroupWindowCommand;
        public BCommand ShowChangeGroupWindowCommand
        {
            get
            {
                return showChangeGroupWindowCommand ?? (showChangeGroupWindowCommand =  new BCommand((obj)=> 
                {
                    Popup pop = obj as Popup;
                    pop.IsOpen = true;
                    NewGroup = SelectedGroup;
                }
                ));

            }
        }

        private BCommand changeGroupCommand;
        public BCommand ChangeGroupCommand
        {
            get
            {
                return changeGroupCommand ?? (changeGroupCommand = new BCommand((obj) =>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите закрыть абонемент клиента: \"{ SelectedClient}\"?"))
                    {
                        Popup pop = obj as Popup;

                        clientManager.ChangeGroup(SelectedClient, NewGroup);

                        groupChanged = true;
                        IsDataChanged = false;

                        pop.IsOpen = false;
                    }
                }
              ));
            }
        }

        private BCommand showNewClientWindowCommand;
        public BCommand ShowNewClientWindowCommand
        {
            get
            {
                return showNewClientWindowCommand ?? (showNewClientWindowCommand = new BCommand((obj)=>
                {
                    Popup pop = obj as Popup;
                    pop.IsOpen = true;
                }
                ));
            }
        }

        private BCommand addClientCommand;
        public BCommand AddClientCommand
        {
            get
            {
                return addClientCommand ?? (addClientCommand = new BCommand((obj)=> 
                {
                    Popup pop = obj as Popup;

                    Client temp = new Client();

                    temp.ClientLastname = NewClient.ClientLastname;
                    temp.ClientName = NewClient.ClientName;
                    temp.ClientBirthday = NewClient.ClientBirthday;
                    temp.ClientEmail = NewClient.ClientEmail;
                    temp.ClientTelephone = NewClient.ClientTelephone;
                    temp.Group = SelectedGroup;

                    TempClient = temp;

                    Clients.Add(temp);
                    SelectedClient = temp;

                    IsDataChanged = false;
                    
                    clientManager.AddNewClient(temp, SelectedGroup);

                    NewClient = new Client();

                    pop.IsOpen = false;
                },
                (obj) =>
                {
                    return true;
                }
                ));


            }
        }

        private BCommand removeClientCommand;
        public BCommand RemoveClientCommand
        {
            get
            {
                return removeClientCommand ?? (removeClientCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите удалить клиента: \"{ SelectedClient}\" из базы данных?"))
                    {
                        clientManager.RemoveClient(SelectedClient);
                        Clients.Remove(SelectedClient);
                        clientManager.SaveDataContext();
                        messager.SuccessMessage("Информация о клиенте успешно удалена из базы данных.");
                        
                    }
                },
                (obj) =>
                {
                    if (SelectedClient == null) return false;
                    return true;
                }
                ));

            }
        }

        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
#endregion
    }
}

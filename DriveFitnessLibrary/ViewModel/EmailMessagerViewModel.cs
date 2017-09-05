using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary.ViewModel
{
    public class EmailMessagerViewModel : INotifyPropertyChanged
    {
        #region Managers
        EmailMessageSender emailSenderManager;
        Messager messager;
        #endregion

        public EmailMessagerViewModel()
        {
            emailSenderManager = new EmailMessageSender();
            messager = new Messager();

            Groups = emailSenderManager.GetGroups();
            if (Groups.Count > 0)
                SelectedGroup = Groups[0];
                
        }

        public List<Group> Groups { get; set; }

        private Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;

                Clients = new List<ClientCheckboxModel>();

                var cl = SelectedGroup.Client.ToList();

                foreach (var c in cl)
                {
                    ClientCheckboxModel temp = new ClientCheckboxModel();
                    temp.ClientData = c;
                    temp.IsSelected = false;

                    Clients.Add(temp);
                }
                
                OnPropertyChanged();
            }
        }

        private List<ClientCheckboxModel> clients;
        public List<ClientCheckboxModel> Clients
        {
            get { return clients; }
            set { clients = value; OnPropertyChanged(); }
        }

        private ClientCheckboxModel selectedClient;
        public ClientCheckboxModel SelectedClient
        {
            get { return selectedClient; }
            set { selectedClient = value; OnPropertyChanged(); }
        }


        private string login;
        public string Login
        {
            get { return login; }
            set { login = value; OnPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        private string subject;
        public string Subject
        {
            get { return subject; }
            set { subject = value; OnPropertyChanged(); }
        }

        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(); }
        }

        private bool allSelected = false;

        #region Commands
        private BCommand sendMessageCommand;
        public BCommand SendMessageCommand
        {
            get
            {
                return sendMessageCommand ?? (sendMessageCommand = new BCommand((obj)=>
                {
                    var cl = Clients.Where(x=>x.IsSelected == true);
                    StringBuilder sb = new StringBuilder();

                    foreach (var c in cl)
                    {
                        emailSenderManager.SendMessage(c.ClientData, Subject, MessageText, Login, Password, out string sended);
                        sb.Append(sended);
                        if (!string.IsNullOrEmpty(sended))
                            sb.Append(Environment.NewLine);
                    }

                    messager.SuccessMessage($"Отправка сообщений завершена, сообщения отправлены:{Environment.NewLine}{sb.ToString()}");
                },
                (obj)=>
                {
                    if (String.IsNullOrEmpty(Login) || String.IsNullOrEmpty(Password) ||
                        String.IsNullOrEmpty(Subject) || String.IsNullOrEmpty(MessageText)
                    )
                        return false;

                    if (Clients.Where(x => x.IsSelected == true).Count() > 0) return true;
                    return false;
                }
                ));
            }
            
        }

        private BCommand checkAllCommand;
        public BCommand CheckAllCommand
        {
            get
            {
                return checkAllCommand ?? (checkAllCommand = new BCommand((obj)=>
                {
                    if (!allSelected)
                    {
                        foreach (var c in Clients)
                            c.IsSelected = true;

                        allSelected = true;
                    }
                    else
                    {
                        foreach (var c in Clients)
                            c.IsSelected = false;

                        allSelected = false;
                    }
                }
                ));
            }
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}

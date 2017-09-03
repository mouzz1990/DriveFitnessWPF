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
        EmailMessageSender emailSenderManager;

        public EmailMessagerViewModel()
        {
            emailSenderManager = new EmailMessageSender();
        }

        public List<Group> Groups { get; set; }

        private List<Client> clients;
        public List<Client> Clients
        {
            get { return clients; }
            set { clients = value; OnPropertyChanged(); }
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

        #region Commands
        private BCommand sendMessageCommand;
        public BCommand SendMessageCommand
        {
            get
            {
                return sendMessageCommand ?? (sendMessageCommand = new BCommand((obj)=>
                {

                },
                (obj)=>
                {
                    return true;
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

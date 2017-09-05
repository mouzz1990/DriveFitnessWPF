using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DriveFitnessLibrary.ViewModel
{
    public class AttendanceManualViewModel : DependencyObject, INotifyPropertyChanged
    {
        AttendanceManager attendanceManager;
        Messager messager;

        private static ObservableCollection<DateTime> selectedClientVisitedDate;
        public static ObservableCollection<DateTime> SelectedClientVisitedDate
        {
            get { return selectedClientVisitedDate; }
            set { selectedClientVisitedDate = value; }
        }


        public AttendanceManualViewModel()
        {
            attendanceManager = new AttendanceManager();

            attendanceManager.AttendanceFixed += AttendanceManager_AttendanceFixed;
            attendanceManager.SubscriptionClosed += AttendanceManager_SubscriptionClosed;

            messager = new Messager();
            AttendancePrice = 30;

            Groups = attendanceManager.GetGroups();
            if (Groups.Count > 0)
                SelectedGroup = Groups[0];

            SelectedDate = DateTime.Today;
        }

        private void AttendanceManager_SubscriptionClosed(object sender, EventArgs e)
        {
            AttendanceEventArgs evArg = (AttendanceEventArgs)e;
            messager.SuccessMessage(evArg.Message);
        }

        private void AttendanceManager_AttendanceFixed(object sender, EventArgs e)
        {
            AttendanceEventArgs evArg = (AttendanceEventArgs)e;
            messager.SuccessMessage(evArg.Message);
        }

        public List<Group> Groups { get; private set; }

        private Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set { selectedGroup = value; Clients = SelectedGroup.Client.ToList(); OnPropertyChanged(); }
        }

        private List<Client> clients;
        public List<Client> Clients
        {
            get { return clients; }
            set { clients = value; OnPropertyChanged(); }
        }

        private Client selectedClient;
        public Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                VisitedDates = attendanceManager.GetVisitedDates(SelectedClient);
                SelectedClientVisitedDate = VisitedDates;

                OnPropertyChanged(); }
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; OnPropertyChanged(); }
        }

        private float attendancePrice;
        public float AttendancePrice
        {
            get { return attendancePrice; }
            set { attendancePrice = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DateTime> visitedDates;
        public ObservableCollection<DateTime> VisitedDates
        {
            get { return visitedDates; }
            set
            {
                visitedDates = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler VisitedDatesChanged;
        public void OnVisitedDatesChanged()
        {
            VisitedDatesChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Commands
        private BCommand addAttendanceCommand;
        public BCommand AddAttendanceCommand
        {
            get
            {
                return addAttendanceCommand ?? (addAttendanceCommand = new BCommand((obj)=>
                {
                    try
                    {
                        attendanceManager.AddAttendance(SelectedClient, AttendancePrice, SelectedDate);
                        
                        VisitedDates.Add(SelectedDate);
                        //temprary solution for refresh calendar
                        Client temp = SelectedClient;
                        SelectedClient = null;
                        SelectedClient = temp;
                    }
                    catch (InvalidOperationException exc)
                    {
                        messager.ErrorMessage(exc.Message);
                    }
                },
                (obj)=>
                {
                    if (SelectedClient == null || VisitedDates.Contains(SelectedDate)) return false;
                    return true;
                }
                ));
            }
        }

        private BCommand removeAttendanceCommand;
        public BCommand RemoveAttendanceCommand
        {
            get
            {
                return removeAttendanceCommand ?? (removeAttendanceCommand = new BCommand((obj)=>
                {
                    bool result = attendanceManager.RemoveAttendance(SelectedClient, SelectedDate);

                    if (!result)
                        messager.ErrorMessage("Ошибка удаления посещения.");

                    VisitedDates.Remove(SelectedDate);
                    messager.SuccessMessage($"Посещение \"{SelectedClient}\" - {SelectedDate.ToShortDateString()} успешно удалено!");

                    ////temprary solution for refresh calendar
                    Client temp = SelectedClient;
                    SelectedClient = null;
                    SelectedClient = temp;
                },
                (obj) =>
                {
                    if (SelectedClient == null || !VisitedDates.Contains(SelectedDate)) return false;

                    return true;
                }
                ));
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}

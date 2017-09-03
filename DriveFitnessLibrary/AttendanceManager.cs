using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary
{
    public class AttendanceManager
    {
        DrivefitnessContext DriveContext;

        public List<Group> GetGroups()
        {
            return DriveContext.Group.ToList();
        }

        public event EventHandler SubscriptionClosed;
        public void OnSubscriptionClosed(string message)
        {
            AttendanceEventArgs evArg = new AttendanceEventArgs(message);
            SubscriptionClosed?.Invoke(this, evArg);
        }

        public event EventHandler AttendanceFixed;
        public void OnAttendanceFixed(string message)
        {
            AttendanceEventArgs evArg = new AttendanceEventArgs(message);
            AttendanceFixed?.Invoke(this, evArg);
        }

        public AttendanceManager()
        {
            DriveContext = new DrivefitnessContext();
        }
        public void AddAttendance(Client client, float price, DateTime date)
        {
            if (CheckAttendance(client, date))
            {
                //Attendance by cash
                if (client.Subscription == null)
                {
                    Attendance att = new Attendance();
                    att.Client = client;
                    att.AttendancePrice = price;
                    att.DateVisit = date;
                    att.Payment = price.ToString();

                    DriveContext.Attendance.Add(att);

                    string message = string.Format("Посещение клиента \"{0}\" зафиксировано: {1} - {2}",
                        client,
                        date.ToShortDateString(),
                        price
                        );

                    OnAttendanceFixed(message);

                    DriveContext.SaveChanges();
                }
                //Attendance by subscription
                else
                {
                    Attendance att = new Attendance();
                    att.Client = client;
                    att.AttendancePrice = 0;
                    att.DateVisit = date;
                    att.Payment = "A";

                    DriveContext.Attendance.Add(att);

                    string message = string.Format("Посещение клиента \"{0}\" зафиксировано {1}",
                        client,
                        date.ToShortDateString()
                        );

                    OnAttendanceFixed(message);

                    client.Subscription.SubscriptionCountExcercise--;
                    if (client.Subscription.SubscriptionCountExcercise == 0)
                    {
                        string mess = string.Format("Абонемент клиента \"{0}\" закрыт.",
                            client
                            );

                        client.Subscription = null;

                        OnSubscriptionClosed(mess);
                    }

                    DriveContext.SaveChanges();
                }
            }
            else
                throw new InvalidOperationException($"Посещение клиента \"{client}\" - {date.ToShortDateString()} уже зафиксировано!");
        }

        private bool CheckAttendance(Client client, DateTime date)
        {
            var q = DriveContext.Attendance.Where(x=> x.ClientId == client.ClientId && x.DateVisit == date);
            
            if (q.ToList().Count != 0) return false;

            return true;
        }

        public Client GetClient(string scannedString)
        {
            if (int.TryParse(scannedString.Split(':')[0], out int id))
            {
                var clientsFinded = DriveContext.Client.Where(x => x.ClientId == id).ToList();
                if (clientsFinded.Count != 0) 
                    return clientsFinded[0];

            }

            return null;
        }

        public ObservableCollection<DateTime> GetVisitedDates(Client client)
        {
            if (client == null) return null;

            var attendances = (from att in DriveContext.Attendance
                              where att.ClientId == client.ClientId
                              select att.DateVisit).ToList();
            ObservableCollection<DateTime> at = new ObservableCollection<DateTime>(attendances);
            return at;
        }

        public bool RemoveAttendance(Client client, DateTime visitationDate)
        {
            var attendances = (DriveContext.Attendance.Where(x => x.ClientId == client.ClientId && x.DateVisit == visitationDate)).ToList();

            if (attendances.Count == 0) return false;

            var removableAttendance = attendances[0];

            DriveContext.Attendance.Remove(removableAttendance);

            DriveContext.SaveChanges();

            return true;
        }
    }
}

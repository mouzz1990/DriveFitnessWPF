using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary
{
    public class ClientManager
    {
        DrivefitnessContext DriveContext;

        public ClientManager()
        {
            DriveContext = new DrivefitnessContext();
        }

        public ObservableCollection<Client> GetClients(Group gr)
        {
            var q = from c in DriveContext.Client
                    where c.GroupId == gr.GroupId
                    select c;

            ObservableCollection<Client> clients = new ObservableCollection<Client>(q);
            return clients;
        }

        public ObservableCollection<Group> GetGroups()
        {
            ObservableCollection<Group> groups = new ObservableCollection<Group>(DriveContext.Group);
            return groups;
        }

        public void SaveDataContext()
        {
            DriveContext.SaveChanges();
        }

        public void DiscardChanges()
        {
            foreach (DbEntityEntry entry in DriveContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }

        public bool CheckChanges()
        {
            var tracker = DriveContext.ChangeTracker;
            bool result = tracker.HasChanges();

            return result;
        }

        public void AddSubscription(Client client, Subscription sub)
        {
            client.Subscription = sub;
        }

        public void RemoveSubscription(Subscription sub)
        {
            DriveContext.Subscription.Remove(sub);
        }

        public void CloseSubscription(Client client)
        {
            client.Subscription.SubscriptionCloseDate = DateTime.Today;
            client.Subscription = null;
        }

        public void ChangeGroup(Client client, Group gr)
        {
            client.Group = gr;
        }

        public void AddNewClient(Client client, Group gr)
        {
            gr.Client.Add(client);
        }

        public void RemoveClient(Client client)
        {
            DriveContext.Client.Remove(client);
        }
    }
}

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
    public class GroupManager
    {
        DrivefitnessContext DriveContext = new DrivefitnessContext();

        public ObservableCollection<Group> GetGroups()
        {
            ObservableCollection<Group> groups = new ObservableCollection<Group>(DriveContext.Group);
            return groups;
        }

        public ObservableCollection<Schedule> GetSchedules(Group gr)
        {
            var q = from s in DriveContext.Schedule
                    where s.GroupId == gr.GroupId
                    select s;

            ObservableCollection<Schedule> schedules = new ObservableCollection<Schedule>(q.ToList());
            return schedules;
        }

        public void RemoveSchedule(Schedule schedule)
        {
            DriveContext.Schedule.Remove(schedule);
        }

        public void AddSchedule(Schedule schedule)
        {
            DriveContext.Schedule.Add(schedule);
        }

        public void AddGroup(Group group)
        {
            DriveContext.Group.Add(group);
            DriveContext.SaveChanges();
        }

        public bool RemoveGroup(Group group)
        {
            if (group.Schedule.Count > 0) return false;

            DriveContext.Group.Remove(group);
            DriveContext.SaveChanges();
            return true;
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
    }
}

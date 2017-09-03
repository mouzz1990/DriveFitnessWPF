using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace DriveFitnessLibrary.ViewModel
{
    public class GroupViewModel : INotifyPropertyChanged
    {
        #region Managers
        GroupManager groupManager;
        DialogManager dialogManager;
        Messager messager;
        #endregion

        public GroupViewModel()
        {
            groupManager = new GroupManager();
            dialogManager = new DialogManager();
            messager = new Messager();
            IsDataChanged = true;
            Groups = groupManager.GetGroups();
            NewSchedule = new Schedule();
            NewGroup = new Group();
        }

        private void SelectedGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GroupName")
            {
                if (!IsDataChanged) return;

                if (groupManager.CheckChanges() == true)
                    IsDataChanged = false;
            }
        }
        private void SchedulesObservable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsDataChanged) return;

            if (groupManager.CheckChanges() == true)
            {
                IsDataChanged = false;
            }
        }
        private void SelectedSchedule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Day" || e.PropertyName == "Time")
            {
                if (!IsDataChanged) return;

                if (groupManager.CheckChanges() == true)
                    IsDataChanged = false;
            }
        }

        public ObservableCollection<Group> Groups { get; set; }

        private Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;
                
                if (SelectedGroup == null)
                {
                    OnPropertyChanged();
                    return;
                }

                SchedulesObservable = groupManager.GetSchedules(SelectedGroup);

                SelectedGroup.PropertyChanged -= SelectedGroup_PropertyChanged;
                SelectedGroup.PropertyChanged += SelectedGroup_PropertyChanged;

                SchedulesObservable.CollectionChanged -= SchedulesObservable_CollectionChanged;
                SchedulesObservable.CollectionChanged += SchedulesObservable_CollectionChanged;

                OnPropertyChanged();
            }
        }

        private ObservableCollection<Schedule> schedulesObservable;
        public ObservableCollection<Schedule> SchedulesObservable
        {
            get { return schedulesObservable; }
            set { schedulesObservable = value; OnPropertyChanged(); }
        }

        private List<Schedule> schedules;
        public List<Schedule> Schedules
        {
            get { return schedules; }
            set { schedules = value; OnPropertyChanged(); }
        }

        private Schedule selectedSchedule;
        public Schedule SelectedSchedule
        {
            get { return selectedSchedule; }
            set
            {
                selectedSchedule = value;

                if (SelectedSchedule == null) return;

                SelectedSchedule.PropertyChanged -= SelectedSchedule_PropertyChanged;
                SelectedSchedule.PropertyChanged += SelectedSchedule_PropertyChanged;
                OnPropertyChanged();
            }
        }

        private Schedule newSchedule;
        public Schedule NewSchedule
        {
            get { return newSchedule; }
            set { newSchedule = value; OnPropertyChanged(); }
        }

        private Group newGroup;
        public Group NewGroup
        {
            get { return newGroup; }
            set { newGroup = value; OnPropertyChanged(); }
        }


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


        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region Commands
        private BCommand saveChangesCommand;
        public BCommand SaveChangesCommand
        {
            get
            {
                return saveChangesCommand ?? (saveChangesCommand = new BCommand((obj)=> 
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите сохранить изменения в группе: \"{ SelectedGroup}\"?"))
                    {
                        groupManager.SaveDataContext();
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

        private BCommand discardChangesCommand;
        public BCommand DiscardChangesCommand
        {
            get
            {
                return discardChangesCommand ?? (discardChangesCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите отменить изменения в группе: \"{ SelectedGroup}\"?"))
                    {
                        groupManager.DiscardChanges();
                        IsDataChanged = true;

                        //Временное решение... Подумать!!!
                        var temp = SelectedGroup;
                        SelectedGroup = Groups[0];
                        SelectedGroup = temp;
                    }
                },
                (obj) => 
                {
                    return !IsDataChanged;
                }
                ));
            }
        }

        private BCommand removeScheduleCommand;
        public BCommand RemoveScheduleCommand
        {
            get
            {
                return removeScheduleCommand ?? (removeScheduleCommand = new BCommand((obj)=> 
                {
                    groupManager.RemoveSchedule(SelectedSchedule);
                    SchedulesObservable.Remove(SelectedSchedule);
                },
                (obj) => 
                {
                    return !(SelectedSchedule == null);
                }
                ));
            }
        }

        private BCommand addScheduleCommand;
        public BCommand AddScheduleCommand
        {
            get
            {
                return addScheduleCommand ?? (addScheduleCommand = new BCommand(
                (obj) =>
                {
                    Popup pop = obj as Popup;
                    Schedule temp = new Schedule();
                    temp.Day = NewSchedule.Day;
                    temp.Time = NewSchedule.Time;
                    temp.Group = SelectedGroup;

                    groupManager.AddSchedule(temp);
                    SchedulesObservable.Add(temp);

                    NewSchedule.Time = string.Empty;
                    NewSchedule.Day = string.Empty;

                    pop.IsOpen = false;
                },
                (obj) =>
                {
                    if (string.IsNullOrWhiteSpace(NewSchedule.Time) || string.IsNullOrWhiteSpace(NewSchedule.Day))
                        return false;

                    return true;
                }
                ));
            }
        }

        private BCommand addGroupCommand;
        public BCommand AddGroupCommand
        {
            get
            {
                return addGroupCommand ?? (addGroupCommand = new BCommand((obj) =>
                {
                    Popup pop = obj as Popup;
                    Group temp = new Group();
                    temp.GroupName = NewGroup.GroupName;
                    groupManager.AddGroup(temp);
                    Groups.Add(temp);

                    NewGroup.GroupName = string.Empty;

                    pop.IsOpen = false;
                },
                (obj) =>
                {
                    if (string.IsNullOrWhiteSpace(NewGroup.GroupName)) return false;
                    return true;
                }
                ));
            }
        }

        private BCommand removeGroupCommand;
        public BCommand RemoveGroupCommand
        {
            get
            {
                return removeGroupCommand ?? (removeGroupCommand = new BCommand((obj)=>
                {
                    if (dialogManager.DialogOkCancel($"Вы уверены что хотите удалить группу: \"{ SelectedGroup}\"?"))
                    {
                        if (groupManager.RemoveGroup(SelectedGroup))
                            Groups.Remove(SelectedGroup);
                        else
                            messager.ErrorMessage(
                                string.Format(
                                    "Удаление группы невозможно пока в ней есть клиенты и рассписание!{0}{0}Пожалуйста удалите рассписание, поправьте списки клиентов и повторите попытку.",
                                    Environment.NewLine
                                    ));
                    }
                    else return;
                },
                (obj) =>
                {
                    if (SelectedGroup == null) return false;

                    return true;
                }
                ));
            }
        }

        private BCommand showNewSchedulePopup;
        public BCommand ShowNewSchedulePopup
        {
            get
            {
                return showNewSchedulePopup ?? (showNewSchedulePopup = new BCommand((obj) =>
               {
                   Popup pop = obj as Popup;
                   pop.IsOpen = true;
               },
                (obj) =>
                {
                    if (SelectedGroup == null) return false;
                    return true;
                }
                ));
            }
        }

        private BCommand showNewGroupPopup;
        public BCommand ShowNewGroupPopup
        {
            get
            {
                return showNewGroupPopup ?? (showNewGroupPopup = new BCommand((obj) =>
                {
                    Popup pop = obj as Popup;
                    pop.IsOpen = true;
                }
                ));
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary.ViewModel
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        ReportManager reportManager;
        DialogManager dialogManager;

        public ReportViewModel()
        {
            reportManager = new ReportManager();
            dialogManager = new DialogManager();
            StartReportDate = DateTime.Today;
            EndReportDate = DateTime.Today;

            Groups = reportManager.GetGroups();
            if (Groups.Count > 0)
                SelectedGroup = Groups[0];
        }

        private List<Group> groups;
        public List<Group> Groups
        {
            get { return groups; }
            set { groups = value; OnPropertyChanged(); }
        }

        private Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set { selectedGroup = value; OnPropertyChanged(); }
        }

        private DataTable data;
        public DataTable Data
        {
            get { return data; }
            set { data = value; OnPropertyChanged(); }
        }

        private DateTime startReportDate;
        public DateTime StartReportDate
        {
            get { return startReportDate; }
            set { startReportDate = value; OnPropertyChanged(); }
        }

        private DateTime endReportDate;
        public DateTime EndReportDate
        {
            get { return endReportDate; }
            set { endReportDate = value; OnPropertyChanged(); }
        }

        #region Commands
        private BCommand makeReportCommand;
        public BCommand MakeReportCommand
        {
            get
            {
                return makeReportCommand ?? (makeReportCommand = new BCommand((obj)=>
                {
                    DataTable dt = reportManager.GetAttendanceTable(SelectedGroup, StartReportDate, EndReportDate);
                    Data = dt;
                }
                ));
            }
        }

        private BCommand excelReportCommand;
        public BCommand ExcelReportCommand
        {
            get
            {
                return excelReportCommand ?? (excelReportCommand = new BCommand((obj) =>
                {
                    string filename = "";
                    if (dialogManager.DialogSavePath(out filename))
                    {
                        Task tsk = Task.Factory.StartNew(()=> { reportManager.CreateReport(Data, filename); });
                    }
                },
                (obj)=>
                {
                    if (Data == null) return false;
                    return true;
                }
                ));
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}

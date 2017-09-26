using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary.ViewModel
{
    public class AttendanceCameraViewModel : INotifyPropertyChanged
    {
        AttendanceManager attendanceManager;
        Messager messager;

        public AttendanceCameraViewModel()
        {
            VideoDeviceManager = new VideoDeviceManager();
            attendanceManager = new AttendanceManager();
            messager = new Messager();

            VideoDeviceManager.VisitationChecked += VideoDeviceManager_VisitationChecked;
            attendanceManager.SubscriptionClosed += AttendanceManager_SubscriptionClosed;
            attendanceManager.AttendanceFixed += AttendanceManager_AttendanceFixed;

            AttendancePrice = (float)30.0;
            AttendanceDate = DateTime.Today;

            if (VideoDeviceManager.Devices.Count > 0)
                VideoDeviceManager.SelectedDevice = VideoDeviceManager.Devices[0];

        }

        private void AttendanceManager_AttendanceFixed(object sender, EventArgs e)
        {
            AttendanceEventArgs evArg = (AttendanceEventArgs)e;
            messager.SuccessMessage(evArg.Message);
        }

        private void AttendanceManager_SubscriptionClosed(object sender, EventArgs e)
        {
            AttendanceEventArgs evArg = (AttendanceEventArgs)e;
            messager.SuccessMessage(evArg.Message);
        }

        private void VideoDeviceManager_VisitationChecked(object sender, EventArgs e)
        {
            Client client = attendanceManager.GetClient(VideoDeviceManager.ScannedResult);

            if (client == null)
            {
                messager.ErrorMessage("Клиент не найдет в базе данных.");
                return;
            }

            try
            {
                attendanceManager.AddAttendance(client, AttendancePrice, AttendanceDate);
            }
            catch (InvalidOperationException ex)
            {
                messager.ErrorMessage(ex.Message);
            }
        }

        private float attendancePrice;
        public float AttendancePrice
        {
            get { return attendancePrice; }
            set { attendancePrice = value; OnPropertyChanged(); }
        }

        private DateTime attendanceDate;
        public DateTime AttendanceDate
        {
            get { return attendanceDate; }
            set { attendanceDate = value; OnPropertyChanged(); }
        }

        private VideoDeviceManager videoDeviceManager;
        public VideoDeviceManager VideoDeviceManager
        {
            get { return videoDeviceManager; }
            set { videoDeviceManager = value; OnPropertyChanged(); }
        }

        private BCommand startScanCommand;
        public BCommand StartScanCommand
        {
            get
            {
                return startScanCommand ?? (startScanCommand = new BCommand((obj)=>
                {
                    VideoDeviceManager.StartScan();
                },
                (obj) =>
                {
                    if (VideoDeviceManager.IsScanStarted)
                        return false;

                    if (VideoDeviceManager.Devices.Count == 0)
                        return false;

                    return true;
                }
                ));
            }
        }

        private BCommand stopScanCommand;
        public BCommand StopScanCommand
        {
            get
            {
                return stopScanCommand ?? (stopScanCommand = new BCommand((obj) =>
                {
                    VideoDeviceManager.StopScan();
                },
                (obj) =>
                {
                    return VideoDeviceManager.IsScanStarted;
                }
                ));
            }
        }
        
        public void AttendanceCameraWindow_Closing(object sender, CancelEventArgs e)
        {
            VideoDeviceManager.StopScan();
        }

        #region INotifyPropertyChanged implementation;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}

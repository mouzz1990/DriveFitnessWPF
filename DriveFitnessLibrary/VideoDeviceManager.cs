using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using AForge.Video;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;

namespace DriveFitnessLibrary
{
    public class VideoDeviceManager : INotifyPropertyChanged
    {
        public VideoDeviceManager()
        {
            devices = new ObservableCollection<FilterInfo>();
            InitalizeVideoManager();
            IsScanStarted = false;
        }

        private ObservableCollection<FilterInfo> devices;
        public ObservableCollection<FilterInfo> Devices
        {
            get { return devices; }
            set { devices = value; OnPropertyChanged(); }
        }

        private VideoCaptureDevice videoSource;
        public VideoCaptureDevice VideoSource
        {
            get { return videoSource; }
            set { videoSource = value; OnPropertyChanged(); }
        }

        private FilterInfo selectedDevice;
        public FilterInfo SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                VideoSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
                OnPropertyChanged();
            }
        }

        private BarcodeReader barcodeReader;
        public BarcodeReader BarcodeReader
        {
            get { return barcodeReader; }
            set { barcodeReader = value; }
        }

        private string scannedResult;
        public string ScannedResult
        {
            get { return scannedResult; }
            set { scannedResult = value; OnPropertyChanged(); }
        }

        void InitalizeVideoManager()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                Devices.Add(device);
            }

            BarcodeReader = new BarcodeReader();
            BarcodeReader.Options.PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE};

        }

        public void StartScan()
        {
            VideoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
            IsScanStarted = true;
        }

        private bool isScanStarted;
        public bool IsScanStarted
        {
            get { return isScanStarted; }
            set { isScanStarted = value; OnPropertyChanged(); }
        }

        private BitmapSource cameraImage;
        public BitmapSource CameraImage
        {
            get { return cameraImage; }
            set { cameraImage = value; OnPropertyChanged(); }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            CameraImage = ConvertBmpToBitmapSource(bitmap);

            QRScanBitmap(bitmap);
        }

        public void StopScan()
        {
            if (VideoSource != null)
            {
                VideoSource.SignalToStop();
                VideoSource.NewFrame -= VideoSource_NewFrame;
                IsScanStarted = false;
                CameraImage = null;
            }
        }

        //need to discover other solution for this purpose
        BitmapSource ConvertBmpToBitmapSource(Bitmap bitmap)
        {
            BitmapSource source;

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = stream;
                bmpImg.EndInit();
                bmpImg.Freeze();
                source = bmpImg;
            }

            return source;
        }

        void QRScanBitmap(Bitmap bitmap)
        {
            Result result = BarcodeReader.Decode(bitmap);
            if (result != null)
            {
                ScannedResult = result.Text;
                OnVisitationChecked();
                StopScan();
            }
        }

        public event EventHandler VisitationChecked;
        public void OnVisitationChecked()
        {
            VisitationChecked?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

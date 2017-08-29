using DriveFitnessLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DriveFitness
{
    /// <summary>
    /// Логика взаимодействия для AttendanceCameraWindow.xaml
    /// </summary>
    public partial class AttendanceCameraWindow : Window
    {
        public AttendanceCameraWindow()
        {
            InitializeComponent();
            AttendanceCameraViewModel vm = new AttendanceCameraViewModel();

            this.Closing += vm.AttendanceCameraWindow_Closing;

            DataContext = vm;
            
        }
    }
}

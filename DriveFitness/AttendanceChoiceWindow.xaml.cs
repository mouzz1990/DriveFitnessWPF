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
    /// Логика взаимодействия для AttendanceChoiceWindow.xaml
    /// </summary>
    public partial class AttendanceChoiceWindow : Window
    {
        public AttendanceChoiceWindow()
        {
            InitializeComponent();
        }

        private void BtnQrCode_Click(object sender, RoutedEventArgs e)
        {
            AttendanceCameraWindow cw = new AttendanceCameraWindow();
            cw.Show();
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            AttendanceManualWindow mw = new AttendanceManualWindow();
            mw.Show();
        }
    }
}

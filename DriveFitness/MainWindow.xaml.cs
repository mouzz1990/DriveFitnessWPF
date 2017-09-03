using DriveFitnessLibrary;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriveFitness
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAttendance_Click(object sender, RoutedEventArgs e)
        {
            AttendanceChoiceWindow w = new AttendanceChoiceWindow();
            w.Show();
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
            GroupView w = new GroupView();
            w.Show();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            ClientView w = new ClientView();
            w.Show();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            ReportView w = new ReportView();
            w.Show();
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void miEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailMessageWindow emw = new EmailMessageWindow();
            emw.Show();
        }
    }
}

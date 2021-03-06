﻿using DriveFitnessLibrary;
using DriveFitnessLibrary.ViewModel;
using System.Windows.Controls.Primitives;
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
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace DriveFitness
{
    /// <summary>
    /// Логика взаимодействия для AttendanceManualWindow.xaml
    /// </summary>
    public partial class AttendanceManualWindow : Window
    {
        AttendanceManualViewModel vm;

        public AttendanceManualWindow()
        {
            InitializeComponent();
            vm =  new AttendanceManualViewModel();
            DataContext = vm;
        }
        
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (DateTime)calendar.SelectedDate;
            calendar.DisplayDate = temp.AddMonths(-3);

            calendar.DisplayDate = temp;
        }
    }
}

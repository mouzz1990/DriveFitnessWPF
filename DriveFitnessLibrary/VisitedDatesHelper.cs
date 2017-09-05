using DriveFitnessLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DriveFitnessLibrary
{
    public class VisitedDatesHelper
    {
        public static DateTime GetDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(DateProperty);
        }

        public static void SetDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
        DependencyProperty.RegisterAttached("Date", typeof(DateTime), typeof(VisitedDatesHelper), new PropertyMetadata { PropertyChangedCallback = DatePropertyChanged });

        private static void DatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var date = GetDate(d);
            SetIsVisited(d, CheckIsVisited(date));
        }

        private static bool CheckIsVisited(DateTime date)
        {
            //here we should determine whether 'date' is a holiday
            //or not and return corresponding value
            if (AttendanceManualViewModel.SelectedClientVisitedDate == null) return false;

            if (AttendanceManualViewModel.SelectedClientVisitedDate.Contains(date))
                return true;

            else return false;
        }

        private static readonly DependencyPropertyKey IsVisitedPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly("IsVisited", typeof(bool), typeof(VisitedDatesHelper), new PropertyMetadata());

        public static readonly DependencyProperty IsVisitedProperty = IsVisitedPropertyKey.DependencyProperty;

        public static bool GetIsVisited(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisitedProperty);
        }

        private static void SetIsVisited(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisitedPropertyKey, value);
        }
    }
}

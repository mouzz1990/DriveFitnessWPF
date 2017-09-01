using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DriveFitnessLibrary
{
    public class DialogManager
    {
        public bool DialogOkCancel(string message)
        {
            var result = MessageBox.Show(message, "Подтверждение операции", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
                return true;

            else return false;
        }

        public bool DialogSavePath(out string filepath)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel document|*.xls";

            if ((bool)sfd.ShowDialog())
            {
                filepath = sfd.FileName;
                return true;
            }

            filepath = "";
            return false;
        }
    }
}

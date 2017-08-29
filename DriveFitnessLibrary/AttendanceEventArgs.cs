using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveFitnessLibrary
{
    public class AttendanceEventArgs : EventArgs
    {
        public string Message { get; set; }


        public AttendanceEventArgs(string message)
        {
            Message = message;
        }

    }
}

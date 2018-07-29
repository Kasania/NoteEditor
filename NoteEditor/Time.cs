using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor
{
    class Time
    {
        //hh.mm.ss.ms
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int MiliSecond { get; set; }
        //must modify setters
        private TimeSpan TimeSpan;

        public Time(string TimeValue)
        {
            string[] tv = TimeValue.Split('.');

            Hour = int.Parse(tv[0]);
            Minute = int.Parse(tv[1]);
            Second = int.Parse(tv[2]);
            MiliSecond = int.Parse(tv[3]);
            TimeSpan = new TimeSpan(0, Hour, Minute, Second, MiliSecond); 
        }

        public override string ToString()
        {
            return string.Format("{0:D2}.{1:D2}.{2:D2}.{3:D2}", Hour, Minute, Second, MiliSecond);
            //return $"{Hour}.{Minute}.{Second}.{MiliSecond}";
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeSpan;
        }
    }
}

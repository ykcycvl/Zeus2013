using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace zeus.HelperClass
{
    public static class SystemTime
    {
        
        public struct SystemTime_

        {

            public ushort Year;

            public ushort Month;

            public ushort DayOfWeek;

            public ushort Day;

            public ushort Hour;

            public ushort Minute;

            public ushort Second;

            public ushort Millisecond;

        };

 

        [DllImport("kernel32.dll", EntryPoint = "GetSystemTime", SetLastError = true)]

        public extern static void Win32GetSystemTime(ref SystemTime_ sysTime);

 

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]

        public extern static bool Win32SetSystemTime(ref SystemTime_ sysTime);
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool SetSystemTime( [In] ref SYSTEMTIME st );

        public static bool SetSystemTime(DateTime dt)
        {
            Ipaybox.AddToLog(Ipaybox.Logs.Main,"Устанавливаем новое системное время "+dt.ToString()+" UTC");
            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = (ushort)dt.Year ; // must be short
            st.wMonth = (ushort)dt.Month;
            st.wDay = (ushort)dt.Day;
            st.wHour = (ushort)dt.Hour;
            st.wMinute = (ushort)dt.Minute;
            st.wSecond = (ushort)dt.Second;

            return SetSystemTime(ref st); // invoke this method.
        }

    }
}

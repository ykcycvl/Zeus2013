using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WD
{
    public class Sobaka
    {
        [DllImport(@"WDTS.DLL")]
        public extern static byte WDTDrvDevType();
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTGetDoorSwitch(byte DSOut);
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTSetTimer(byte sec);
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTClearTimer();
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTStopTimer();
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTResetModem();
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTResetComputer();
        [DllImport(@"WDTS.DLL")]
        public extern static UInt32 WDTResetDevice();

        public byte DrvDevType()
        {
            return WDTDrvDevType();
        }

        public bool GetDoorSwitch(byte DSOut)
        {
            if (WDTGetDoorSwitch(DSOut) == 0)
                return true;
            else
                return false;
        }

        public bool SetTimer(byte sec)
        {
            if (WDTSetTimer(sec) == 0)
                return true;
            else
                return false;
        }

        public bool ClearTimer()
        {
            if (WDTClearTimer() == 0)
                return true;
            else
                return false;
        }

        public bool StopTimer()
        {
            if (WDTStopTimer() == 0)
                return true;
            else
                return false;
        }

        public bool ResetModem()
        {
            if (WDTResetModem() == 0)
                return true;
            else
                return false;
        }

        public bool ResetComputer()
        {
            if (WDTResetComputer() == 0)
                return true;
            else
                return false;
        }
        public bool ResetDevice()
        {
            if (WDTResetDevice() == 0)
                return true;
            else
                return false;
        }

    }
}

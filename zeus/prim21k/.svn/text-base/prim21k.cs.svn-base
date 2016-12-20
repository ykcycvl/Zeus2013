using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace _prim21k
{
    class prim21k
    {
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenDLL(byte[] OpName, byte[] Psw, byte[] DevName, Int32 FlagOem);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenTCPDLL(byte[] OpName, byte[] Psw, byte[] ServerName, Int32 Port, Int32 FlagOem);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenDLLPlus(byte[] OpName, byte[] Psw, byte[] DevName, byte[] BaudRate, Int32 FlagOem);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseDLL();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 StartSeans();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ShiftOpen(byte[] Buf);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ShiftOpenEx(byte[] _CurDate, byte[] _CurTime, byte[] Buf);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ShiftClose();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ShiftCloseEx();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 XReport();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FromCash(Int32 Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FromCashEx(byte[] Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ToCash(Int32 Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ToCashEx(byte[] Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FromCashPlus(Int32 _Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ToCashPlus(Int32 _Sum, byte[] FreeField);

        /* ------------ FISCAL DOCUMENT public extern static Int32S ---------------*/

        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDoc(Byte DocType, Byte PayType, Byte FlipFOffs, Byte PageNum, Byte HCopyNum,
            Byte VCopyNum, UInt32 LOffs, UInt32 VGap, Byte LGap, Int32 Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDocEx(Byte DocType, Byte PayType, Byte FlipFOffs, Byte PageNum, Byte HCopyNum, 
            Byte VCopyNum, UInt32 LOffs, UInt32 VGap, Byte LGap, byte[] Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPosField(UInt32 SerNoLine, UInt32 SerNoCol, Byte SerNoFont,
                             UInt32 DocNoLine, UInt32 DocNoCol, Byte DocNoFont,
                             UInt32 DateLine , UInt32 DateCol, Byte DateFont,
                             UInt32 TimeLine , UInt32 TimeCol, Byte TimeFont,
                             UInt32 InnLine  , UInt32 InnCol, Byte InnFont,
                             UInt32 OperLine , UInt32 OperCol, Byte OperFont,
                             UInt32 SumLine  , UInt32 SumCol,Byte SumFont);
        [DllImport(@"Azimuth.dll")]        
        public extern static Int32 AddFreeField(UInt32 Line, UInt32 Col, Byte Font, Byte PrintMode, Byte JourNo, 
            byte[] Info);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFreeDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFiscalSlip();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFiscalReceipt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintEjournal();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDoc77(Byte DocType, Byte PayType, Byte FlipFOffs, Byte PageNum, Byte HCopyNum,
            Byte VCopyNum, UInt32 LOffs, UInt32 VGap, Byte LGap, Int32 Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDocEx77(Byte DocType, Byte PayType, Byte FlipFOffs, Byte PageNum, Byte HCopyNum,
            Byte VCopyNum, UInt32 LOffs, UInt32 VGap, Byte LGap, byte[] Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPosField77(UInt32 SerNoLine, UInt32 SerNoCol, Byte SerNoFont,
                             UInt32 DocNoLine, UInt32 DocNoCol, Byte DocNoFont,
                             UInt32 DateLine , UInt32 DateCol, Byte DateFont,
                             UInt32 TimeLine , UInt32 TimeCol, Byte TimeFont,
                             UInt32 InnLine  , UInt32 InnCol, Byte InnFont,
                             UInt32 OperationLine , UInt32 OperationCol, Byte OperationFont,
                             UInt32 KPKLine , UInt32 KPKCol, Byte KPKFont,
                             UInt32 OperLine , UInt32 OperCol, Byte OperFont,
                             UInt32 SumLine  , UInt32 SumCol, Byte SumFont);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddFreeField77(UInt32 Line, UInt32 Col, Byte Font, Byte PrintMode, Byte JourNo, 
            byte[] Info);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFreeDoc77();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFiscalSlip77();

        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetNumbers();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetEReport (Byte ReportNum, Byte Param);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetResource();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetSerialNum();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFiscalNums();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetPassword(byte[] Psw);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetDate();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetDateEx(byte[] CurDate, byte[] CurTime);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetPrezenter(Byte _IsRetrak, Byte _IsPezenter, Byte _IsSet);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetDate();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetHeader(byte[] H1, byte[] H2, byte[] H3, byte[] H4);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetHeaderNew(byte[] H1, byte[] H2, byte[] H3, byte[] H4, byte[] H5, byte[] H6);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetTail(byte[] T1, byte[] T2, byte[] T3, byte[] T4);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetOperNames(byte[] N1, byte[] N2, byte[] N3, byte[] N4, byte[] N5, byte[] N6, Int32 NCmd);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetMony();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ChangeService(Byte ServiceType, byte[] OperatorPosition);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FreeDoc(byte[] Infomation, UInt32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FreeDocCut();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFDoc(byte[] Information, UInt32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintOEMDoc(byte[] Information, UInt32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintOEMCRLFDoc(byte[] Information, UInt32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FontSelectFDoc(Byte B);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SlipSelectDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SlipEjectDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintBarcodeFDoc(Byte BType, Byte BWith, Byte BHight, Byte HRIFont, Byte HRIMode, Byte BarCodeLen, byte[] BarCode);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFDocEx(byte[] Information, UInt32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFDocPlus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SlipSelectFDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SlipEjectFDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CutFDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintHFDoc(byte[] H1, byte[] H2, byte[] H3, byte[] H4, byte[] H5, byte[] H6, Byte IsCut);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintHFDocEx(byte[] H1, byte[] H2, byte[] H3, byte[] H4, byte[] H5, byte[] H6, Byte IsCut, Byte IsGraph);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 VirtualPaperLimitFDoc(Byte H, Byte L);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetPaperLengthFDoc(byte[] P);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetPaperLengthBeforeEndFDoc(byte[] P);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GRunCommand(Byte CommandNum, byte[] CommandI);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddDept (Int32 _DepNum, byte[] _DepName);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseDept();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetDept();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetDept();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddArt(Int32 _ArtNum, Int32 _ArtFlag, byte[] _ArtName);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseArt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetArt(Int32 _DepNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetArt(Int32 _DepNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ClearDept(Byte DepNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ClearAllDept();
        [DllImport(@"Azimuth.dll")]
        public extern static UInt32 GetLastDllError();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFldsCount();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFldInt(Byte Num);
        [DllImport(@"Azimuth.dll")]
        public extern static Double GetFldFloat(Byte Num);
        [DllImport(@"Azimuth.dll")]
        public extern static String GetFldStr(Byte Num, byte[] Field);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFldWord(Byte Num);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFldByte(Byte Num);
        [DllImport(@"Azimuth.dll")]
        public extern static String GetCommand(byte[] PtrCommand);
        [DllImport(@"Azimuth.dll")]
        public extern static UInt32 GetLastCommandNum();
        [DllImport(@"Azimuth.dll")]
        public extern static String GetAnswer(byte[] PtrCommand);
        [DllImport(@"Azimuth.dll")]
        public extern static UInt32 GetAnswerSize();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetIDChar(char CharID);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetAutoID(Byte IsIDAuto);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetParameters();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetSerialAnswer();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetLastAnswer();
        [DllImport(@"Azimuth.dll")]
        public extern static String GetDllVer(byte[] DllVer);
        [DllImport(@"Azimuth.dll")]
        public extern static String GetFWVer(byte[] FWVer);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetParamDoc(UInt32 ParamDoc1, UInt32 ParamDoc2,UInt32 TimeoutSlip);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetParamDoc();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetCommTimeout(Int32 WaitRXTime, Int32 WaitTXTime);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetCommTimeoutMs(Int32 WaitRXTime, Int32 WaitTXTime);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CashDriverOpen();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetDrawerParam(Byte _OnTime, Byte _OffTime, UInt32 _ParamDoc1);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetDrawerParam();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ShowDisplay(byte[] Info, Int32 Len);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 InitDisplay();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetTaxes(Byte TIndex, Byte TType, byte[] TName, byte[] TValue, byte[] TMin);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetTaxes(Byte TIndex);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetTaxesPlus(Byte TIndex, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 DownloadGraphHeader(byte[] FName);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDocPlus(Byte _DocType, Byte _FlipFOffs, Byte _PageNum, Byte _HCopyNum, Byte _VCopyNumA,
                           UInt32 _LOffs, UInt32 _VGap, Byte _LGap, Byte _DepartNum, Byte _ArticlesNum, Int32 _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 OpenFiscalDocPlusEx( Byte _DocType, Byte _FlipFOffs, Byte _PageNum, Byte _HCopyNum, Byte _VCopyNumA,
                           UInt32 _LOffs, UInt32 _VGap, Byte _LGap, Byte _DepartNum, Byte _ArticlesNum, byte[] _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPosFieldPlus(UInt32 _SerNoLine, UInt32 _SerNoCol, Byte _SerNoFont,
                                  UInt32 _DocNoLine, UInt32 _DocNoCol, Byte _DocNoFont,
                                  UInt32 _OperNoLine, UInt32 _OperNoCol, Byte _OperNoFont,
                                  UInt32 _DateLine , UInt32 _DateCol, Byte _DateFont,
                                  UInt32 _TimeLine , UInt32 _TimeCol, Byte _TimeFont,
                                  UInt32 _InnLine  , UInt32 _InnCol, Byte _InnFont,
                                  UInt32 _OperLine , UInt32 _OperCol, Byte _OperFont,
                                  UInt32 _DepLine  , UInt32 _DepCol, Byte _DepFont,
                                  UInt32 _ArtLine  , UInt32 _ArtCol, Byte _ArtFont,
                                  UInt32 _SumLine  , UInt32 _SumCol, Byte _SumFont);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPosFieldPlus1( UInt32 _SerNoLine, UInt32 _SerNoCol, Byte _SerNoFont,
                                  UInt32 _DocNoLine, UInt32 _DocNoCol, Byte _DocNoFont,
                                  UInt32 _OperNoLine, UInt32 _OperNoCol, Byte _OperNoFont,
                                  UInt32 _DateLine , UInt32 _DateCol, Byte _DateFont,
                                  UInt32 _TimeLine , UInt32 _TimeCol, Byte _TimeFont);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPosFieldPlus2(UInt32 _InnLine, UInt32 _InnCol, Byte _InnFont,
                                   UInt32 _OperLine, UInt32 _OperCol, Byte _OperFont,
                                   UInt32 _DepLine, UInt32 _DepCol, Byte _DepFont,
                                   UInt32 _ArtLine, UInt32 _ArtCol, Byte _ArtFont,
                                   UInt32 _SumLine, UInt32 _SumCol, Byte _SumFont);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddFreeFieldPlus(UInt32 _Line, UInt32 _Col,
                                   Byte _Font, Byte _PrintMode, Byte _JourNo, byte[] _Info);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPayFieldPlus(UInt32 _Line, UInt32 _Col,
                                  Byte _Font, Byte _PayMode, Int32 _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddPayFieldPlusEx(UInt32 _Line, UInt32 _Col,
                                  Byte _Font, Byte _PayMode, byte[] _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddSubDepFieldPlus(Byte _SubDepNum, Int32 _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 AddSubDepFieldPlusex(Byte _SubDepNum, byte[] _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseFreeDocPlus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFiscalSlipPlus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 PrintFiscalReceiptPlus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 StartReceipt(Byte _DocType, Byte _Copies, 
            byte[] _TableNo, byte[] _PlaceNo, byte[] _AccountNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 StartReceiptPlus(Byte _DocType, Byte _Copies,
                               byte[] _TableNo, byte[] _PlaceNo, byte[] _AccountNo, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CommentReceipt(byte[] Buf);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemReceipt(byte[] _WareName,
                              byte[] _WareCode, byte[] _Measure, byte[] _SecID, Int32 _Price, Int32 _Count, Byte _WareType);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemReceiptPlus(byte[] _WareName, byte[] _WareCode, byte[] _Measure,
                              byte[] _SecID, byte[] FreeField, Int32 _Price, Int32 _Count, Byte _WareType);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemReceiptEx(byte[] _WareName, byte[] _WareCode, byte[] _Measure,
                              byte[] _SecID, byte[] FreeField, byte[] _Price, Int32 _Count, Byte _WareType);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemReceiptExx(byte[] _WareName, byte[] _WareCode, byte[] _Measure,
                              byte[] _SecID, byte[] FreeField, byte[] _Price, byte[] _Count, Byte _WareType);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemDepReceipt(Byte _SecID, Byte _WareName,
                                 Int32 _Price, Int32 _Count, byte[] _Measure, byte[] _WareCode);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemDepReceiptPlus(Byte _SecID, Byte _WareName,
                                 Int32 _Price, Int32 _Count, byte[] _Measure, byte[] _WareCode, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemDepReceiptEx(Byte _SecID, Byte _WareName, byte[] _Price, 
                                Int32 _Count, byte[] _Measure, byte[] _WareCode, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ItemDepReceiptExx(Byte _SecID,
                                   Byte _WareName,
                                   byte[] _Price,
                                   byte[] _Count,
                                   byte[] _Measure,
                                   byte[] _WareCode,
                                   byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TotalReceipt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TotalReceiptPlus(byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SubTotalReceipt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SubTotalReceiptPlus(byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TenderReceipt(Byte _PayType, Int32 _TenderSum, byte[] _CardName);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TenderReceiptPlus(Byte _PayType, Int32 _TenderSum, byte[] _CardName, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TenderReceiptEx(Byte _PayType, byte[] _TenderSum, byte[] _CardName, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ComissionReceipt(Byte _OType, Int32 _Percent, Int32 _Sum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ComissionReceiptPlus(Byte _OType, Int32 _Percent, Int32 _Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ComissionReceiptEx(Byte _OType, Int32 _Percent, byte[] _Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ComissionReceiptExx(Byte _OType, byte[] _Percent, byte[] _Sum, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseReceipt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CancelReceipt();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 BarcodeReceipt(Byte _BarcodeType, Byte _HRI, Byte _Font, Byte _Height, Byte _Width, byte[] _Barcode);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TaxReceipt(Byte _TaxIndex);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TaxReceiptPlus(Byte _TaxIndex, byte[] FreeField);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ChangeOpName(byte[] OpName);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 DllComWrite(byte[] P, Int32 Count);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 DllComWritePlus(byte[] P, Int32 Count);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 WriteComm(byte[] P, Int32 Count);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ReadComm(byte[] P, Int32 Count);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetCommStatus(Int32 cbRX, Int32 cbTX);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetFiscalInfo();
        [DllImport(@"Azimuth.dll")]
        public extern static String GetErrorMessage(byte[] P);
        [DllImport(@"Azimuth.dll")]
        public extern static String GetErrorMessageNo(byte[] P, UInt32 ErrNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetCommParam(byte[] DCB);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetStatus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetStatusPlus();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetStatusNo(Byte No);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetStatusNoPlus(Byte No);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetStatusNum(Int32 Num);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CheckStatusNum(Int32 Num, Int32 BitNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLActivization();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLClose();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLActivizationReport();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLEJournalTotal(Int32 EJournalNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLKPKReport(byte[] KPKNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLEJournalReport(Int32 EJournalNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLShiftsNumReport(Int32 ReportType, Int32 _StartNo, Int32 _EndNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EKLShiftsDateReport(Int32 ReportType, byte[] _StartDate, byte[] _EndDate);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ChangeBaudrate(byte[] BaudRate);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetPayment(Byte _Index, byte[] _PName, Int32 _IsSecondLine, Int32 _IsChange, 
                Byte _CurrencyIndex, Byte _PermOperation, byte[] _CrossCourse);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetPayment(Byte PaymentNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 WriteCMOS(Byte _Offs, byte[] _Info);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ReadCMOS(Byte _Offs, Byte _Num);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 ExceptToLog(byte[] Msg);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 StartMonitor();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 TerminateMonitor();
        [DllImport(@"Azimuth.dll")]
        public extern static Boolean DownLoadHex(Char MemoryChar, UInt32 StartL, UInt32 CountO, byte[] BufferP);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 FreeDocCutPlus(Byte Count);
        [DllImport(@"Azimuth.dll")]
        public extern static void LibEnable(Byte IsEnable);
        [DllImport(@"Azimuth.dll")]
        public extern static void LogEnable(Byte IsEnable);
        [DllImport(@"Azimuth.dll")]
        public extern static void CashDriverEnable(Byte IsEnable);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJPrint();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJPrintEx(UInt32 ShiftNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJErase();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJReadPage(UInt32 PageNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJTimeReport(byte[] _StartTime, byte[] _EndTime);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJDateTimeReport(byte[] _StartTime, byte[] _EndTime, byte[] _StartDate);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJNoReport(UInt32 _StartNo, UInt32 _EndNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJNoDoc(UInt32 DocNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJNoReportEx(UInt32 _StartNo, UInt32 _EndNo, Byte _OverCount);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 EJNoDocEx(UInt32 DocNo, Byte _OverCount);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetEJParam();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 Fiscalization(byte[] _OldPass, byte[] _NewPass, byte[] _NewRegNo, byte[] _NewCode, 
            Byte _Group, Byte _TotalInFlash);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetDateReport(Byte ReportType, byte[] _Pass, byte[] _StartDate, byte[] _EndDate);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetNumReport(Byte ReportType, byte[] _Pass, UInt32 _StartNo, UInt32 _EndNo);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 SetInterfaceParam(byte[] _sBaudRate, Byte _sIs5Wires, Byte _sIsDateTime);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 StartReceiptNF();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CloseReceiptNF();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 LineReceiptNF(byte[] Line);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 LinesReceiptNF(byte[] Line1, byte[] Line2, byte[] Line3, byte[] Line4, byte[] Line5,
            byte[] Line6, byte[] Line7, byte[] Line8, byte[] Line9, byte[] Line10);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetCounters();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetLastDoc(byte[] KPKNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetLastKPK();
        [DllImport(@"Azimuth.dll")]
        public extern static void SetTCPDelay(Int32 DelayTCP);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetTCPDelay();
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 Sertification(byte[] Serial);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CheckBit(Int32 Val, Int32 BitNum);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 CheckMask(Int32 Val, Int32 Mask);
        [DllImport(@"Azimuth.dll")]
        public extern static Int32 GetShiftState(Int32 ShiftState, Int32 ShiftTime);
    }

    public enum KKMModel
    { NULL, Prim21K }

    public class Prim21KNF
    {
        public string ComPort = string.Empty;
        public string LastAnswer = string.Empty;
        public int LastErrorCode = 0;
        public KKMModel Model = KKMModel.NULL;
        /// <summary>
        /// Расшифрока ошибки по коду возвращаемого значения
        /// </summary>
        public string GetError(int ErrorCode)
        {
            String hex = ErrorCode.ToString("X");
            string res = string.Empty;

            switch (hex)
            {
                case "0": res = "Ошибок нет; Счетчики обновлены"; break;
                case "01": res = "Неверный формат сообщения"; break;
                case "02": res = "Неверный формат поля"; break;
                case "03": res = "Неверное дата/время"; break;
                case "04": res = "Неверная контрольная сумма (BCC)"; break;
                case "05": res = "Неверный пароль передачи данных"; break;
                case "06": res = "Нет команды с таким номером"; break;
                case "07": res = "Необходима команда начала сеанса"; break;
                case "08": res = "Время изменилось более чем на 24 часа"; break;
                case "09": res = "Превышена максимальная длина строкового поля"; break;
                case "0A": res = "Превышена максимальная длина сообщения"; break;
                case "0B": res = "Неправильная операция"; break;
                case "0C": res = "Значение поля вне диапазона"; break;
                case "0D": res = "При данном состоянии документа эта команда не допустима"; break;
                case "0E": res = "Обязательное строковое поле имеет нулевую длину"; break;
                case "0F": res = "Слишком большой результат"; break;
                case "10": res = "Переполнение денежного счетчика"; break;
                case "11": res = "Обратная операция невозможна из-за отсутствия прямой"; break;
                case "12": res = "Нет столько наличных для выполнения операции"; break;
                case "13": res = "Обратная операция превысила итог по прямой операции"; break;
                case "14": res = "Необходимо выполнить сертификацию (ввод заводского номера)"; break;
                case "15": res = "Необходимо выполнить Z-отчет (закрытие смены)"; break;
                case "16": res = "Таймаут при печати"; break;
                case "17": res = "Неисправимая ошибка принтера"; break;
                case "18": res = "Принтер не готов к печати"; break;
                case "19": res = "Бумага близка к концу"; break;
                case "1A": res = "Необходимо провести фискализацию"; break;
                case "1B": res = "Неверный пароль налогового инспектора. Необходимо выполнить команду налогового инспектора, например, фискальный отчет, введя правильный пароль"; break;
                case "1C": res = "Регистратор уже сертифицирован"; break;
                case "1D": res = "Исчерпано число фискализаций"; break;
                case "1E": res = "Неверный буфер печати (для команды 70)"; break;
                case "1F": res = "Неверное G-поле (для команды 71/73)"; break;
                case "20": res = "Неверный номер типа оплаты"; break;
                case "21": res = "Таймаут приема"; break;
                case "22": res = "Ошибка приема"; break;
                case "23": res = "Неверное состояние регистратора"; break;
                case "24": res = "Слишком много операций в документе. Необходима команда аннулировать"; break;
                case "25": res = "Необходима команда открытие смены"; break;
                case "26": res = "Необходима команда печать электронного журнала"; break;
                case "27": res = "Неверный номер вида платежа"; break;
                case "28": res = "Неверное состояние принтера"; break;
                case "29": res = "Смена уже открыта"; break;
                case "2A": res = "Таймаут ожидания подкладного листа"; break;
                case "2B": res = "Неверная дата"; break;
                case "2C": res = "Нет места для добавления отдела/составляющей"; break;
                case "2D": res = "Индекс отдела/составляющей уже существует"; break;
                case "2E": res = "Невозможно удалить отдел, так как есть составляющие отдела"; break;
                case "2F": res = "Индекс отдела/составляющей не обнаружен"; break;
                case "30": res = "Фискальная память неисправна"; break;
                case "31": res = "Дата последней существующей записи в фискальной памяти позже, чем дата операции, которую пытались выполнить"; break;
                case "32": res = "Необходима инициализация фискальной памяти"; break;
                case "33": res = "Заполнена вся фискальная память. Блокируются все команды, кроме снятия фискальных отчетов и формирования нефискальных документов"; break;
                case "34": res = "Некорректный стартовый символ на приеме"; break;
                case "35": res = "Неопознанный ответ от ЭКЛЗ"; break;
                case "36": res = "Неизвестная команда ЭКЛЗ"; break;
                case "37": res = "Неверное состояние ЭКЛЗ"; break;
                case "38": res = "Таймаут приема от ЭКЛЗ"; break;
                case "39": res = "Таймаут передачи в ЭКЛЗ"; break;
                case "3A": res = "Неверная контрольная сумма ответа ЭКЛЗ"; break;
                case "3B": res = "Аварийное состояние ЭКЛЗ"; break;
                case "3C": res = "Переполнение ЭКЛЗ"; break;
                case "3D": res = "Неверная контрольная сумма в команде ЭКЛЗ"; break;
                case "3E": res = "Контроллер ЭКЛЗ не обнаружен"; break;
                case "3F": res = "Данные в ЭКЛЗ отсутствуют"; break;
                case "40": res = "Данные в ЭКЛЗ не синхронизированы"; break;
                case "41": res = "Аварийное состояние РИК"; break;
                case "42": res = "Неверные дата и время в команде ЭКЛЗ"; break;
                case "43": res = "Закончилось время эксплуатации ЭКЛЗ"; break;
                case "44": res = "Нет свободного места в ЭКЛЗ"; break;
                case "45": res = "Число активаций исчерпано"; break;
                case "50": res = "Неверное состояние СКЛ"; break;
                case "51": res = "Требуется печать СКЛ"; break;
                case "A0": res = "Ошибка передачи"; break;
                case "A1": res = "Ошибка приема"; break;
                case "A2": res = "Ошибка контрольной суммы на приеме"; break;
                case "A3": res = "Ошибка символа"; break;
                case "A4": res = "Ошибка структуры ответа"; break;
                case "A5": res = "Неверный порт"; break;
                case "B0": res = "Нехватка памяти"; break;
                case "B1": res = "DLL не подключена (Azimuth.dll)"; break;
                case "B2": res = "Повторное подключение DLL"; break;
                case "B3": res = "ПФ документ не открыт"; break;
                case "B4": res = "ПФ документ уже открыт"; break;
                case "B5": res = "Отсутствовала команда ADDPOSFIELD"; break;
                case "B6": res = "Ошибка открытия DLL"; break;
                case "B7": res = "Ошибка номера поля"; break;
                case "B8": res = "Ошибка типа поля"; break;
                case "B9": res = "Не введена оплата по видам"; break;
                case "BA": res = "Превышено максимальное количество оплат"; break;
                case "BB": res = "Нарушена последовательность вызовов функций библиотеки"; break;
                case "F005": res = "Ошибка при подключении к устройству"; break;
                default: res = "Неизвестная ошибка"; break;
            }

            return res;
        }
        /// <summary>
        /// Инициализация устройства
        /// </summary>
        public bool Initialize(string[] ComPorts)
        {
            bool res = false;

            try
            {
                for (int i = 0; i < ComPorts.Length; i++)
                {
                    int s = prim21k.OpenDLL(Encoding.GetEncoding(866).GetBytes(" "), Encoding.GetEncoding(866).GetBytes(""), Encoding.GetEncoding(866).GetBytes(ComPorts[i]), 0);
                    s = prim21k.GetStatus();
                    prim21k.CloseDLL();
                    LastAnswer = GetError(s);
                    LastErrorCode = s;

                    if (s == 0)
                    {
                        res = true;
                        Model = KKMModel.Prim21K;
                        ComPort = ComPorts[i];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }

            return res;
        }
        /// <summary>
        /// Печать произвольного нефискального документа
        /// </summary>
        public int PrintPND(string text)
        {
            int res = Print(text);
            LastAnswer = GetError(res);
            LastErrorCode = res;
            return res;
        }
        private int TestPrint(string text)
        {
            byte[] b = Encoding.GetEncoding(866).GetBytes(text);

            int i = prim21k.OpenDLL(Encoding.GetEncoding(866).GetBytes(" "), Encoding.GetEncoding(866).GetBytes(""), Encoding.GetEncoding(866).GetBytes(ComPort), 0);

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.GetStatus();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.GetStatus();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.FreeDoc(b, Convert.ToUInt32(b.Length));

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.FreeDocCutPlus(5);

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.CutFDoc();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            prim21k.CloseDLL();
            return i;
        }
        private int Print(string text)
        {
            byte[] b = Encoding.GetEncoding(866).GetBytes(text);

            int i = prim21k.OpenDLL(Encoding.GetEncoding(866).GetBytes(" "), Encoding.GetEncoding(866).GetBytes(""), Encoding.GetEncoding(866).GetBytes(ComPort), 0);

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.GetStatus();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.OpenFDoc();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.SlipSelectFDoc();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.PrintFDoc(b, Convert.ToUInt32(b.Length));

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.SlipEjectFDoc();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.CloseFDoc();

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            i = prim21k.FreeDocCutPlus(9);

            if (i != 0)
            {
                prim21k.CloseDLL();
                return i;
            }

            prim21k.CloseDLL();
            return i;
        }
    }
}

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace STO.Print.Utilities.PrinterHelperExtend
{
    /// <summary>
    /// ������Ҫ�Ƿ���ָ���Ӵ�ӡ����
    /// </summary>
    public class PrinterJob
    {
        /// <summary>
        /// OpenPrinter ��ָ���Ĵ�ӡ��������ȡ��ӡ���ľ�� 
        /// </summary>
        /// <param name="szPrinter">Ҫ�򿪵Ĵ�ӡ��������</param>
        /// <param name="hPrinter">����װ�ش�ӡ���ľ��</param>
        /// <param name="pd">PRINTER_DEFAULTS������ṹ����Ҫ����Ĵ�ӡ����Ϣ</param>
        /// <returns>bool</returns>
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);
        /// <summary>
        /// ClosePrinter �ر�һ���򿪵Ĵ�ӡ������
        /// </summary>
        /// <param name="hPrinter">һ���򿪵Ĵ�ӡ������ľ��</param>
        /// <returns></returns>
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        /// <summary>
        /// StartDocPrinter �ں�̨��ӡ�ļ�������һ�����ĵ�
        /// </summary>
        /// <param name="hPrinter">ָ��һ���Ѵ򿪵Ĵ�ӡ���ľ������openprinterȡ�ã�</param>
        /// <param name="level">1��2��������win95��</param>
        /// <param name="di">����һ��DOC_INFO_1��DOC_INFO_2�ṹ�û�����</param>
        /// <returns>bool ע: ��Ӧ�ó���ļ��𲢷����á���̨��ӡ����������ʶһ���ĵ��Ŀ�ʼ</returns>
        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);
        /// <summary>
        /// EndDocPrinter �ں�̨��ӡ����ļ���ָ��һ���ĵ��Ľ���
        /// </summary>
        /// <param name="hPrinter">һ���Ѵ򿪵Ĵ�ӡ���ľ��������OpenPrinter��ã�</param>
        /// <returns>bool</returns>
        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);
        /// <summary>
        /// StartPagePrinter �ڴ�ӡ��ҵ��ָ��һ����ҳ�Ŀ�ʼ 
        /// </summary>
        /// <param name="hPrinter">ָ��һ���Ѵ򿪵Ĵ�ӡ���ľ������openprinterȡ�ã�</param>
        /// <returns>boolע:��Ӧ�ó���ļ��𲢷��ر�����</returns>
        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);
        /// <summary>
        /// EndPagePrinter ָ��һ��ҳ�ڴ�ӡ��ҵ�еĽ�β 
        /// </summary>
        /// <param name="hPrinter">һ���Ѵ򿪵Ĵ�ӡ������ľ������OpenPrinter��ã�</param>
        /// <returns>bool</returns>
        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);
        /// <summary>
        /// WritePrinter ������Ŀ¼�е�����д���ӡ�� 
        /// </summary>
        /// <param name="hPrinter">ָ��һ���Ѵ򿪵Ĵ�ӡ���ľ������openprinterȡ�ã�</param>
        /// <param name="pBytes">�κ����ͣ�������Ҫд���ӡ�������ݵ�һ����������ṹ</param>
        /// <param name="dwCount">dwCount�������ĳ���</param>
        /// <param name="dwWritten">ָ��һ��Long�ͱ���������װ��ʵ��д����ֽ���</param>
        /// <returns>bool</returns>
        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);
        /// <summary>
        /// GetPrinter ȡ����ָ����ӡ���йص���Ϣ
        /// </summary>
        /// <param name="handle">һ���Ѵ򿪵Ĵ�ӡ���ľ������OpenPrinter��ã�</param>
        /// <param name="level">1��2��3����������NT����4����������NT��������5����������Windows 95 �� NT 4.0��</param>
        /// <param name="buffer">����PRINTER_INFO_x�ṹ�Ļ�������x������</param>
        /// <param name="size">pPrinterEnum�������е��ַ�����</param>
        /// <param name="sizeNeeded">ָ��һ��Long�ͱ�����ָ�룬�ñ������ڱ�������Ļ��������ȣ�����ʵ�ʶ�����ֽ�����</param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetPrinter(IntPtr handle, UInt32 level, IntPtr buffer, UInt32 size, out UInt32 sizeNeeded);
        /// <summary>
        /// EnumPrinters ö��ϵͳ�а�װ�Ĵ�ӡ��
        /// </summary>
        /// <param name="flags">һ������������־
        /// PRINTER_ENUM_LOCAL ö�ٱ��ش�ӡ��������Windows 95�е������ӡ���������ֻᱻ���� 
        ///PRINTER_ENUM_NAME ö����name����ָ���Ĵ�ӡ�������е����ֿ�����һ����Ӧ�̡�������������nameΪNULL����ö�ٳ����õĴ�ӡ�� 
        ///PRINTER_ENUM_SHARE ö�ٹ����ӡ��������ͬ�����������ʹ�ã� 
        ///PRINTER_ENUM_CONNECTIONS ö�����������б��еĴ�ӡ������ʹĿǰû�����ӡ�����������NT�� 
        ///PRINTER_ENUM_NETWORK ö��ͨ���������ӵĴ�ӡ��������Level������Ϊ1����������NT 
        ///PRINTER_ENUM_REMOTE ö��ͨ���������ӵĴ�ӡ���ʹ�ӡ���������������Ϊ1����������NT 
        ///</param>
        /// <param name="name">null��ʾö��ͬ�������ӵĴ�ӡ���������ɱ�־�ͼ������</param>
        /// <param name="level">1��2��4��5��4��������NT��5��������Win95��NT 4.0����ָ����ö�ٵĽṹ�����͡������1����name�����ɱ�־���þ����������2��5����ôname�ʹ����������ӡ������ö�ٵķ����������֣�����ΪvbNullString�������4����ôֻ��PRINTER_ENUM_LOCAL��PRINTER_ENUM_CONNECTIONS����Ч�����ֱ�����vbNullString</param>
        /// <param name="pPrinterEnum">����PRINTER_ENUM_x�ṹ�Ļ����������е�x������Level��</param>
        /// <param name="cbBuf">pPrinterEnum�������е��ַ�����</param>
        /// <param name="pcbNeeded">ָ��һ��out Long�ͱ������ñ������ڱ�������Ļ��������ȣ�����ʵ�ʶ�����ֽ�����</param>
        /// <param name="pcReturned">���뻺�����Ľṹ������������Щ�ܷ��ض���ṹ�ĺ�����</param>
        /// <returns>bool</returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool EnumPrinters(int flags, string name, int level, IntPtr pPrinterEnum, int cbBuf, out int pcbNeeded, out int pcReturned);
        /// <summary>
        /// ��ȡ��ָ����ҵ�йص���Ϣ
        /// </summary>
        /// <param name="hPrinter">һ���Ѵ򿪵Ĵ�ӡ���ľ������OpenPrinter��ã�</param>
        /// <param name="JobId">��ҵ���</param>
        /// <param name="level">1��2��3����������NT����4����������NT��������5����������Windows 95 �� NT 4.0��</param>
        /// <param name="pPrinterEnum">����PRINTER_INFO_x�ṹ�Ļ�������x������</param>
        /// <param name="cbBuf">pPrinterEnum�������е��ַ�����</param>
        /// <param name="pcbNeeded">ָ��һ��uint32�ͱ�����ָ�룬�ñ������ڱ�������Ļ��������ȣ�����ʵ�ʶ�����ֽ�����</param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool GetJob(IntPtr hPrinter, UInt32 JobId, UInt32 level, IntPtr pPrinterEnum, UInt32 cbBuf, out UInt32 pcbNeeded);
        /// <summary>
        /// ö�ٴ�ӡ�����е���ҵ
        /// </summary>
        /// <param name="hPrinter">һ���Ѵ򿪵Ĵ�ӡ������ľ������OpenPrinter��ã�</param>
        /// <param name="FirstJob">��ҵ�б���Ҫö�ٵĵ�һ����ҵ��������ע���Ŵ�0��ʼ��</param>
        /// <param name="NoJobs">Ҫö�ٵ���ҵ����</param>
        /// <param name="level">1��2</param>
        /// <param name="pJob">���� JOB_INFO_1 �� JOB_INFO_2 �ṹ�Ļ�����</param>
        /// <param name="cbBuf">pJob�������е��ַ�����</param>
        /// <param name="pcbNeeded">ָ��һ��Uint32�ͱ�����ָ�룬�ñ������ڱ�������Ļ��������ȣ�����ʵ�ʶ�����ֽ�����</param>
        /// <param name="pcReturned">���뻺�����Ľṹ������������Щ�ܷ��ض���ṹ�ĺ�����</param>
        /// <returns>bool</returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool EnumJobs(IntPtr hPrinter, UInt32 FirstJob, UInt32 NoJobs, UInt32 level, IntPtr pJob, UInt32 cbBuf, out UInt32 pcbNeeded, out UInt32 pcReturned);
        /// <summary>
        /// �ύһ��Ҫ��ӡ����ҵ
        /// </summary>
        /// <param name="hPrinter">һ̨�Ѵ򿪵Ĵ�ӡ�����</param>
        /// <param name="JobID">��ҵ���</param>
        /// <returns>bool</returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool ScheduleJob(IntPtr hPrinter, out UInt32 JobID);

        /// <summary>
        /// ȡ����ָ�����йص���Ϣ
        /// </summary>
        /// <param name="hPrinter">��ӡ���ľ��</param>
        /// <param name="pFormName">���ȡ��Ϣ��һ����������</param>
        /// <param name="Level">��Ϊ1</param>
        /// <param name="pForm">����FORM_INFO_1�ṹ�Ļ�����</param>
        /// <param name="cbBuf">pForm�������е��ַ�����</param>
        /// <param name="pcbNeeded">ָ��һ��Long�ͱ�����ָ�룬�ñ������ڱ�������Ļ��������ȣ�����ʵ�ʶ�����ֽ�����</param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool GetForm(IntPtr hPrinter, string pFormName, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded);

        /// <summary>
        /// ö��һ̨��ӡ�����õı�
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="Level"></param>
        /// <param name="pForm"></param>
        /// <param name="cbBuf"></param>
        /// <param name="pcbNeeded"></param>
        /// <param name="pcReturned"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool EnumForms(IntPtr hPrinter, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded, out��UInt32 pcReturned);

        /// <summary>
        /// Ϊϵͳ���һ����ӡ��������
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="Level"></param>
        /// <param name="pMonitors"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool AddMonitor(IntPtr pName, UInt32 Level, IntPtr pMonitors);

        /// <summary>
        /// ö�ٿ��õĴ�ӡ������
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="Level"></param>
        /// <param name="pForm"></param>
        /// <param name="cbBuf"></param>
        /// <param name="pcbNeeded"></param>
        /// <param name="pcReturned"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool EnumMonitors(string hPrinter, UInt32 Level, IntPtr pForm, UInt32 cbBuf, out UInt32 pcbNeeded, out��UInt32 pcReturned);

        /// <summary>
        /// ���ָ���Ĵ�ӡ������ȡ���ӡ�����������йص���Ϣ
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="pEnvironment"></param>
        /// <param name="Level"></param>
        /// <param name="pDriverInfo"></param>
        /// <param name="cbBuf"></param>
        /// <param name="pcbNeeded"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool GetPrinterDriver(IntPtr pName, string pEnvironment, UInt32 Level, IntPtr pDriverInfo, UInt32 cbBuf, out  UInt32 pcbNeeded);

        /// <summary>
        /// һ�������豸���ƺ���
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="nEscape"></param>
        /// <param name="nCount"></param>
        /// <param name="lpInData"></param>
        /// <param name="lpOutData"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern short Escape(IntPtr pName, UInt32 nEscape, UInt32 nCount, IntPtr lpInData, out IntPtr lpOutData);

        /// <summary>
        /// ����һ�����Ĵ�ӡ�����ÿ��ƺ�����
        /// �ú�������������DEVMODE�ṹ�����ڴ���һ���豸����ʱΪ����Ӧ�ó���ı��ӡ�����á�
        /// ���������ĵ���ӡ�ڼ�ı��ӡ������
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hPrinter"></param>
        /// <param name="pDeviceName"></param>
        /// <param name="pDevModeOutput"></param>
        /// <param name="pDevModeInput"></param>
        /// <param name="fMode"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern bool DocumentProperties(IntPtr hwnd, IntPtr hPrinter, string pDeviceName, out IntPtr pDevModeOutput, out IntPtr pDevModeInput, int fMode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", EntryPoint = "EndDoc", CharSet = CharSet.Auto)]
        public static extern short EndDocAPI(IntPtr hPrinter);


        #region ����Ľṹ��

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public UInt32 Attributes;
            public UInt32 Priority;
            public UInt32 DefaultPriority;
            public UInt32 StartTime;
            public UInt32 UntilTime;
            public UInt32 Status;
            public UInt32 cJobs;
            public UInt32 AveragePPM;

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PRINTER_INFO_1
        {
            public int flags;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDescription;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RECT
        {
            public UInt32 Left;
            public UInt32 Top;
            public UInt32 Right;
            public UInt32 Bottom;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct FORM_INFO_1
        {
            public UInt32 Flags;
            public string pName;
            public UInt32 Size;
            public IntPtr ImageableArea;

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONINTOR_INFO_2
        {
            public string pName;
            public string pEnvironment;
            public string pDLLName;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct JOB_INFO_1
        {
            public UInt32 Jobid;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pPrinterName;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pMachineName;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pUserName;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pDocument;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pDatatype;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pStatus;
            public UInt32 Status;
            public UInt32 Priority;
            public UInt32 Position;
            public UInt32 TotalPages;
            public UInt32 PagesPrinted;
            public IntPtr Submitted;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct JOB_INFO_2
        {
            public int Jobid;
            public string pPrinterName;
            public string pMachineName;
            public string pUserName;
            public string pDocument;
            public string pNotifyName;
            public string pDatatype;
            public string pPrintProcessor;
            //[MarshalAs(UnmanagedType.LPStr)]
            public string pParameters;
            public string pDriverName;
            public IntPtr pDevMode;
            public string pStatus;
            public IntPtr pSecurityDescriptor;
            public int Status;
            public int Priority;
            public int Position;
            public int StartTime;
            public int UntilTime;
            public int TotalPages;
            public int Size;
            public IntPtr Submitted;
            public int Time;
            public int PagesPrinted;

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITOR_INFO_1
        {
            public string pName;
            //public string pEnvironment;
            //public string pDLLName; 

        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DRIVER_INFO_1
        {
            public string pName;
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DRIVER_INFO_2
        {
            public int cVersion;
            public string pName;
            public string pEnvironment;
            public string pDriverPath;
            public string pDataFile;
            public string pConfigFile;

        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DRIVER_INFO_3
        {
            public int cVersion;
            public string pName;
            public string pEnvironment;
            public string pDriverPath;
            public string pDataFile;
            public string pConfigFile;
            public string pHelpFile;
            public string pDependentFiles;
            public string pMonitorName;
            public string pDefaultDataType;
        };

        #endregion

        /// <summary>
        /// Ϊר���豸�����豸����
        /// </summary>
        /// <param name="pDrive"></param>
        /// <param name="pName"></param>
        /// <param name="pOutput"></param>
        /// <param name="pDevMode"></param>
        /// <returns></returns>
        [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false,
                    CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)] string pDrive,
            [MarshalAs(UnmanagedType.LPTStr)] string pName,
            [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
           ref DEVMODE pDevMode);


        /// <summary>
        /// Ϊר���豸����һ����Ϣ������
        /// ��Ϣ�������������ٻ�ȡĳ�豸����Ϣ�����봴���豸����������ϵͳ������
        /// ������Ϊ�������ݸ�GetDeviceCapsһ�����Ϣ����������豸��������
        /// </summary>
        /// <param name="pDrive">��vbNullString����nullֵ���ò�����
        /// ���ǣ�1����DISPLAY���ǻ�ȡ������Ļ���豸������2����WINSPOOL�����Ƿ��ʴ�ӡ����</param>
        /// <param name="pName"></param>
        /// <param name="pOutput"></param>
        /// <param name="pDevMode"></param>
        /// <returns></returns>
        [DllImport("GDI32.dll", EntryPoint = "CreateIC", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false,
                    CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateIC([MarshalAs(UnmanagedType.LPTStr)]
            string pDrive,
            [MarshalAs(UnmanagedType.LPTStr)] string pName,
            [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
           ref DEVMODE pDevMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool Rectangle(IntPtr hwnd, int x, int y, int x1, int y1);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateEnhMetaFile(IntPtr hdcRef, string lpFileName, ref RECT lpRect, string lpDescription);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CloseEnhMetaFile(IntPtr hdcRef);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CopyEnhMetaFile(IntPtr hdcRef, string lpszFile);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hdcRef);


        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string name);

        public struct TempFile
        {
            public string pFullName;
            public long leng;
            public string pName;
            //public string pEnvironment;
            //public string pDLLName; 

        };

        #region �������ݣ������ҵ

        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "�����ĵ�";
            di.pDataType = "TEXT";  //RAW     �������ӡ���ϲ��Ա��뽫pDataType = "RAW"  ��Ϊ TEXT

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);

                }
                //int aa= EndDocAPI(hPrinter);

                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        #endregion
    }

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace TimeMonitor
{

    public class WinAPIFunctions
    {
        //Used to get Handle for Foreground Window
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        //Used to get ID of any Window
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        public static int GetWindowProcessId(IntPtr hwnd)
        {
            int pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        public static IntPtr GetforegroundWindow()
        {
            return GetForegroundWindow();
        }
    }

    class FindHostedProcess
    {
        static private Process _realProcess;

        static public Process Find()
        {
            var foregroundProcess = Process.GetProcessById(WinAPIFunctions.GetWindowProcessId(WinAPIFunctions.GetforegroundWindow()));
            if (foregroundProcess.ProcessName == "ApplicationFrameHost")
            {
                foregroundProcess = GetRealProcess(foregroundProcess);
            }
            return foregroundProcess;
        }

        static private Process GetRealProcess(Process foregroundProcess)
        {
            WinAPIFunctions.EnumChildWindows(foregroundProcess.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
            return _realProcess;
        }

        static private bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            var process = Process.GetProcessById(WinAPIFunctions.GetWindowProcessId(hwnd));
            if (process.ProcessName != "ApplicationFrameHost")
            {
                _realProcess = process;
            }
            return true;
        }
    }

    /// <summary>
    /// GetForegroundWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GetForeground : Window
    {
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return wpfBitmap;
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        [DllImport(
        "Kernel32.dll",
        EntryPoint = "QueryFullProcessImageNameW",
        CallingConvention = CallingConvention.StdCall,
        CharSet = CharSet.Unicode)]
        extern static int QueryFullProcessImageName(
            IntPtr hProcess,
            UInt32 flags,
            char[] exeName,
            ref UInt32 nameLen);

        public static int GetCurrentProcessID(IntPtr IP)
        {
            int oo;
            GetWindowThreadProcessId(IP, out oo);
            return oo;
        }
        DispatcherTimer DT = new DispatcherTimer();
        public GetForeground()
        {
            InitializeComponent();
            DT.Interval = new TimeSpan(0, 0, 1);
            DT.Tick += new EventHandler(ShowForegroundWindow);
            DT.Start();
        }
        private void ShowForegroundWindow(object sender, EventArgs e)
        {
            var FW = GetForegroundWindow();
            /*
            StringBuilder SB = new StringBuilder();
            //char[] filepath = new char[10000];
            //uint maxfilepathlength = 1000;
            try
            {
                GetWindowText(FW, SB, 1000);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            */
            //Debug.Write(SB);
            //QueryFullProcessImageName(new IntPtr(GetCurrentProcessID(FW)), 0, filepath, ref maxfilepathlength);
            var P = Process.GetProcessById(GetCurrentProcessID(FW));
            var PP = FindHostedProcess.Find();
            if (PP == null) return;
            TB.Text = PP.ProcessName + "|" + PP.MainModule.FileName + "|" + PP.MainWindowTitle;
            Bitmap img = System.Drawing.Icon.ExtractAssociatedIcon(PP.MainModule.FileName).ToBitmap();
            IMG.Source = ChangeBitmapToImageSource(img);
        }
    }
}

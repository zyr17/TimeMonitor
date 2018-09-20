using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

    class CheckForeground
    {
        static DispatcherTimer DT = new DispatcherTimer();
        static bool IsInited = false;
        static Data.Actions LastActions = null;

        static void Init()
        {
            IsInited = true;
            DT.Tick += ShowForegroundWindow;
            DT.Interval = Consts.CheckForegroundTimeSpan;
        }

        public static void Start()
        {
            if (!IsInited)
                Init();
            DT.Start();
        }

        public static void ShowForegroundWindow(object sender, EventArgs e)
        {
            try
            {
                var PP = FindHostedProcess.Find();
                if (PP == null) return;
                //TB.Text = PP.ProcessName + "|" + PP.MainModule.FileName + "|" + PP.MainWindowTitle;
                Icon i = System.Drawing.Icon.ExtractAssociatedIcon(PP.MainModule.FileName);
                Data.Actions A = new Data.Actions();
                A.DateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                A.Type = 0;
                A.Action = PP.MainModule.FileName;
                A.Title = PP.MainWindowTitle;
                A.Icon = i;
                if (LastActions == null || LastActions.Action != A.Action || LastActions.Title != A.Title)
                {
                    Data.AddData(A);
                    LastActions = A;
                }
            }
            catch
            {
                
            }
        }
    }
}

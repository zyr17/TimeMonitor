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

    

    

    /// <summary>
    /// GetForegroundWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GetForeground : Window
    {
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
            //Bitmap img = Consts.Base64String2Icon(Consts.Icon2Base64String(System.Drawing.Icon.ExtractAssociatedIcon(PP.MainModule.FileName))).ToBitmap();
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(PP.MainModule.FileName);
            Icon icon2 = Consts.Base64String2Icon(Consts.Icon2Base64String(icon));
            IMG.Source = Consts.ChangeBitmapToImageSource(icon2.ToBitmap());
        }
    }
}

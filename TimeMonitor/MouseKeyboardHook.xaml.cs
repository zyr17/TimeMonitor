using System;
using System.Collections.Generic;
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
    /// MouseKeyboardHook.xaml 的交互逻辑
    /// </summary>
    public partial class MouseKeyboardHook : Window
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        public const int WH_GETMESSAGE = 3;
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        static int hHook = 0;
        HookProc DeviceHookProcedure;
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        //Import for UnhookWindowsHookEx.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //Import for CallNextHookEx.
        //Use this function to pass the hook information to next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);
        DispatcherTimer T = new DispatcherTimer();
        public MouseKeyboardHook()
        {
            InitializeComponent();
            T.Interval = new TimeSpan(0, 0, 3);
            T.Tick += ClearText;
        }

        private void ClearText(object sender, EventArgs e)
        {
            TBLeft.Text = TBRight.Text = "";
        }

        public int DeviceHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            else
            {
                if (wParam.ToInt32() == DBT_DEVICEARRIVAL)
                    TBLeft.Text = "there is a USB plugged in";
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeviceHookProcedure = new HookProc(DeviceHookProc);
            hHook = SetWindowsHookEx(WH_GETMESSAGE,
                        DeviceHookProcedure,
                        (IntPtr)0,
                       0);
            if (hHook == 0)
            {
                MessageBox.Show("SetWindowsHookEx Failed");
                return;
            }

        }
    }
}

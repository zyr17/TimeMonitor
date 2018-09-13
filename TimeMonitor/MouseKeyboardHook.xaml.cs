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
        DispatcherTimer T = new DispatcherTimer();
        public MouseKeyboardHook()
        {
            InitializeComponent();
            T.Interval = new TimeSpan(0, 0, 3);
            T.Tick += ClearText;
            T.Start();
        }

        private void ClearText(object sender, EventArgs e)
        {
            TBLeft.Text = TBRight.Text = "";
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MouseHook.AddEvent(MouseEvent);
            MouseHook.Start();
            KeyboardHook.AddEvent(KeyboardEvent);
            KeyboardHook.Start();
        }

        private void KeyboardEvent(ref KeyboardHook.StateKeyboard state)
        {
            TBRight.Text = state.Key + " " + state.Time;
        }

        private void MouseEvent(ref MouseHook.StateMouse state)
        {
            TBLeft.Text = state.button + " " + state.posX + " " + state.posY;
        }
    }
}

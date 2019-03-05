using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// KeepAlive.xaml 的交互逻辑
    /// </summary>
    public partial class KeepAlive : Window
    {
        DispatcherTimer AliveTimer = new DispatcherTimer();
        public KeepAlive()
        {
            InitializeComponent();

            AliveTimer.Interval = Consts.ActiveTimeSpan - new TimeSpan(0, 0, 1);
            AliveTimer.Tick += AliveTimer_Tick;
            Deactivated += KeepAlive_Deactivated;
            Closing += MainWindow_Closing;
        }

        private void KeepAlive_Deactivated(object sender, EventArgs e)
        {
            Debug.Write("KA deactivated");
            HideWindow();
        }

        private void AliveTimer_Tick(object sender, EventArgs e)
        {
            Data.Actions actions = new Data.Actions
            {
                DateTime = DateTime.Now,
                Type = 1,
                Action = "Fake",
                Title = "Fake"
            };
            Data.AddActions(actions);
        }

        public void KeepAliveStart(string s)
        {
            MainTextBlock.Text = "正在进行 " + s;
            Title = s;
            AliveTimer.Start();
            Show();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            HideWindow();
        }

        private void HideWindow()
        {
            AliveTimer.Stop();
            Hide();
        }
    }
}

using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Drawing;
using System.Data.SQLite;

namespace TimeMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        NotifyIconComponent NIC;
        DateTimeSelector DTS;

        public MainWindow()
        {
            InitializeComponent();
            DTS = new DateTimeSelector();
            NIC = new NotifyIconComponent();
            NIC.NotifyIconText = "一个静悄悄的图标";
            Closing += MainWindow_Closing;
            NIC.ContextMenuStripItems.Add("显示/隐藏主界面");
            NIC.ContextMenuStripItems[0].Click += Visibility_Click;
            NIC.ContextMenuStripItems[0].Font = new Font(NIC.ContextMenuStripItems[0].Font, NIC.ContextMenuStripItems[0].Font.Style | System.Drawing.FontStyle.Bold);
            NIC.ContextMenuStripItems.Add("退出");
            NIC.ContextMenuStripItems[1].Click += Exit_Click;
            NIC.NotifyIconDoubleClick = Visibility_Click;
            NIC.NotifyIconIcon = Consts.Base64String2Icon(Consts.clockstr);
            Hook.Start();
            CheckForeground.Start();
        }

        private void Visibility_Click(object sender, EventArgs e)
        {
            WindowVisibilityChange();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            NIC.NotifyIconVisibility = false;
            Environment.Exit(0);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            WindowVisibilityChange();
        }

        void WindowVisibilityChange()
        {
            if (Visibility == Visibility.Hidden) Visibility = Visibility.Visible;
            else Visibility = Visibility.Hidden;
        }

        double delta = 0.11356;
        double start = 0.05;

        private void StartTimeSelectButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            double l = SectorGrid.ActualWidth / 2, t = SectorGrid.ActualHeight / 2;
            double r = (l > t ? t : l) / 1.2;
            double st = start, ed = start + delta;
            start += delta;
            SectorPath TM = new SectorPath(l, t, r, st, ed);
            SectorGrid.Children.Add(TM.GetPath);
            l = RectangleGrid.ActualWidth / 20;
            t = RectangleGrid.ActualHeight / 20;
            double w = l * 18;
            double h = t * 18;
            RectanglePath RP = new RectanglePath(l, t, w, h, st, ed);
            RectangleGrid.Children.Add(RP.GetPath);
            */
            string SelectedTime = GetTime();
            if (SelectedTime != null)
            {
                var strs = SelectedTime.Split(' ');
                StartYMDTextBlock.Text = strs[0];
                StartHMSTextBlock.Text = strs[1];
            }
        }

        string GetTime()
        {
            DTS = new DateTimeSelector();
            DTS.ShowDialog();
            return DTS.TimeResult;
        }

        private void EndTimeSelectButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedTime = GetTime();
            if (SelectedTime != null)
            {
                var strs = SelectedTime.Split(' ');
                EndYMDTextBlock.Text = strs[0];
                EndHMSTextBlock.Text = strs[1];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dis = ((Button)sender).TranslatePoint(new System.Windows.Point(0, 0), ListStackPanel).Y;
            ListScrollViewer.ScrollToVerticalOffset(dis);
            Mouse.GetPosition(ListStackPanel);
        }
    }
}

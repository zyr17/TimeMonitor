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
using System.Windows.Forms;

namespace TimeMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        NotifyIconComponent NIC;

        public MainWindow()
        {
            InitializeComponent();
            NIC = new NotifyIconComponent();
            NIC.NotifyIconText = "一个静悄悄的图标";
            Closing += MainWindow_Closing;
            NIC.ContextMenuStripItems.Add("显示/隐藏主界面");
            NIC.ContextMenuStripItems[0].Click += Visibility_Click;
            NIC.ContextMenuStripItems.Add("退出");
            NIC.ContextMenuStripItems[1].Click += Exit_Click;
            NIC.NotifyIconIcon = new Icon("clock.ico");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var W = new GetForeground();
            W.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var W = new MouseKeyboardHook();
            W.Show();
        }

        double delta = 0.6;
        double start = 0.05;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var myPath = new Path();
            myPath.Fill = System.Windows.Media.Brushes.MediumSlateBlue;
            myPath.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myPath.VerticalAlignment = VerticalAlignment.Top;
            PathGeometry myGeometry = new PathGeometry();
            var oy = MainGrid.ActualHeight / 2;
            var ox = MainGrid.ActualWidth / 2;
            var R = MainGrid.ActualHeight;
            if (MainGrid.ActualWidth < R) R = MainGrid.ActualWidth;
            R /= 2;
            PathFigure PF = new PathFigure();
            PF.StartPoint = new System.Windows.Point(ox, oy);
            double startpos = start, endpos = delta + start;
            start = endpos;
            PF.Segments.Add(new LineSegment(new System.Windows.Point(ox + Math.Cos(Math.PI * 2 * startpos) * R, oy + Math.Sin(Math.PI * 2 * startpos) * R), false));
            PF.Segments.Add(new ArcSegment(new System.Windows.Point(ox + Math.Cos(Math.PI * 2 * endpos) * R, oy + Math.Sin(Math.PI * 2 * endpos) * R), new System.Windows.Size(R, R), 0, (endpos - startpos > 0.5 ? true : false), SweepDirection.Clockwise, false));
            PF.Segments.Add(new LineSegment(new System.Windows.Point(ox, oy), false));
            myGeometry.Figures.Add(PF);
            myPath.Data = myGeometry;
            MainGrid.Children.Add(myPath);
        }
    }
}

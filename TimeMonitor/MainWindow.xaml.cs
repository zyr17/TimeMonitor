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

        List<Data.ShowData> ShowDataList = new List<Data.ShowData>();
        List<Data.ShowData> SectorDataList = new List<Data.ShowData>();
        List<Path> RectanglePathList = new List<Path>();
        List<Path> SectorPathList = new List<Path>();
        NotifyIconComponent NIC;
        DateTimeSelector DTS;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            Loaded += Initialize_ShowData;

            Hook.Start();
            CheckForeground.Start();
        }

        private void Initialize_ShowData(object sender, EventArgs e)
        {
            UpdateByShowData();
        }

        private void GenerateNIC()
        {
            NIC = new NotifyIconComponent();
            NIC.NotifyIconText = "一个静悄悄的图标";
            NIC.ContextMenuStripItems.Add("显示/隐藏主界面");
            NIC.ContextMenuStripItems[0].Click += Visibility_Click;
            NIC.ContextMenuStripItems[0].Font = new Font(NIC.ContextMenuStripItems[0].Font, NIC.ContextMenuStripItems[0].Font.Style | System.Drawing.FontStyle.Bold);
            NIC.ContextMenuStripItems.Add("退出");
            NIC.ContextMenuStripItems[1].Click += Exit_Click;
            NIC.NotifyIconDoubleClick = Visibility_Click;
            NIC.NotifyIconIcon = Consts.Base64String2Icon(Consts.clockstr);
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

        double GetShowData()
        {
            ShowDataList.Clear();
            SectorDataList.Clear();

            var SD = new Data.ShowData
            {
                I = Consts.Base64String2Icon(Consts.xstr),
                Start = new DateTime(2018, 7, 7, 12, 0, 0),
                End = DateTime.Now,
                Action = @"E:/dididi bababa (x86)/explorer.exe",
                Title = "TimeMonitor",
            };
            for (int i = 10; i > 0; i--)
            {
                SD.End = SD.Start.AddHours(i);
                ShowDataList.Add(SD);
                var TSD = new Data.ShowData(SD);
                SD = TSD;
                SD.Start = SD.End.AddHours(0);
                if (i < 10) SD.Title = "Chrome";
                if (i < 9) SD.Action = @"F:\123\notepad.exe";
            }

            double totalTime = (ShowDataList.Last().End - ShowDataList[0].Start).Ticks;

            foreach (var data in ShowDataList)
            {
                bool newdata = true;
                foreach (var old in SectorDataList)
                {
                    if (old.Action == data.Action && old.Title == data.Title)
                    {
                        newdata = false;
                        old.Ticks += data.Ticks;
                        data.Color = old.Color;
                    }
                }
                if (newdata)
                {
                    data.Color = Consts.NextBrush;
                    var TSD = new Data.ShowData(data);
                    SectorDataList.Add(TSD);
                }
            }
            SectorDataList.Sort((Data.ShowData x, Data.ShowData y) => {
                return y.Ticks.CompareTo(x.Ticks);
            });
            return totalTime;
        }
        
        void UpdateByShowData()
        {
            var totalTime = GetShowData();
            
            ListStackPanel.Children.Clear();
            RectanglePathList.Clear();
            SectorPathList.Clear();
            RectangleGrid.Children.Clear();
            SectorGrid.Children.Clear();
            
            double sectorst = 0;
            foreach (var data in SectorDataList)
            {
                double l = SectorGrid.ActualWidth / 2, t = SectorGrid.ActualHeight / 2;
                double r = (l > t ? t : l) / 1.2;
                double ed = sectorst + (data.End - data.Start).Ticks / totalTime;
                SectorPath SP = new SectorPath(l, t, r, sectorst - 0.25, ed - 0.25, data.Color);
                sectorst = ed;

                SectorPathList.Add(SP.GetPath);
                SectorGrid.Children.Add(SectorPathList.Last());
            }

            foreach (var data in ShowDataList)
            {
                ListStackPanelItem LSPI = new ListStackPanelItem(data);
                ListStackPanel.Children.Add(LSPI);
                
                double l = RectangleGrid.ActualWidth / 20;
                double t = RectangleGrid.ActualHeight / 20;
                double w = l * 18;
                double h = t * 18;
                double st = (data.Start - ShowDataList[0].Start).Ticks / totalTime;
                double ed = st + (data.End - data.Start).Ticks / totalTime;
                RectanglePath RP = new RectanglePath(l, t, w, h, st, ed, data.Color);

                RectanglePathList.Add(RP.GetPath);
                RectangleGrid.Children.Add(RectanglePathList.Last());
            }

            
        }

    }
}

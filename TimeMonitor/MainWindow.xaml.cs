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
        List<RectanglePath> RectanglePathList = new List<RectanglePath>();
        List<SectorPath> SectorPathList = new List<SectorPath>();
        List<string> CatchFishEvents = new List<string>();
        NotifyIconComponent NIC;
        DateTimeSelector DTS;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            Loaded += Initialize_ShowData;

            GenerateNIC();
            Hook.Start();
            CheckForeground.Start();
            SetStatistical(null, new TimeSpan(), new TimeSpan(), new TimeSpan(), false);
            CatchFishEvents = Data.GetFishs();
        }

        private void Initialize_ShowData(object sender, EventArgs e)
        {
            //var str = Consts.Icon2Base64String(new Icon("clock.ico"));
            UpdateByShowData();
        }

        private void GenerateNIC()
        {
            NIC = new NotifyIconComponent();
            NIC.NotifyIconText = "一个静悄悄的图标";
            NIC.ContextMenuStripItems.Add("显示/隐藏主界面");
            NIC.ContextMenuStripItems[0].Click += Visibility_Click;
            NIC.ContextMenuStripItems[0].Font = new Font(NIC.ContextMenuStripItems[0].Font, NIC.ContextMenuStripItems[0].Font.Style | System.Drawing.FontStyle.Bold);
            NIC.ContextMenuStripItems.Add("设置挂机标题");
            NIC.ContextMenuStripItems[1].Click += ShowEnterName_Click;
            NIC.ContextMenuStripItems.Add("退出");
            NIC.ContextMenuStripItems[2].Click += Exit_Click;
            NIC.NotifyIconDoubleClick = Visibility_Click;
            NIC.NotifyIconIcon = Consts.Base64String2Icon(Consts.IconClockStr);
        }

        private void Visibility_Click(object sender, EventArgs e)
        {
            WindowVisibilityChange();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            NIC.NotifyIconVisibility = false;
            Hook.Stop();
            Environment.Exit(0);
        }

        private void ShowEnterName_Click(object sender, EventArgs e)
        {
            EnterName EN = new EnterName();
            EN.Show();
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
            string SelectedTime = GetTime();
            if (SelectedTime != null)
            {
                var strs = SelectedTime.Split(' ');
                StartYMDTextBlock.Text = strs[0];
                StartHMSTextBlock.Text = strs[1];
            }
            UpdateByShowData();
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
            UpdateByShowData();
        }

        string GetTime()
        {
            DTS = new DateTimeSelector();
            DTS.ShowDialog();
            return DTS.TimeResult;
        }

        double GetShowData()
        {
            ShowDataList.Clear();
            SectorDataList.Clear();
            Consts.ResetNextBrush();

            var Actions = Data.GetActions(StartYMDTextBlock.Text + " " + StartHMSTextBlock.Text, EndYMDTextBlock.Text + " " + EndHMSTextBlock.Text);

            var lastmove = new DateTime(0);
            if (Actions.Count > 0) lastmove = Actions[0].DateTime;
            foreach (var a in Actions)
            {
                Data.ShowData last = null;
                if (ShowDataList.Count != 0) last = ShowDataList.Last();
                
                if (a.Type == 0)
                {
                    if (last == null || last.Action != a.Action || last.Title != a.Title)
                    {
                        if (a.DateTime - lastmove > Consts.ActiveTimeSpan)
                        {
                            var fishdata = new Data.ShowData()
                            {
                                I = Consts.Base64String2Icon(Consts.IconFishStr),
                                Start = lastmove,
                                End = a.DateTime,
                                Action = Consts.CatchFishString,
                                IsCatchFish = true,
                                Title = ""
                            };
                            ShowDataList.Add(fishdata);
                            last = ShowDataList.Last();
                        }
                        var SD = new Data.ShowData()
                        {
                            I = a.Icon,
                            Start = a.DateTime,
                            End = a.DateTime,
                            Action = a.Action,
                            Title = a.Title,
                        };
                        SD.IsCatchFish = CatchFishEvents.Any(str => { return str == SD.ActionTitle; });
                        if (last != null) last.End = SD.Start;
                        ShowDataList.Add(SD);
                        lastmove = a.DateTime;
                    }
                    else
                    {
                        last.End = a.DateTime;
                    }
                }
                else
                {
                    if ((a.DateTime - lastmove) > Consts.ActiveTimeSpan)
                    {
                        if (ShowDataList.Count == 0)
                            goto End;
                        var lastshowdata = new Data.ShowData(ShowDataList.Last());
                        var fishdata = new Data.ShowData()
                        {
                            I = Consts.Base64String2Icon(Consts.IconFishStr),
                            Start = lastmove,
                            End = a.DateTime,
                            Action = Consts.CatchFishString,
                            IsCatchFish = true,
                            Title = ""
                        };
                        ShowDataList.Last().End = lastmove;
                        ShowDataList.Add(fishdata);
                        lastshowdata.Start = lastshowdata.End = a.DateTime;
                        ShowDataList.Add(lastshowdata);
                    }
                    End:;
                    lastmove = a.DateTime;
                }
            }

            var tmplist = ShowDataList;
            ShowDataList = new List<Data.ShowData>();
            foreach (var i in tmplist)
            {
                if (ShowDataList.Count > 0 && i.Action == ShowDataList.Last().Action && i.Title == ShowDataList.Last().Title)
                    ShowDataList.Last().End = i.End;
                else if (i.Ticks > Consts.MinimumCountTimeSpan.Ticks)
                    ShowDataList.Add(i);
                else if (ShowDataList.Count > 0)
                    ShowDataList.Last().End = i.End;
            }

            if (ShowDataList.Count == 0) return 0;

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
            for (int i = 0; i < SectorDataList.Count; i++)
            {
                var data = SectorDataList[i];
                double l = SectorGrid.ActualWidth / 2, t = SectorGrid.ActualHeight / 2;
                double r = (l > t ? t : l) / 1.2;
                double ed = sectorst + (data.End - data.Start).Ticks / totalTime;
                SectorPath SP = new SectorPath(l, t, r, sectorst - 0.25, ed - 0.25, data.Color);
                sectorst = ed;

                SectorPathList.Add(SP);
                SectorPathList.Last().GetPath.MouseEnter += DataMouseEnter;
                SectorPathList.Last().GetPath.MouseLeave += DataMouseLeave;
                SectorPathList.Last().GetPath.MouseDown += DataMouseClick;
                SectorPathList.Last().GetPath.Tag = -1 - i;
                SectorGrid.Children.Add(SectorPathList.Last().GetPath);
            }

            for (int i = 0; i < ShowDataList.Count; i++)
            {
                var data = ShowDataList[i];
                ListStackPanelItem LSPI = new ListStackPanelItem(data);
                LSPI.MouseEnter += DataMouseEnter;
                LSPI.MouseLeave += DataMouseLeave;
                LSPI.MouseDown += DataMouseClick;
                LSPI.Tag = i;
                ListStackPanel.Children.Add(LSPI);
                
                double l = RectangleGrid.ActualWidth / 20;
                double t = RectangleGrid.ActualHeight / 20;
                double w = l * 18;
                double h = t * 18;
                double st = (data.Start - ShowDataList[0].Start).Ticks / totalTime;
                double ed = st + (data.End - data.Start).Ticks / totalTime;
                RectanglePath RP = new RectanglePath(l, t, w, h, st, ed, data.Color);

                RectanglePathList.Add(RP);
                RectanglePathList.Last().GetPath.MouseEnter += DataMouseEnter;
                RectanglePathList.Last().GetPath.MouseLeave += DataMouseLeave;
                RectanglePathList.Last().GetPath.MouseDown += DataMouseClick;
                RectanglePathList.Last().GetPath.Tag = i;
                RectangleGrid.Children.Add(RectanglePathList.Last().GetPath);
            }
            
        }

        bool ClickInData = false;
        int LastDataId = -1;

        void DataMouseEnter(object sender, MouseEventArgs e)
        {
            ClickInData = false;
            DataLeave();
            string a = "", t = "";
            bool liststackpanelin = false;
            int id;
            if (sender is ListStackPanelItem)
            {
                var LSP = sender as ListStackPanelItem;
                liststackpanelin = true;
                a = LSP.FullFileName;
                t = LSP.TitleTextBlock.Text;
                id = (int)LSP.Tag;
            }
            else
            {
                id = (int)(sender as FrameworkElement).Tag;
                if (id < 0)
                {
                    a = SectorDataList[-id - 1].Action;
                    t = SectorDataList[-id - 1].Title;
                }
                else
                {
                    a = ShowDataList[id].Action;
                    t = ShowDataList[id].Title;
                }
            }
            //Debug.Write(a + "|" + t + "|" + liststackpanelin);
            LastDataId = id;
            if (id < 0)
            {
                for (int i = 0; i < ShowDataList.Count; i++)
                    if (ShowDataList[i].Action == a && ShowDataList[i].Title == t)
                    {
                        LastDataId = i;
                        break;
                    }
            }
            CatchFishButton.Content = ShowDataList[LastDataId].IsCatchFish ? "设为工作任务" : "设为摸鱼任务";
            if (!liststackpanelin)
            {
                if (id >= 0)
                {
                    var nowitem = ListStackPanel.Children[id] as ListStackPanelItem;
                    nowitem.AddTransform();
                    var dis = nowitem.TranslatePoint(new System.Windows.Point(0, 0), ListStackPanel).Y;
                    ListScrollViewer.ScrollToVerticalOffset(dis);
                }
                else
                {
                    var moved = false;
                    foreach (ListStackPanelItem item in ListStackPanel.Children)
                    {
                        if (item.FullFileName == a && item.TitleTextBlock.Text == t)
                        {
                            item.AddTransform();
                            if (!moved)
                            {
                                moved = true;
                                var dis = item.TranslatePoint(new System.Windows.Point(0, 0), ListStackPanel).Y;
                                ListScrollViewer.ScrollToVerticalOffset(dis);
                            }
                        }
                    }
                }
            }
            else
            {
                var nowitem = ListStackPanel.Children[id] as ListStackPanelItem;
                nowitem.AddTransform();
            }
            if (liststackpanelin)
            {
                RectanglePathList[id].AddTransform();
            }
            else
            {
                for (var i = 0; i < ShowDataList.Count; i++)
                    if (ShowDataList[i].Action == a && ShowDataList[i].Title == t)
                        RectanglePathList[i].AddTransform();
            }
            for (int i = 0; i < SectorDataList.Count; i++)
                if (SectorDataList[i].Action == a && SectorDataList[i].Title == t)
                    SectorPathList[i].AddTransform();
            Data.ShowData SD = null;
            if (id >= 0) SD = ShowDataList[id];
            else for (var i = 0; i < ShowDataList.Count; i++)
                    if (ShowDataList[i].Action == a && ShowDataList[i].Title == t)
                    {
                        SD = ShowDataList[i];
                        break;
                    }
            TimeSpan totaltime = ShowDataList.Last().End - ShowDataList[0].Start;
            TimeSpan thistime = new TimeSpan();
            foreach (var i in SectorDataList)
                if (i.Action == a & i.Title == t)
                    thistime = new TimeSpan(i.Ticks);
            TimeSpan fishtime = new TimeSpan(0);
            foreach (var i in ShowDataList)
                if (i.IsCatchFish)
                    fishtime += new TimeSpan(i.Ticks);
            SetStatistical(SD, totaltime, thistime, fishtime, true);
        }

        void DataMouseLeave(object sender, MouseEventArgs e)
        {
            DataLeave();
        }

        void DataLeave()
        {
            if (ClickInData) return;
            foreach (ListStackPanelItem item in ListStackPanel.Children)
                item.RemoveTransform();
            foreach (var i in SectorPathList)
                i.RemoveTransform();
            foreach (var i in RectanglePathList)
                i.RemoveTransform();
            SetStatistical(null, new TimeSpan(), new TimeSpan(), new TimeSpan(), false);
        }

        void DataMouseClick(object sender, MouseEventArgs e)
        {
            ClickInData = true;
        }

        void SetStatistical(Data.ShowData SD, TimeSpan TotalTime, TimeSpan ThisTime, TimeSpan FishTime, bool DivideByTitle)
        {
            if (SD == null)
            {
                DivideByTitleTextBlock.Text = null;
                CatchFishTextBlock.Text = null;
                ThisTimeTextBlock.Text = null;
                ThisTotalTimeTextBlock.Text = null;
                TotalTimeTextBlock.Text = null;
                ThisPercentTextBlock.Text = null;
                ThisTotalPercentTextBlock.Text = null;
                ActionNameTextBlock.Text = null;
                TitleTextBlock.Text = null;
                CatchFishTimeTextBlock.Text = null;
                CatchFishPercentTextBlock.Text = null;
                return;
            }
            DivideByTitleTextBlock.Text = DivideByTitle ? "是" : "否";
            CatchFishTextBlock.Text = SD.IsCatchFish ? "是" : "否";
            ThisTimeTextBlock.Text = Consts.TimeSpan2String(new TimeSpan(SD.Ticks));
            ThisTotalTimeTextBlock.Text = Consts.TimeSpan2String(ThisTime);
            TotalTimeTextBlock.Text = Consts.TimeSpan2String(TotalTime);
            ThisPercentTextBlock.Text = (100.0 * SD.Ticks / TotalTime.Ticks).ToString("F3") + "%";
            ThisTotalPercentTextBlock.Text = (100.0 * ThisTime.Ticks / TotalTime.Ticks).ToString("F3") + "%";
            ActionNameTextBlock.Text = Consts.GetFileName(SD.Action);
            TitleTextBlock.Text = SD.Title;
            CatchFishTimeTextBlock.Text = Consts.TimeSpan2String(FishTime);
            CatchFishPercentTextBlock.Text = (100.0 * FishTime.Ticks / TotalTime.Ticks).ToString("F3") + "%";
        }

        private void CatchFishButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ClickInData)
                return;
            if (ShowDataList[LastDataId].IsCatchFish)
            {
                if (ShowDataList[LastDataId].Action != Consts.CatchFishString)
                {
                    Data.RemoveFish(ShowDataList[LastDataId].ActionTitle);
                    CatchFishEvents.Remove(ShowDataList[LastDataId].ActionTitle);
                }
                else
                {
                    MessageBox.Show("摸鱼时间永远是摸鱼时间！");
                }
            }
            else
            {
                Data.AddFish(ShowDataList[LastDataId].ActionTitle);
                CatchFishEvents.Add(ShowDataList[LastDataId].ActionTitle);
            }
            ClickInData = false;
            UpdateByShowData();
            DataLeave();
        }
    }
}

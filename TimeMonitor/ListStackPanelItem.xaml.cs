using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace TimeMonitor
{
    /// <summary>
    /// ListStackPanelItem.xaml 的交互逻辑
    /// </summary>
    public partial class ListStackPanelItem : UserControl
    {
        public ListStackPanelItem(Data.ShowData SD)
        {
            InitializeComponent();
            IconImage.Source = Consts.ChangeBitmapToImageSource(SD.I.ToBitmap());
            TimeTextBlock.Text = "时间：" + SD.Start.ToString(Consts.DateTimeFormatString) + " 至 " + SD.End.ToString(Consts.DateTimeFormatString);
            ActionTextBlock.Text = SD.Action;
            TitleTextBlock.Text = SD.Title;
        }
    }
}

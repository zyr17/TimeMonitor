using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TimeMonitor
{
    /// <summary>
    /// DateTimeSelector.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeSelector : Window
    {
        ObservableCollection<string> PreTimeUnitOC, PreTimeNumOC, YearOC, MonthOC, DayOC, HourOC, MinuteOC, SecondOC;
        DateTime DT;
        bool Confirm, DisableYMDTextChangedEvent, DisableHMSTextChangedEvent;

        public string TimeResult
        {
            get
            {
                if (Confirm) return YMDTextBox.Text + " " + HMSTextBox.Text;
                return null;
            }
        }

        public DateTimeSelector()
        {
            InitializeComponent();
            DisableHMSTextChangedEvent = DisableYMDTextChangedEvent = false;
            DT = DateTime.Now;
            PreTimeNumOC = new ObservableCollection<string>();
            for (int i = 1; i < 30; i++)
                PreTimeNumOC.Add(i.ToString());
            PreTimeUnitOC = new ObservableCollection<string> { "分钟", "小时", "日", "周", "月", "年" };
            PreTimeNumComboBox.ItemsSource = PreTimeNumOC;
            PreTimeUnitComboBox.ItemsSource = PreTimeUnitOC;
            PreTimeUnitComboBox.SelectedIndex = 2;
            //PreTimeNumComboBox.SelectedIndex = 0;
            UpdateYMDHMSComboBoxItems();
            UpdateYMDHMSTextBox();
            Confirm = false;
        }

        void UpdateYMDHMSComboBoxItems()
        {

            if (YearOC == null)
            {
                YearOC = new ObservableCollection<string>();
                YearComboBox.ItemsSource = YearOC;
            }
            YearComboBox.SelectedIndex = -1;
            YearOC.Clear();
            for (int i = DT.Year - 30; i <= DT.Year; i++)
                YearOC.Add(i.ToString());
            YearComboBox.SelectedIndex = YearOC.Count - 1;
            if (MonthOC == null)
            {
                MonthOC = new ObservableCollection<string>();
                MonthComboBox.ItemsSource = MonthOC;
            }
            MonthComboBox.SelectedIndex = -1;
            MonthOC.Clear();
            for (int i = 1; i <= 12; i++)
                MonthOC.Add(i.ToString().PadLeft(2, '0'));
            MonthComboBox.SelectedIndex = DT.Month - 1;
            if (DayOC == null)
            {
                DayOC = new ObservableCollection<string>();
                DayComboBox.ItemsSource = DayOC;
            }
            DayComboBox.SelectedIndex = -1;
            DayOC.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(DT.Year, DT.Month); i++)
                DayOC.Add(i.ToString().PadLeft(2, '0'));
            DayComboBox.SelectedIndex = DT.Day - 1;
            if (HourOC == null)
            {
                HourOC = new ObservableCollection<string>();
                HourComboBox.ItemsSource = HourOC;
            }
            HourComboBox.SelectedIndex = -1;
            HourOC.Clear();
            for (int i = 0; i < 24; i++)
                HourOC.Add(i.ToString().PadLeft(2, '0'));
            HourComboBox.SelectedIndex = DT.Hour;
            if (MinuteOC == null)
            {
                MinuteOC = new ObservableCollection<string>();
                MinuteComboBox.ItemsSource = MinuteOC;
            }
            MinuteComboBox.SelectedIndex = -1;
            MinuteOC.Clear();
            for (int i = 0; i < 60; i++)
                MinuteOC.Add(i.ToString().PadLeft(2, '0'));
            MinuteComboBox.SelectedIndex = DT.Minute;
            if (SecondOC == null)
            {
                SecondOC = new ObservableCollection<string>();
                SecondComboBox.ItemsSource = SecondOC;
            }
            SecondComboBox.SelectedIndex = -1;
            SecondOC.Clear();
            for (int i = 0; i < 60; i++)
                SecondOC.Add(i.ToString().PadLeft(2, '0'));
            SecondComboBox.SelectedIndex = DT.Second;
        }
        void UpdateYMDHMSTextBox()
        {
            DisableHMSTextChangedEvent = DisableYMDTextChangedEvent = true;
            YMDTextBox.Text = (string)YearComboBox.SelectedValue + '-' + (string)MonthComboBox.SelectedValue + '-' + (string)DayComboBox.SelectedValue;
            HMSTextBox.Text = (string)HourComboBox.SelectedValue + ':' + (string)MinuteComboBox.SelectedValue + ':' + (string)SecondComboBox.SelectedValue;
            DisableYMDTextChangedEvent = DisableHMSTextChangedEvent = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Confirm = true;
            Close();
        }

        private void SetHMSZeroButton_Click(object sender, RoutedEventArgs e)
        {
            HourComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
            SecondComboBox.SelectedIndex = 0;
            UpdateYMDHMSTextBox();
        }

        private void PreTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PreTimeNumComboBox.SelectedIndex < 0 || PreTimeUnitComboBox.SelectedIndex < 0)
                return;
            if (PreTimeUnitComboBox.SelectedIndex == 0) DT = DateTime.Now.AddMinutes(-PreTimeNumComboBox.SelectedIndex - 1);
            if (PreTimeUnitComboBox.SelectedIndex == 1) DT = DateTime.Now.AddHours(-PreTimeNumComboBox.SelectedIndex - 1);
            if (PreTimeUnitComboBox.SelectedIndex == 2) DT = DateTime.Now.AddDays(-PreTimeNumComboBox.SelectedIndex - 1);
            if (PreTimeUnitComboBox.SelectedIndex == 3) DT = DateTime.Now.AddDays((-PreTimeNumComboBox.SelectedIndex - 1) * 7);
            if (PreTimeUnitComboBox.SelectedIndex == 4) DT = DateTime.Now.AddMonths(-PreTimeNumComboBox.SelectedIndex - 1);
            if (PreTimeUnitComboBox.SelectedIndex == 5) DT = DateTime.Now.AddYears(-PreTimeNumComboBox.SelectedIndex - 1);
            UpdateYMDHMSComboBoxItems();
            UpdateYMDHMSTextBox();
        }

        private void SetNowButton_Click(object sender, RoutedEventArgs e)
        {
            DT = DateTime.Now;
            UpdateYMDHMSComboBoxItems();
            UpdateYMDHMSTextBox();
        }

        private bool HasInvalidCharactor(string text, string v)
        {
            foreach (var i in text)
            {
                bool result = false;
                foreach (var j in v)
                    if (i == j) result = true;
                if (!result) return true;
            }
            return false;
        }

        private void YMDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TB = (TextBox)sender;
            if (DisableYMDTextChangedEvent || TB.Text.Length == 0) return;
            DisableYMDTextChangedEvent = true;
            if (HasInvalidCharactor(TB.Text, "1234567890-"))
                goto End;
            var part = TB.Text.Split('-');
            DateTime DT = DateTime.Now;
            string ys = "", ms = "", ds = "";
            int y, m, d;
            if (part.Length == 1)
            {
                for (int i = 0; i < part[0].Length; i ++)
                {
                    if (i < 4) ys += part[0][i];
                    else if (i < 6) ms += part[0][i];
                    else ds += part[0][i];
                }
            }
            else
            {
                ys = part[0];
                if (part.Length == 2) ms = part[1];
                if (part.Length == 3)
                {
                    ms = part[1].PadLeft(2, '0');
                    ds = part[2];
                }
            }
            y = Convert.ToInt32(ys);
            m = ms.Length > 0 ? Convert.ToInt32(ms) : -1;
            d = ds.Length > 0 ? Convert.ToInt32(ds) : -1;
            if (y > DT.Year || y < DT.Year - 30)
                goto End;
            if ((m > 12 || m < 1) && m != -1)
                goto End;
            if ((d > DateTime.DaysInMonth(DT.Year, DT.Month) || d < 1) && d != -1)
                goto End;
            TB.Text = ys + '-' + ms;
            if (ms.Length == 2)
                TB.Text += '-' + ds;
            TB.SelectionStart = TB.Text.Length;
            End: DisableYMDTextChangedEvent = false;
        }

        private void HMSTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TB = (TextBox)sender;
            if (DisableHMSTextChangedEvent || TB.Text.Length == 0) return;
            DisableHMSTextChangedEvent = true;
            if (HasInvalidCharactor(TB.Text, "1234567890:"))
                goto End;
            var part = TB.Text.Split(':');
            DateTime DT = DateTime.Now;
            string hs = "", ms = "", ss = "";
            int h, m, s;
            if (part.Length == 1)
            {
                for (int i = 0; i < part[0].Length; i++)
                {
                    if (i < 2) hs += part[0][i];
                    else if (i < 4) ms += part[0][i];
                    else ss += part[0][i];
                }
            }
            else
            {
                hs = part[0];
                if (part.Length == 2) ms = part[1];
                if (part.Length == 3)
                {
                    ms = part[1].PadLeft(2, '0');
                    ss = part[2];
                }
            }
            h = Convert.ToInt32(hs);
            m = ms.Length > 0 ? Convert.ToInt32(ms) : -1;
            s = ss.Length > 0 ? Convert.ToInt32(ss) : -1;
            if (h > 23 || h < 0)
                goto End;
            if ((m > 59 || m < 0) && m != -1)
                goto End;
            if ((s > 59 || s < 0) && s != -1)
                goto End;
            TB.Text = hs;
            if (hs.Length == 2)
                TB.Text += ':' + ms;
            if (ms.Length == 2)
                TB.Text += ':' + ss;
            TB.SelectionStart = TB.Text.Length;
            End: DisableHMSTextChangedEvent = false;
        }

        private void YMDHMSTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var TB = (TextBox)sender;
            if (TB.Text.Length == 0) return;
            var last = TB.Text[TB.Text.Length - 1];
            if ((e.Key == Key.OemMinus || e.Key == Key.Subtract) && last == '-')
                e.Handled = true;
            if (e.Key == Key.Oem1 && last == ':')
                e.Handled = true;
            if (e.Key == Key.Back && (last == '-' || last == ':'))
            {
                DisableHMSTextChangedEvent = DisableYMDTextChangedEvent = true;
                TB.Text = TB.Text.Substring(0, TB.Text.Length - 1);
                TB.SelectionStart = TB.Text.Length;
                DisableHMSTextChangedEvent = DisableYMDTextChangedEvent = false;
            }
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DateTime DTT = DateTime.Now;
                if (HasInvalidCharactor(YMDTextBox.Text, "1234567890-"))
                    return;
                var part = YMDTextBox.Text.Split('-');
                if (part.Length != 3)
                    return;
                var year = Convert.ToInt32(part[0]);
                if (year > DTT.Year || year < DTT.Year - 30)
                    return;
                var month = Convert.ToInt32(part[1]);
                if (month > 12 || month < 1)
                    return;
                var day = Convert.ToInt32(part[2]);
                if (day > DateTime.DaysInMonth(year, month) || day < 1)
                    return;
                if (HasInvalidCharactor(HMSTextBox.Text, "1234567890:"))
                    return;
                part = HMSTextBox.Text.Split(':');
                if (part.Length != 3)
                    return;
                var hour = Convert.ToInt32(part[0]);
                if (hour > 23 || hour < 0)
                    return;
                var minute = Convert.ToInt32(part[1]);
                if (minute > 59 || minute < 0)
                    return;
                var second = Convert.ToInt32(part[2]);
                if (second > 59 || second < 0)
                    return;
                DT = new DateTime(year, month, day, hour, minute, second);
                UpdateYMDHMSComboBoxItems();
                UpdateYMDHMSTextBox();
            }
        }

    }
}

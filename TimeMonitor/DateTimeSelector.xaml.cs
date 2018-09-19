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
        bool Confirm;

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
            MinuteComboBox.SelectedIndex = DT.Minute - 1;
            if (SecondOC == null)
            {
                SecondOC = new ObservableCollection<string>();
                SecondComboBox.ItemsSource = SecondOC;
            }
            SecondComboBox.SelectedIndex = -1;
            SecondOC.Clear();
            for (int i = 0; i < 60; i++)
                SecondOC.Add(i.ToString().PadLeft(2, '0'));
            SecondComboBox.SelectedIndex = DT.Second - 1;
        }
        void UpdateYMDHMSTextBox()
        {
            YMDTextBox.Text = (string)YearComboBox.SelectedValue + '-' + (string)MonthComboBox.SelectedValue + '-' + (string)DayComboBox.SelectedValue;
            HMSTextBox.Text = (string)HourComboBox.SelectedValue + ':' + (string)MinuteComboBox.SelectedValue + ':' + (string)SecondComboBox.SelectedValue;
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

    }
}

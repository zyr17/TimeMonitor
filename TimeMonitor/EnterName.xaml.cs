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
using System.Windows.Shapes;

namespace TimeMonitor
{
    /// <summary>
    /// EnterName.xaml 的交互逻辑
    /// </summary>
    public partial class EnterName : Window
    {
        public EnterName()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.Text.Length == 0)
            {
                MessageBox.Show("没有填写标题");
                return;
            }
            Consts.KA.KeepAliveStart(MainTextBox.Text);
            Close();
        }
    }
}

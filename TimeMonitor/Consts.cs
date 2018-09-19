using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeMonitor
{
    class Consts
    {
        static SolidColorBrush[] Colors = {
            Brushes.MediumSlateBlue,
            Brushes.LightSkyBlue,
            Brushes.LimeGreen,
            Brushes.Chocolate,
            Brushes.Orchid,
            Brushes.PaleGreen,
            Brushes.DodgerBlue,
            Brushes.Silver,
            Brushes.Red,
            Brushes.Plum,
            Brushes.DodgerBlue,
            Brushes.Orange,
            Brushes.DarkGray,
        };
        static int nowcolor = 0;
        public static SolidColorBrush NextBrush
        {
            get
            {
                return Colors[nowcolor++ % Colors.Length];
            }
        }
    }
}

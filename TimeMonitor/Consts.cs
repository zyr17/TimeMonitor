using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TimeMonitor
{
    class Consts
    {
        public static TimeSpan ActiveTimeSpan = new TimeSpan(0, 5, 0);
        public static TimeSpan HookTimeSpan = new TimeSpan(0, 0, 5);
        public static TimeSpan CheckForegroundTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
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

        public static ImageSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return wpfBitmap;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeMonitor
{
    class SectorPath
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
        double ox, oy, r, radstart, radend;
        public Path GetPath { get; private set; }
        public SectorPath(double Left, double Top, double R, double Start, double End)
        {
            ox = Left;
            oy = Top;
            r = R;
            radstart = Start * Math.PI * 2;
            radend = End * Math.PI * 2;
            GeneratePath();
        }
        public void GeneratePath()
        {
            GetPath = new Path();
            GetPath.Fill = Colors[nowcolor++];
            if (nowcolor >= Colors.Length) nowcolor = 0;
            GetPath.HorizontalAlignment = HorizontalAlignment.Left;
            GetPath.VerticalAlignment = VerticalAlignment.Top;
            PathGeometry myGeometry = new PathGeometry();
            PathFigure PF = new PathFigure();
            PF.StartPoint = new Point(ox, oy);
            PF.Segments.Add(new LineSegment(new Point(ox + Math.Cos(radstart) * r, oy + Math.Sin(radstart) * r), false));
            PF.Segments.Add(new ArcSegment(new Point(ox + Math.Cos(radend) * r, oy + Math.Sin(radend) * r), new Size(r, r), 0, (radend - radstart > Math.PI ? true : false), SweepDirection.Clockwise, false));
            PF.Segments.Add(new LineSegment(new Point(ox, oy), false));
            myGeometry.Figures.Add(PF);
            GetPath.Data = myGeometry;
            GetPath.MouseEnter += MyPath_MouseEnter;
            GetPath.MouseLeave += MyPath_MouseLeave;
        }

        private void MyPath_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GetPath.RenderTransform = null;
            GetPath.Margin = new Thickness(0, 0, 0, 0);
        }

        private void MyPath_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GetPath.RenderTransform = new ScaleTransform(1.1, 1.1);
            GetPath.Margin = new Thickness(-ox / 10, -oy / 10, 0, 0);
        }
    }
}

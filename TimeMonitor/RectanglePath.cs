using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeMonitor
{
    class RectanglePath
    {
        Point p1, p2;
        public Path GetPath { get; private set; }

        public RectanglePath(double Left, double Top, double Width, double Height, double Start, double End, SolidColorBrush Brush = null)
        {
            Point P1 = new Point(Left + Width * Start, Top);
            Point P2 = new Point(Left + Width * End, Top + Height);
            Initialize(P1, P2, Brush);
        }

        public RectanglePath(Point P1, Point P2, SolidColorBrush Brush = null)
        {
            Initialize(P1, P2, Brush);
        }

        void Initialize(Point P1, Point P2, SolidColorBrush Brush)
        {
            if (P1.X > P2.X)
            {
                var tmp = P1.X;
                P1.X = P2.X;
                P2.X = tmp;
            }
            if (P1.Y > P2.Y)
            {
                var tmp = P1.Y;
                P1.Y = P2.Y;
                P2.Y = tmp;
            }
            p1 = P1;
            p2 = P2;
            GeneratePath(Brush);
        }

        void GeneratePath(SolidColorBrush Brush)
        {
            if (Brush == null) Brush = Consts.NextBrush;
            GetPath = new Path();
            GetPath.Fill = Brush;
            GetPath.HorizontalAlignment = HorizontalAlignment.Left;
            GetPath.VerticalAlignment = VerticalAlignment.Top;
            PathGeometry myGeometry = new PathGeometry();
            PathFigure PF = new PathFigure();
            PF.StartPoint = p1;
            PF.Segments.Add(new LineSegment(new Point(p1.X, p2.Y), false));
            PF.Segments.Add(new LineSegment(p2, false));
            PF.Segments.Add(new LineSegment(new Point(p2.X, p1.Y), false));
            myGeometry.Figures.Add(PF);
            GetPath.Data = myGeometry;
        }

        private void MyPath_MouseLeave(object sender, MouseEventArgs e)
        {
            Path path = (Path)sender;
            RemoveTransform();
        }

        private void MyPath_MouseEnter(object sender, MouseEventArgs e)
        {
            AddTransform();
        }

        public void AddTransform()
        {
            GetPath.RenderTransform = new ScaleTransform(1, 1.1);
            GetPath.Margin = new Thickness(0, -(p2.Y - p1.Y) / 20, 0, 0);
        }

        public void RemoveTransform()
        {
            GetPath.RenderTransform = null;
            GetPath.Margin = new Thickness(0);
        }
    }
}

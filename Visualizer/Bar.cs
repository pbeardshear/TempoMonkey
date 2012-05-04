using System;

using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace Visualizer
{

    public class Bar
    {
        private static double i = .10;
        private static double vanishingPointz = 500;
        public static double CANVAS_CENTERX = 1305 / 2;
        public static Canvas myCanvas;
        private double x, d , k;

        private Rectangle bar;
        private Polygon side;

        //Position relative to the center;
        public Bar(double xPosition, Brush mainFill, Brush sideFill, double Width = 50, double Base = 30)
        {
            x = xPosition;
            bar = new Rectangle();
            bar.Width = Width;
			bar.Fill = mainFill;

            side = new Polygon();
			side.Fill = sideFill;

            d = Math.Sqrt(vanishingPointz * vanishingPointz + x * x);
            k = d * i * Math.Cos(Math.Atan(vanishingPointz / x));
            double h = -50;


            Point p1, p2, p3, p4;
            if (x > 0)
            {
                p1 = new Point(CANVAS_CENTERX + x - Width / 2, 0);
				p2 = new Point(CANVAS_CENTERX + x - Width / 2 - k, 0);
				p3 = new Point(CANVAS_CENTERX + x - Width / 2 - k, h - h * i);
				p4 = new Point(CANVAS_CENTERX + x - Width / 2, h);
            }
            else
            {
				p1 = new Point(CANVAS_CENTERX + x + Width / 2, 0);
				p2 = new Point(CANVAS_CENTERX + x + Width / 2 + k, 0);
				p3 = new Point(CANVAS_CENTERX + x + Width / 2 + k, h - h * i);
				p4 = new Point(CANVAS_CENTERX + x + Width / 2, h);
            }

            PointCollection myCollection = new PointCollection();
            myCollection.Add(p1);
            myCollection.Add(p2);
            myCollection.Add(p3);
            myCollection.Add(p4);
            side.Points = myCollection;

            Height = 50;
            myCanvas.Children.Add(side);
            myCanvas.Children.Add(bar);

            Canvas.SetLeft(bar, CANVAS_CENTERX - bar.Width / 2 + xPosition);
            Canvas.SetBottom(bar, Base);
            Canvas.SetBottom(side, Base);
        }

		// Destructor
		public void Destroy()
		{
			myCanvas.Children.Remove(bar);
			myCanvas.Children.Remove(side);
		}

        public static Canvas canvas
        {
            set
            {
                myCanvas = value;
            }
        }

        public double Height
        {
            set
            {
                bar.Height = value;
                double h = -value;
                if (x > 0)
                {
					side.Points[2] = new Point(CANVAS_CENTERX + x - bar.Width / 2 - k, h - h * i);
					side.Points[3] = new Point(CANVAS_CENTERX + x - bar.Width / 2, h);
                }
                else if (x < 0)
                {
					side.Points[2] = new Point(CANVAS_CENTERX + x + bar.Width / 2 + k, h - h * i);
					side.Points[3] = new Point(CANVAS_CENTERX + x + bar.Width / 2, h);
                }
            }
            get
            {
                return bar.Height;
            }
        }

        public System.Windows.Media.Brush BarFill
        {
            set
            {
                bar.Fill = value;
            }
        }

        public System.Windows.Media.Brush sideFill
        {
            set
            {
                side.Fill = value;
            }
        }

		public double Opacity
		{
			get
			{
				return bar.Opacity;
			}
			set
			{
				bar.Opacity = value;
				side.Opacity = value;
			}
		}

    }
}

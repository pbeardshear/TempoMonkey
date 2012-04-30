using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace slidingMenu
{
    /// <summary>
    /// Interaction logic for box.xaml
    /// </summary>
    public partial class mySlider : UserControl
    {

        public mySlider(string name, double start, double stop, double value, double width)
        {
            this.Width = width;
            InitializeComponent();
            SliderName.Content = name;
            _start = start;
            _stop = stop;
            Value = value;
        }

        double _value, _start, _stop;
        BrushConverter bc = new BrushConverter();

        public double Value
        {
            set
            {
                _value = Math.Min(_stop, Math.Max(_start, value));
                double percent = ((_value-_start) / (_stop - _start));
                Bar.Width = percent * this.Width;
            }
            get
            {
                return _value;
            }
        }

        public bool player1Exists
        {
            set
            {
                if (value)
                {
                    Border.Stroke = bc.ConvertFromString("Green") as System.Windows.Media.Brush;
                    Border.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Border.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        public bool player2Exists
        {
            set
            {
                if (value)
                {
                    Border.Stroke = bc.ConvertFromString("Blue") as System.Windows.Media.Brush;
                    Border.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Border.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
    }
}

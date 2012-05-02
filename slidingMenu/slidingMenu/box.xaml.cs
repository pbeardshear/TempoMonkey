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
    public partial class box : UserControl
    {

        public box(int sizeOfBox)
        {
            InitializeComponent();
            size = sizeOfBox;
            this.Width = this.Height = sizeOfBox;
            Highlight.Visibility = Visibility.Collapsed;
        }

        public int size;

        public string boxName
        {
            set
            {
                textBox.Content = value;
            }
        }

        public string address;
        public string name;
        public double position;
        public int index;
        public void setImage(string path)
        {
            try
            {
                this.Image.Source = new BitmapImage(new Uri(path));
            }
            catch
            {
                //Nothing
            }
        }

        public void highlightBox()
        {
            Highlight.Visibility = Visibility.Visible;
        }

        public void unHighlightBox()
        {
            Highlight.Visibility = Visibility.Collapsed;
        }

		public void setBackground(Brush backgroundColor)
		{
			boxCanvas.Background = backgroundColor;
		}

    }
}

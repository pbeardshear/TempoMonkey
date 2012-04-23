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
        Page myPage;

        public box(int sizeOfBox, Page page)
        {
            InitializeComponent();
            myPage = page;          
            size = sizeOfBox;
            //this.boxCanvas.Width = size;
            //this.boxCanvas.Height = size;
            //this.Highlight.Width = size;
            //this.Highlight.Height = size;
            //this.textBox.Width = size - 10;
            //this.textBox.Height = size - 10;
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
        public void setImage(string name)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\" + name + ".jpg";
                this.Image.Source = new BitmapImage(new Uri(path));
                //this.Image.Height = 3*size/4;
                //this.Image.Width = 3*size/4;
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

    }
}

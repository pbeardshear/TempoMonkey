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
using Microsoft.Kinect;
//using System.Windows.Forms;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page, CursorPage
    {
        public HomePage()
        {
            InitializeComponent();
            MainWindow.changeFonts(mainCanvas);
        }

        public void setCursor(SkeletonPoint point)
        {
            FrameworkElement element = myCursor;
            Canvas.SetLeft(element, point.X);// - element.Width / 2);
            Canvas.SetTop(element, point.Y);// - element.Height / 2);
        }

        public Ellipse getCursor()
        {
            return myCursor;
        }

        #region Button Handlers
        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        private void learningStudioButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new LearningStudio();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        private void browseMusic_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new BrowseMusic("Free");
            NavigationService.Navigate(MainWindow.currentPage);
        #endregion
        }
    }
}

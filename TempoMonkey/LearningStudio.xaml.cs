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


namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for LearningStudio.xaml
    /// </summary>
    public partial class LearningStudio : Page, CursorPage
    {

        public LearningStudio()
        {
            //InitializeComponent();
            //changeFonts();
        }

        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        private void Tutor_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new TutorMode();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        private void InteractiveModeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new BrowseMusic("Interactive");
            NavigationService.Navigate(MainWindow.currentPage);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new HomePage();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        public void setCursor(Microsoft.Kinect.SkeletonPoint point)
        {
            FrameworkElement element = myCursor;
            Canvas.SetLeft(element, point.X);// - element.Width / 2);
            Canvas.SetTop(element, point.Y);// - element.Height / 2);
        }

        public Ellipse getCursor()
        {
            return myCursor;
        }
    }
}

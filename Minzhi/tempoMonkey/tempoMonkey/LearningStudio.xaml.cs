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


namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for LearningStudio.xaml
    /// </summary>
    public partial class LearningStudio : Page
    {

        Boolean isReady;
        int direction;

        public LearningStudio()
        {
            InitializeComponent();
            isReady = false;
            direction = 999;
        }

        //Tell the MainWindow which menu button has been selected
        public int getSelectedMenu()
        {
            return direction;
        }


        private void setSelectionStatus(Boolean value)
        {
            isReady = value;
            if (!isReady)
            {
                RaiseEvent(new RoutedEventArgs(MainWindow.resetTimer));
            }
        }

        //Tell the MainWindow if the cursor is on the button.
        public Boolean isSelectionReady()
        {
            return isReady;
        }


        private void TutorButton_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 1;
        }

        private void TutorButton_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void InteractiveModeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 2;
        }

        private void InteractiveModeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void Back_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 3;
        }

        private void Back_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void Tutor_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("TutorMode.xaml", UriKind.Relative));
        }

        private void InteractiveModeButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("InteractiveMode.xaml", UriKind.Relative));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("HomePage.xaml", UriKind.Relative));
        }


    }
}

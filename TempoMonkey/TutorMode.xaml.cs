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
using Coding4Fun.Kinect.Wpf;

namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class TutorMode : Page
    {
        Boolean isClose = false;
        const int sklCount = 6;
        Skeleton[] allSkeletons = new Skeleton[sklCount];
        string mediaAddress;
        Boolean isReady;
        int direction;

        public TutorMode(string addr)
        {
            InitializeComponent();
            tipsRightHandPush.Visibility = Visibility.Collapsed;
            doneLabel.Visibility = Visibility.Collapsed;
            //Play.IsEnabled = false;
            Stop.IsEnabled = false;
            mediaAddress = addr;
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

        private void Play_MouseEnter(object sender, MouseEventArgs e)
        {
            Stop.IsEnabled = true;
            tutorScreen.Source = new Uri(mediaAddress);
            Play.IsEnabled = true;
            if (Play.Content.ToString() == "Play")
            {
                tutorScreen.Play();
                Play.Content = "Pause";
            }
            else
            {
                tutorScreen.Pause();
                Play.Content = "Play";
            }
        }

        private void Stop_MouseEnter(object sender, MouseEventArgs e)
        {
            tutorScreen.Stop();
            Play.Content = "Play";
            Play.IsEnabled = true;
            Stop.IsEnabled = false;
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


         
    }
}

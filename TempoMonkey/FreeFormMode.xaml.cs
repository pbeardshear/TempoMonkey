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
using System.Collections;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;


namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for FreeFormMode.xaml
    /// </summary>
    public partial class FreeFormMode : Page
    {
        /*
        ArrayList musicAddrList;
        ArrayList musicNameList;
        string mediaAddress;
        int direction;
        Boolean isReady;
        */
        bool closing = false;
        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer player1 = new KinectGesturePlayer();

        public void freeAllFramesReady(object sender, AllFramesReadyEventArgs e){
            if (!closing)
            {
                Skeleton[] skeletons = KinectGesturePlayer.getFirstTwoSkeletons(e);

                Skeleton leftSkeleton = skeletons.Count() > 0 ? skeletons[0] : null;
                Skeleton rightSkeleton = skeletons.Count() > 1 ? skeletons[1] : null;

                if (leftSkeleton != null)
                {
                    player1.skeletonReady(e, leftSkeleton);
                }
            }
        }

        //Handlers
        void pauseTrackingHandler(bool exist)
        {
            pause.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void debugTrackerHandler(double value)
        {
            DebugBox.Content = value.ToString();
        }

        void volumeChangeHandler(double change)
        {
            Canvas.SetTop(VolumePos, Canvas.GetTop(VolumePos) + change);
        }

        void volumeTrackingHandler(bool exist)
        {
            VolumePos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void tempoChangeHandler(double change)
        {
            Canvas.SetTop(TempoPos, Canvas.GetTop(TempoPos) + change);
        }

        void tempoTrackingHandler(bool exist)
        {
            TempoPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void seekChangeHandler(double change)
        {
            Canvas.SetLeft(SeekPos, Canvas.GetLeft(SeekPos) + change);
        }

        void seekTrackingHandler(bool exist)
        {
            SeekPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void pitchChangeHandler(double change)
        {
            Canvas.SetTop(PitchPos, Canvas.GetTop(PitchPos) + change);
        }

        void pitchTrackingHandler(bool exist)
        {
            PitchPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }


        public FreeFormMode(ArrayList addrList, ArrayList nameList)
        {
            InitializeComponent();
            /*
             * musicAddrList = addrList;
            musicNameList = nameList;
            addingToMusicList(musicNameList);
            mediaAddress = (string)musicAddrList[0];
            direction = 999;
            isReady = false; */

            //kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
            player1.registerCallBack(player1.kinectGuideListener, pauseTrackingHandler, debugTrackerHandler);
            player1.registerCallBack(player1.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            player1.registerCallBack(player1.handSwingListener, seekTrackingHandler, seekChangeHandler);
            player1.registerCallBack(player1.fistsPumpListener, tempoTrackingHandler, tempoChangeHandler);
            player1.registerCallBack(player1.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
        }

        /*
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

        
        private void addingToMusicList(ArrayList list)
        {
            foreach (string name in list)
            {
                Label myLabel = new Label();
                myLabel.Content = name;
                musicList.Children.Add(myLabel);
            }
        }


        private void Stop_MouseEnter(object sender, MouseEventArgs e)
        {
            musicPlayer.Stop();
            Play.Content = "Play";
            Play.IsEnabled = true;
            Stop.IsEnabled = false;

        }

        private void Play_MouseEnter(object sender, MouseEventArgs e)
        {
            Stop.IsEnabled = true;

            musicPlayer.Source = new Uri(mediaAddress);
            Play.IsEnabled = true;
            if (Play.Content.ToString() == "Play")
            {
                musicPlayer.Play();
                Play.Content = "Pause";
            }
            else
            {
                musicPlayer.Pause();
                Play.Content = "Play";
            }
        }

        private void back_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 3;
        }

        private void back_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }*/


    }
}

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
    /// Interaction logic for InteractiveMode.xaml
    /// </summary>
    public partial class InteractiveMode : Page
    {

        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer leftPlayer, rightPlayer;
        bool isPaused = false;

        public void interAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!isPaused)
            {
                Skeleton[] skeletons = KinectGesturePlayer.getFirstTwoSkeletons(e);
                Skeleton leftSkeleton;
                Skeleton rightSkeleton;

                if (skeletons.Length == 0)
                {
                    return;
                }
                else if (skeletons.Length == 1)
                {
                    leftSkeleton = skeletons[0];
                    leftPlayer.skeletonReady(e, leftSkeleton);
                }
                else
                {
                    leftSkeleton = skeletons[0];
                    rightSkeleton = skeletons[1];
                    leftPlayer.skeletonReady(e, leftSkeleton);
                    rightPlayer.skeletonReady(e, rightSkeleton);
                }
            }
        }

        //Handlers
        void pauseTrackingHandler(bool exist)
        {
            pause.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
            if (isPaused)
            {
                return;
            }

            if (exist)
            {
                Pause();
            }
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



        void volumeChangeHandler2(double change)
        {
            Canvas.SetTop(VolumePos2, Canvas.GetTop(VolumePos2) + change);
        }

        void volumeTrackingHandler2(bool exist)
        {
            VolumePos2.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void tempoChangeHandler2(double change)
        {
            Canvas.SetTop(TempoPos2, Canvas.GetTop(TempoPos2) + change);
        }

        void tempoTrackingHandler2(bool exist)
        {
            TempoPos2.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void seekChangeHandler2(double change)
        {
            Canvas.SetLeft(SeekPos2, Canvas.GetLeft(SeekPos2) + change);
        }

        void seekTrackingHandler2(bool exist)
        {
            SeekPos2.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        void pitchChangeHandler2(double change)
        {
            Canvas.SetTop(PitchPos2, Canvas.GetTop(PitchPos2) + change);
        }

        void pitchTrackingHandler2(bool exist)
        {
            PitchPos2.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
        }

        public void unPause()
        {
            isPaused = false;
            Border.Visibility = System.Windows.Visibility.Hidden;
            Resume.Visibility = System.Windows.Visibility.Hidden;
            Quit.Visibility = System.Windows.Visibility.Hidden;
        }

        public void Pause()
        {
            isPaused = true;
            Border.Visibility = System.Windows.Visibility.Visible;
            Resume.Visibility = System.Windows.Visibility.Visible;
            Quit.Visibility = System.Windows.Visibility.Visible;
        }

        private void ResumeEnter(object sender, MouseEventArgs e){
            setSelectionStatus(true);
            direction = 6;
        }

        private void ResumeLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void QuitEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 7;
        }

        private void QuitLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
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

        int direction = 999;
        bool isReady = false;


        public InteractiveMode()//ArrayList addrList, ArrayList nameList)
        {
            InitializeComponent();
            leftPlayer = new KinectGesturePlayer();
            rightPlayer = new KinectGesturePlayer();

            leftPlayer.registerCallBack(leftPlayer.kinectGuideListener, pauseTrackingHandler, debugTrackerHandler);
            leftPlayer.registerCallBack(leftPlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.fistsPumpListener, tempoTrackingHandler, tempoChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);

            rightPlayer.registerCallBack(rightPlayer.kinectGuideListener, pauseTrackingHandler, debugTrackerHandler);
            rightPlayer.registerCallBack(rightPlayer.handsAboveHeadListener, pitchTrackingHandler2, pitchChangeHandler2);
            rightPlayer.registerCallBack(rightPlayer.handSwingListener, seekTrackingHandler2, seekChangeHandler2);
            rightPlayer.registerCallBack(rightPlayer.fistsPumpListener, tempoTrackingHandler2, tempoChangeHandler2);
            rightPlayer.registerCallBack(rightPlayer.handsWidenListener, volumeTrackingHandler2, volumeChangeHandler2);
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

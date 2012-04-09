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

namespace TempoMonkey
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
            if (isPaused)
            {
                return;
            }

            if (exist)
            {
                Pause();
            }
        }


        /// <summary>
        ///          Kinect
        ///      
        ///           You
        /// |   1  |   0  |  2   |
        /// </summary>
        int previousTrack = 1;
        void changeTrackHandler(double value)
        {
            if (!wasSeeking)
            {
                SeekSlider.Value += .05; // THIS IS NOT REALLY TRUE
            }

            if (value < 250 && previousTrack != 1)
            {
                Track.Content = "On Track 1";
                previousTrack = 1;
            }
            else if (value > 450 && previousTrack != 2)
            {
                Track.Content = "On Track 2";
                previousTrack = 2;
            }
            else if (value >= 250 && value <= 450 && previousTrack != 0)
            {
                Track.Content = "On Track 0";
                previousTrack = 0;
            }
            else
            {
                return;
            }

            Processing.Audio.SwapTrack(previousTrack);
            Processing.Audio.Seek(SeekSlider.Value);
            Processing.Audio.ChangeVolume(VolumeSlider.Value);
            Processing.Audio.ChangeTempo(TempoSlider.Value);
            Processing.Audio.ChangePitch(PitchSlider.Value);
        }

        void volumeChangeHandler(double change)
        {
            VolumeSlider.Value -= change;
            Processing.Audio.ChangeVolume(VolumeSlider.Value);
        }

        void volumeTrackingHandler(bool exist)
        {
            Volume.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
        }

        void tempoChangeHandler(double change)
        {
            TempoSlider.Value += change / 2;
            Processing.Audio.ChangeTempo(TempoSlider.Value);
        }


        void tempoTrackingHandler(bool exist)
        {
            Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;

        }

        bool wasSeeking = false;
        void seekChangeHandler(double change)
        {
            SeekSlider.Value += change;
        }

        void seekTrackingHandler(bool exist)
        {
            Seek.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            if (exist)
            {
                wasSeeking = true;
            }
            else
            {
                if (wasSeeking)
                {
                    Processing.Audio.Seek(SeekSlider.Value);
                }
                wasSeeking = false;
            }
        }

        void pitchChangeHandler(double change)
        {
            PitchSlider.Value -= change * 3;
            Processing.Audio.ChangePitch(PitchSlider.Value);
        }

        void pitchTrackingHandler(bool exist)
        {
            Pitch.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
        }

        public void unPause()
        {
            isPaused = false;
            Border.Visibility = System.Windows.Visibility.Hidden;
            Resume.Visibility = System.Windows.Visibility.Hidden;
            Quit.Visibility = System.Windows.Visibility.Hidden;
            System.Windows.Forms.Cursor.Hide();
        }

        public void Pause()
        {
            isPaused = true;
            Border.Visibility = System.Windows.Visibility.Visible;
            Resume.Visibility = System.Windows.Visibility.Visible;
            Quit.Visibility = System.Windows.Visibility.Visible;
            System.Windows.Forms.Cursor.Show();
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


        public InteractiveMode(ArrayList addrList, ArrayList nameList)
        {
            System.Windows.Forms.Cursor.Hide();
            InitializeComponent();
            leftPlayer = new KinectGesturePlayer();
            rightPlayer = new KinectGesturePlayer();

            leftPlayer.registerCallBack(leftPlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            leftPlayer.registerCallBack(leftPlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.fistsPumpListener, tempoTrackingHandler, tempoChangeHandler);
            leftPlayer.registerCallBack(leftPlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);

            rightPlayer.registerCallBack(rightPlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            rightPlayer.registerCallBack(rightPlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            rightPlayer.registerCallBack(rightPlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            rightPlayer.registerCallBack(rightPlayer.handsUppenListener, tempoTrackingHandler, tempoChangeHandler);
            rightPlayer.registerCallBack(rightPlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
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

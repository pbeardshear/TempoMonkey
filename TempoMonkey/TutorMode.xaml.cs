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

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class TutorMode : Page
    {
        bool isPaused = false;
        Boolean isClose = false;
        const int sklCount = 6;
        Skeleton[] allSkeletons = new Skeleton[sklCount];
        //string mediaAddress;
        int direction = 999;
        bool isReady = false;
        BrushConverter bc = new BrushConverter();

        KinectGesturePlayer tutorPlayer;

        // Tutorial Mode will only have one lesson that we will show, which will be gestures
        // This is just a BIG hack, late extract different lessons into their instance of a TutorLesson Class.
        public TutorMode(string addr)
        {
            InitializeComponent();
            tipsRightHandPush.Visibility = Visibility.Collapsed;
            doneLabel.Visibility = Visibility.Collapsed;
            //Stop.IsEnabled = false;
            //mediaAddress = addr;

            tutorPlayer = new KinectGesturePlayer();
            tutorPlayer.registerCallBack(tutorPlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.fistsPumpListener, tempoTrackingHandler, tempoChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.kinectGuideListener, pauseTrackingHandler, debugTrackerHandler);
            
            Instructions.Content = TaskInstructions[currentTaskIndex];
        }

        public void tutorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
           Skeleton skeleton = KinectGesturePlayer.getFristSkeleton(e);
           if (skeleton != null)
           {
               if(!isPaused){
                   tutorPlayer.skeletonReady(e, skeleton);
               }
           }
        }

        int currentTaskIndex = 0;
        String[] Task = {"Seek", "Volume", "Pitch", "Tempo", "Switch Tracks" };
        String[] TaskInstructions = {"To Seek through a track just place your right hand up and move it left and right",
                                    "To change volume put both of your hands into your mid section and expand/impact them",
                                    "To change the pitch put both hands over your head and move your head up or down",
                                    "To increase the tempo pump your right hand, to decrease the tempo pump your left hand",
                                    "To change tracks jump left and right" };

        Boolean[] taskCompleted = { false, false, false, false, false, };
        
        void proceedIfGood(bool exist, String task)
        {
            if (exist && Task[currentTaskIndex] == task)
            {
                taskCompleted[currentTaskIndex] = true;
            }
            else if (!exist && taskCompleted[currentTaskIndex] && Task[currentTaskIndex] == task)
            {
                proceed();
            }
        }

        void proceed()
        {
            if (currentTaskIndex == Task.Length - 1)
            {
                Instructions.Content = "You have completed everything.. good job";
            }
            else
            {
                currentTaskIndex++;
                Instructions.Content = TaskInstructions[currentTaskIndex];
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
            proceedIfGood(exist, "Volume");
        }

        void tempoChangeHandler(double change)
        {
            Canvas.SetTop(TempoPos, Canvas.GetTop(TempoPos) + change);
        }

        void tempoTrackingHandler(bool exist)
        {
            TempoPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
            proceedIfGood(exist, "Tempo");
        }

        void seekChangeHandler(double change)
        {
            Canvas.SetLeft(SeekPos, Canvas.GetLeft(SeekPos) + change);
        }

        void seekTrackingHandler(bool exist)
        {
            SeekPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
            proceedIfGood(exist, "Seek");
        }

        void pitchChangeHandler(double change)
        {
            Canvas.SetTop(PitchPos, Canvas.GetTop(PitchPos) + change);
        }

        void pitchTrackingHandler(bool exist)
        {
            PitchPos.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
            proceedIfGood(exist, "Pitch");
        }


        //Tell the MainWindow which menu button has been selected

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

        private void ResumeEnter(object sender, MouseEventArgs e)
        {
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
    }
}

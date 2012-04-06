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

        /*
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor theOldSensor = (KinectSensor)e.OldValue;
            stopKinect(theOldSensor);

            KinectSensor theNewSensor = (KinectSensor)e.NewValue;


            if (theNewSensor == null)
            {
                return;
            }

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            theNewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            theNewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            theNewSensor.SkeletonStream.Enable(parameters);
            theNewSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(theNewSensor_AllFramesReady);
            try
            {
                theNewSensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            stopKinect(kinectSensorChooser1.Kinect);
        }

        void stopKinect(KinectSensor theSensor)
        {
            if (theSensor != null)
            {
                if (theSensor.IsRunning)
                {
                    theSensor.Stop();
                    if (theSensor.AudioSource != null)
                    {
                        theSensor.AudioSource.Stop();
                    }
                }
            }
        }

        void theNewSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (isClose)
            {
                return;
            }

            Skeleton skl = getSkeleton(e);
            if (skl == null) { return; }

            //GetCameraPoint(skl, e);
            float rightHandDiffY = Math.Abs(skl.Joints[JointType.HandRight].Position.Y - skl.Joints[JointType.ShoulderRight].Position.Y);
            float rightHandDiffX = Math.Abs(skl.Joints[JointType.HandRight].Position.X - skl.Joints[JointType.ShoulderRight].Position.X);
            int lessonCode = 1;
            if (lessonCode == 1)
            { //right hand push forward.
                if (rightHandDiffX > 0.01 && rightHandDiffX < 0.12 &&
                     rightHandDiffY > 0.001 && rightHandDiffY < 0.15)
                {
                    tipsRightHandPush.Content = "That's right. Awesome!!";
                    
                }
                else
                {
                    tipsRightHandPush.Content = "Tips: Lift you right hand up and push straightforward.";
                    tipsRightHandPush.Visibility = Visibility.Visible;
                }
            }

        }
        Skeleton getSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame sklFrame = e.OpenSkeletonFrame())
            {
                if (sklFrame == null)
                {
                    return null;
                }

                sklFrame.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton skl = (from s in allSkeletons
                                where s.TrackingState == SkeletonTrackingState.Tracked
                                select s).FirstOrDefault();

                return skl;

            }
        }
         * */
/*
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

        */


         
    }
}

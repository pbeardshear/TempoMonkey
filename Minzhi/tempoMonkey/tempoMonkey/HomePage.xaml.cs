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

namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        int direction;
        Boolean isReady;

        public HomePage()
        {
            InitializeComponent();
            isReady = false;
            direction = 9999;
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

        private void browseMusic_MouseEnter(object sender, MouseEventArgs e)
        {
            //label1.Content = "enter browse button";
            setSelectionStatus(true);
            direction = 1;
        }

        private void browseMusic_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
            //label1.Content = "Music leave";
        }
        private void browseMusic_Click(object sender, RoutedEventArgs e)
        {
            //DependencyObject s = (DependencyObject)e.OriginalSource;
            //var p = VisualTreeHelper.GetParent(this);

            //((MainWindow)p).Navigate(new Uri("TutorMode.xaml", UriKind.Relative));
            direction = 1;
            //RaiseEvent(new RoutedEventArgs(MainWindow.resetTimer));
        }

        private void learningStudioButton_MouseEnter(object sender, MouseEventArgs e)
        {
            //label1.Content = "enter learning button";
            setSelectionStatus(true);
            direction = 2;
        }

        private void learningStudioButton_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
            //label1.Content = "learning leave";
        }

        private void learningStudioButton_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
            direction = 2;
            //textBox1.Text = GetParent<Frame>(this).ToString();
            GetParent<Frame>(this).Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
            //RaiseEvent(new RoutedEventArgs(MainWindow.theCommand));
            //label1.Content = this.Parent.ToString();
        }

        //get the Frame object in MainWindow
        public static T GetParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParent<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }
/*
        private void LearningStudioPush()
        {
            this.NavigationService.Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
        }
 */
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
            int code = 1;
            textBox1.Text = "iam in";
            if (code == 1)
            { //right hand push forward.
                
                if (rightHandDiffX > 0.01 && rightHandDiffX < 0.12 &&
                     rightHandDiffY > 0.001 && rightHandDiffY < 0.15)
                {
                    timer++;
                    textBox1.Text = "Pushing";
                    //tipsRightHandPush.Content = "That's right. Awesome!!";
                    if (timer <= waitTime)
                    {
                        textBox1.Text = "Dynamic Gesture: Right Hand Push";
                        

                    }
                }
                else
                {
                    timer = 0;
                    //tipsRightHandPush.Content = "Tips: Lift you right hand up and push straightforward.";
                    //tipsRightHandPush.Visibility = Visibility.Visible;
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
       
        */

    }

}

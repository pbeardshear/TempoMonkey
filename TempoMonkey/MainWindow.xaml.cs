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
using System.Drawing;
using System.Collections;

namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int sklCount = 6;
        System.Drawing.Point curPos = new System.Drawing.Point(0, 0);
        Skeleton[] allSkeletons = new Skeleton[sklCount];
        int timer = 0;
        int waitTime = 100;
        string mediaAddress;
        bool isManipulating = false;

        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new HomePage());
            this.setTimer +=new RoutedEventHandler(handle_resetTimer);
        }

        public static readonly RoutedEvent resetTimer =
            EventManager.RegisterRoutedEvent("resetTimer", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainWindow));

        public event RoutedEventHandler setTimer
        {
            add { AddHandler(resetTimer, value); }
            remove { RemoveHandler(resetTimer, value); }
        }

        private void handle_resetTimer(object sender, RoutedEventArgs e)
        {
            timer = 0;
        }

        //hide the NavigationBar
        private void myFrame_ContentRendered(object sender, EventArgs e)
        {
            frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);


            //for the cursor
            System.Drawing.Point winPos = new System.Drawing.Point((int)this.Left, (int)this.Top);
            System.Drawing.Size winSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
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

        void theNewSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!isManipulating)
            {
                handleCursorDetection(e);
            }
            else
            {

            }
        }

        public void handleCursorDetection(AllFramesReadyEventArgs e)
        {
                Object pg = frame.Content;
                Skeleton skl = getSkeleton(e);
                if (skl == null) {
                        return; 
                }
                moveMouse(skl);
    
                float rightHandY = skl.Joints[JointType.HandRight].Position.Y;
                float rightHandX = skl.Joints[JointType.HandRight].Position.X;
                float rightHandDiffZ = Math.Abs(skl.Joints[JointType.HandRight].Position.Z - skl.Joints[JointType.ShoulderRight].Position.Z);
                
                switch(pg.GetType().ToString())
                {
                    case "tempoMonkey.HomePage":
                        HomePage p = (HomePage)pg;
                        if (p.isSelectionReady())
                        {
                            int menu = p.getSelectedMenu();

                            timer++;
                            if (timer > waitTime)
                            {
                                switch (p.getSelectedMenu())
                                {
                                    case 1:
                                        frame.Navigate(new Uri("BrowseMusic.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                    case 2:
                                        frame.Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
                    case "tempoMonkey.LearningStudio":
                        LearningStudio l = (LearningStudio)pg;
                        if (l.isSelectionReady())
                        {
                            int menu = l.getSelectedMenu();
                            timer++;
                            if (timer > waitTime)
                            {
                                switch (l.getSelectedMenu())
                                {
                                    case 1: 
                                        frame.Navigate(new Uri("BrowseTutorials.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                    case 2:
                                        frame.Navigate(new Uri("InteractiveMode.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                    case 3:
                                        frame.Navigate(new Uri("HomePage.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
                    case "tempoMonkey.BrowseTutorials":
                        BrowseTutorials b = (BrowseTutorials)pg;
                        if (rightHandX > 0.0 && rightHandX < 0.12 &&
                           rightHandY > 0.045 && rightHandY < 0.55 &&
                           rightHandDiffZ > 0.16 && rightHandDiffZ < 0.48)  //detect push gesture
                        {
                            timer++;
                            if (timer > 20)
                            {
                                
                                string addr = b.getAddr();
                                frame.Navigate(new TutorMode(addr));
                                timer = 0;
                            }
                        }
                        else
                        {
                            timer = 0;
                        }
                        if (b.isSelectionReady())
                        {
                            int menu = b.getSelectedMenu();

                            timer++;
                            if (timer > waitTime)
                            {
                                switch (b.getSelectedMenu())
                                {
                                    case 3:
                                        frame.Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
                    case "tempoMonkey.BrowseMusic":
                        BrowseMusic m = (BrowseMusic)pg;
                        if (rightHandX > 0.0 && rightHandX < 0.12 &&
                           rightHandY > 0.045 && rightHandY < 0.55 &&
                           rightHandDiffZ > 0.16 && rightHandDiffZ < 0.48)
                        {
                            timer++;
                            if (timer > 20)
                            {
                                if (m.isMenuSelectionValid())
                                {
                                    m.addingToMusicAddrList();
                                    m.addingToMusicList();
                                }
                                timer = 0;
                            }
                        }
                        else if (m.isSelectionReady())
                        {
                            int menu = m.getSelectedMenu();

                            timer++;
                            if (timer > waitTime)
                            {
                                switch (menu)
                                {
                                    case 3:
                                        frame.Navigate(new Uri("HomePage.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                    case 4:
                                        if (m.isSelectionDone())
                                        {
                                            frame.Navigate(new FreeFormMode(m.getMusicAddrList(), m.getMusicList()));
                                        }
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        else {
                            timer = 0;
                        }
                        break;
                    case "tempoMonkey.TutorMode":
                        TutorMode t = (TutorMode)pg;
                        if (t.isSelectionReady())
                        {
                            int menu = t.getSelectedMenu();
                            timer++;
                            if (timer > waitTime)
                            {
                                switch (t.getSelectedMenu())
                                {
                                    case 3:
                                        frame.Navigate(new Uri("BrowseTutorials.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
                    case "tempoMonkey.InteractiveMode":
                        InteractiveMode i = (InteractiveMode)pg;
                        if (i.isSelectionReady())
                        {
                            int menu = i.getSelectedMenu();
                            timer++;
                            if (timer > waitTime)
                            {
                                switch (i.getSelectedMenu())
                                {
                                    case 3:
                                        frame.Navigate(new Uri("LearningStudio.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
                    case "tempoMonkey.FreeFormMode":
                        FreeFormMode f = (FreeFormMode)pg;
                        if (f.isSelectionReady())
                        {
                            int menu = f.getSelectedMenu();
                            timer++;
                            if (timer > waitTime)
                            {
                                switch (f.getSelectedMenu())
                                {
                                    case 3:
                                        frame.Navigate(new Uri("BrowseMusic.xaml", UriKind.Relative));
                                        timer = 0;
                                        break;
                                }
                            }
                        }
                        break;
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

        private void moveMouse(Skeleton skl)
        {
            Joint hand = skl.Joints[JointType.HandRight].ScaleTo((int)this.Width, (int)this.Height, 0.25f, 0.25f);

            double x = hand.Position.X;
            double y = hand.Position.Y;
            System.Windows.Point thePoint = this.PointToScreen(new System.Windows.Point(hand.Position.X, hand.Position.Y));

            curPos.X = (int)thePoint.X;
            curPos.Y = (int)thePoint.Y;
            System.Windows.Forms.Cursor.Position = curPos;
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


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopKinect(kinectSensorChooser1.Kinect);
           
        }

        private Boolean isFrameContentReady()
        {
            Boolean ready = false;
            frame.Navigated += delegate(object sender, NavigationEventArgs e)
            {
                ready = true;
            };
            return ready;
        }

    }
}

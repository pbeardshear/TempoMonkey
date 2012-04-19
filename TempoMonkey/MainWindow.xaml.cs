using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
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
using Processing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    public static class MyExt
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}


namespace TempoMonkey
{

    interface KinectPage
    {
        void allFramesReady(object sender, AllFramesReadyEventArgs e);
    }

    interface CursorPage
    {
        Ellipse getCursor();
        void setCursor(SkeletonPoint skeletonPoint);
    }

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int sklCount = 6;
        System.Drawing.Point curPos = new System.Drawing.Point(0, 0);
        Skeleton[] allSkeletons = new Skeleton[sklCount];
        static public Page currentPage;
        public bool mouseOverride = false;

        #region functions
        static public Button currentlySelectedButton;
        static public int timeOnCurrentButton;

        static public void Mouse_Enter(object sender, MouseEventArgs e)
        {
            currentlySelectedButton = ((Button)sender);
        }

        static public void Mouse_Leave(object sender, MouseEventArgs e)
        {
            currentlySelectedButton = null;
            timeOnCurrentButton = 0;
        }

        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (System.Windows.Input.Key.LeftShift == e.Key)
            {
                mouseOverride = true;
            }
        }

        private void HandleKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (System.Windows.Input.Key.LeftShift == e.Key)
            {
                mouseOverride = false;
            }
        }

        static public void changeFonts(Canvas currCanvas)
        {
            foreach (FrameworkElement c in currCanvas.Children)
            {
                if (c is Label)
                {
                    ((Label)c).FontFamily = new System.Windows.Media.FontFamily("Bold Art");
                }
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(.1);
            Timer.Tick += (delegate(object s, EventArgs args)
            {
                if(!isManipulating){

                    Ellipse currentlySelectedEllipse = ((CursorPage)currentPage).getCursor();
                    if (currentlySelectedButton != null)
                    {
                        if (timeOnCurrentButton >= 55/3)
                        {
                            currentlySelectedButton.PerformClick();
                            //Resize the cursor
                            currentlySelectedEllipse.Width = currentlySelectedEllipse.Height = 25;
                            timeOnCurrentButton = 10;
                        }
                        else
                        {
                            currentlySelectedEllipse.Width = 55 - timeOnCurrentButton*3;
                            currentlySelectedEllipse.Height = 55 - timeOnCurrentButton*3;
                            timeOnCurrentButton++;
                        }
                    }
                    else
                    {
                        if (currentlySelectedEllipse.Width <= 55)
                        {
                            currentlySelectedEllipse.Width += 6;
                            currentlySelectedEllipse.Height += 6;
                        }
                    }
                }
            });

            Page homepage = new HomePage();
            frame.Navigate(homepage);
            currentPage = homepage;
            Timer.Start();
            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
            AddHandler(Keyboard.KeyUpEvent, (KeyEventHandler)HandleKeyUpEvent);
            //System.Windows.Forms.Cursor.Hide();
            //this.Cursor = new Cursor();
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
            theNewSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(allFramesReady);
            try
            {
                theNewSensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        public static bool isManipulating = false;
        void allFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (isManipulating)
            {
                ((KinectPage)currentPage).allFramesReady(sender, e);
            }
            else
            {
                Skeleton skl = getSkeleton(e);
                if (skl != null && !mouseOverride)
                {
                    moveMouse(skl);
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

        private void moveMouse(Skeleton skl)
        {
            Joint hand = skl.Joints[JointType.HandRight].ScaleTo((int)this.Width, (int)this.Height, 0.25f, 0.25f);

            double x = hand.Position.X;
            double y = hand.Position.Y;
            System.Windows.Point thePoint = this.PointToScreen(new System.Windows.Point(hand.Position.X, hand.Position.Y));

            curPos.X = (int)thePoint.X;
            curPos.Y = (int)thePoint.Y;
            System.Windows.Forms.Cursor.Position = curPos;
            ((CursorPage)currentPage).setCursor(hand.Position);
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

    }
}

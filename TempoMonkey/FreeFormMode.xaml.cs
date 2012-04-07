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
        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer freePlayer;

        public void freeAllFramesReady(object sender, AllFramesReadyEventArgs e){
            Skeleton skeleton = KinectGesturePlayer.getFristSkeleton(e);
            if (skeleton != null)
            {
                freePlayer.skeletonReady(e, skeleton);
                DebugBox.Content = KinectGesturePlayer.GetBodySegmentAngle(
                    freePlayer.currLeftHand, 
                    freePlayer.currLeftShoulder, 
                    freePlayer.currHipCenter).ToString();
            }
        }

        //Handlers
        void pauseTrackingHandler(bool exist)
        {
            pause.Fill = exist ? (Brush)bc.ConvertFrom("GREEN") : (Brush)bc.ConvertFrom("RED");
            // Add a pop-up with option to continue or quit
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
            freePlayer = new KinectGesturePlayer();
            freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, debugTrackerHandler);
            freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            freePlayer.registerCallBack(freePlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            freePlayer.registerCallBack(freePlayer.fistsPumpListener, tempoTrackingHandler, tempoChangeHandler);
            freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
        }
    }
}

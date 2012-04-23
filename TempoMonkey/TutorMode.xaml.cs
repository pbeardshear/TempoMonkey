using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Windows.Media.Animation;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class TutorMode : Page, KinectPage
    {
        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer tutoree;
        bool isPaused = false;

        string mySelection;

        public TutorMode(string selection)
        {
            InitializeComponent();
            mySelection = selection;
            /*
            Processing.Audio.Initialize();
            Processing.Audio.LoadFile("C:\\Users\\Doboy\\Desktop\\Minh\\TempoMonkey\\bin\\Debug\\Music\\Enough To Fly With You.mp3");
            Processing.Audio.LoadFile("C:\\Users\\Doboy\\Desktop\\Minh\\TempoMonkey\\bin\\Debug\\Music\\Chasing Pavements.mp3");
            Processing.Audio.Play();

            tutoree = new KinectGesturePlayer();
            tutoree.registerCallBack(tutoree.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            tutoree.registerCallBack(tutoree.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutoree.registerCallBack(tutoree.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutoree.registerCallBack(tutoree.leanListener, tempoTrackingHandler, tempoChangeHandler);
            tutoree.registerCallBack(tutoree.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
            MainWindow.changeFonts(mainCanvas);
             * */
        }

        public void allFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!isPaused)
            {
                Skeleton skeleton = KinectGesturePlayer.getFristSkeleton(e);

                if (skeleton != null)
                {
                    tutoree.skeletonReady(e, skeleton);
                }
            }
        }

        public void setCursor(Microsoft.Kinect.SkeletonPoint point)
        {
            FrameworkElement element = myCursor;
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);
        }



        #region Gesture Handlers
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


        int previousTrack = 1;
        void changeTrackHandler(double value)
        {
            if (!wasSeeking)
            {
                SeekSlider.Value += .05; // THIS IS NOT REALLY TRUE
            }

            if (value < 250 && previousTrack != 1)
            {
                previousTrack = 1;
            }
            else if (value > 450 && previousTrack != 2)
            {
                previousTrack = 2;
            }
            else if (value >= 250 && value <= 450 && previousTrack != 0)
            {
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
            VolumeFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        void tempoChangeHandler(double change)
        {
            TempoSlider.Value += change / 2;
            Processing.Audio.ChangeTempo(TempoSlider.Value);
        }


        void tempoTrackingHandler(bool exist)
        {
            Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            TempoFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
            PitchFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void Resumee()
        {
            isPaused = false;
            Processing.Audio.Play();
            Border.Visibility = System.Windows.Visibility.Hidden;
            Resume.Visibility = System.Windows.Visibility.Hidden;
            Quit.Visibility = System.Windows.Visibility.Hidden;
            myCursor.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.isManipulating = true;
        }

        public void Pause()
        {
            isPaused = true;
            Processing.Audio.Play();
            Border.Visibility = System.Windows.Visibility.Visible;
            Resume.Visibility = System.Windows.Visibility.Visible;
            Quit.Visibility = System.Windows.Visibility.Visible;
            myCursor.Visibility = System.Windows.Visibility.Visible;
            MainWindow.isManipulating = false;
        }

        #endregion

        #region Navigation
        void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        void Quit_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception();
        }

        void Resume_Click(object sender, RoutedEventArgs e)
        {
            Resumee();
        }

        #endregion


        public Ellipse getCursor()
        {
            return myCursor;
        }
    }
}

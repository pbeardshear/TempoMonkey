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
    public partial class TutorMode : Page, KinectPage, CursorPage
    {
        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer tutoree;
        bool isPaused = false;

        public TutorMode()
        {
            InitializeComponent();

            Processing.Audio.Initialize();
            Processing.Audio.LoadFile("C:\\Users\\Doboy\\Desktop\\Minh\\TempoMonkey\\bin\\Debug\\Music\\Enough To Fly With You.mp3");
            Processing.Audio.LoadFile("C:\\Users\\Doboy\\Desktop\\Minh\\TempoMonkey\\bin\\Debug\\Music\\Chasing Pavements.mp3");
            Processing.Audio.Play();

            initVisualizer();
            tutoree = new KinectGesturePlayer();
            tutoree.registerCallBack(tutoree.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            tutoree.registerCallBack(tutoree.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutoree.registerCallBack(tutoree.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutoree.registerCallBack(tutoree.leanListener, tempoTrackingHandler, tempoChangeHandler);
            tutoree.registerCallBack(tutoree.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
            MainWindow.changeFonts(mainCanvas);
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

        #region Visualization
        private Storyboard myStoryboard;
        private float ione = 125F;
        private float itwo = .5F;
        private float ithree = 125F;
        private float ifour = .5F;
        private float ifive = 125F;
        private float isix = .5F;
        //private Grid myPanel = new Grid();
        private Ellipse eone = new Ellipse();
        private Ellipse etwo = new Ellipse();
        private Ellipse ethree = new Ellipse();
        private Ellipse efour = new Ellipse();
        private Ellipse efive = new Ellipse();
        private Ellipse esix = new Ellipse();
        public void initVisualizer()
        {
            //Ellipse eone = new Ellipse();
            eone.Name = "one";
            this.RegisterName(eone.Name, eone);
            eone.Fill = Brushes.Blue;
            Grid.SetColumn(eone, 0);
            //Ellipse etwo = new Ellipse();
            etwo.Name = "two";
            this.RegisterName(etwo.Name, etwo);
            etwo.Width = 100;
            etwo.Height = 100;
            etwo.Fill = Brushes.Violet;
            Grid.SetColumn(etwo, 1);
            //Ellipse ethree = new Ellipse();
            ethree.Name = "three";
            this.RegisterName(ethree.Name, ethree);
            ethree.Fill = Brushes.Blue;
            Grid.SetColumn(ethree, 2);
            //Ellipse efour = new Ellipse();
            efour.Name = "four";
            this.RegisterName(efour.Name, efour);
            efour.Width = 100;
            efour.Height = 100;
            efour.Fill = Brushes.Violet;
            Grid.SetColumn(efour, 0);
            Grid.SetRow(efour, 1);
            //Ellipse efive = new Ellipse();
            efive.Name = "five";
            this.RegisterName(efive.Name, efive);
            efive.Fill = Brushes.Blue;
            Grid.SetColumn(efive, 1);
            Grid.SetRow(efive, 1);
            //Ellipse esix = new Ellipse();
            esix.Name = "six";
            this.RegisterName(esix.Name, esix);
            esix.Width = 100;
            esix.Height = 100;
            esix.Fill = Brushes.Violet;
            Grid.SetColumn(esix, 2);
            Grid.SetRow(esix, 1);
            visualize();
        }

        //take a sample of data [3 points]
        //values start from 125 or .5 and go to a ratioed value (/12*25+125 or <1:-(4/100x),>=1:+(x/6))
        //next sample is .to, previous .to is .from
        private void visualize()//float one, float three, float five, float two, float four, float six)
        {
            //myPanel.Children.RemoveRange(0, 6);

            DoubleAnimation aone = new DoubleAnimation(); //pitch
            aone.From = 100;
            aone.To = 75;
            aone.Duration = new Duration(TimeSpan.FromSeconds(3));
            aone.AutoReverse = true;
            aone.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation bone = new DoubleAnimation();
            bone.From = 100;
            bone.To = 75;
            bone.Duration = new Duration(TimeSpan.FromSeconds(3));
            bone.AutoReverse = true;
            bone.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation atwo = new DoubleAnimation(); //tempo
            atwo.From = 1;
            atwo.To = .1;
            atwo.Duration = new Duration(TimeSpan.FromSeconds(1));
            atwo.AutoReverse = true;
            atwo.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation athree = new DoubleAnimation();
            athree.From = 100.0;
            athree.To = 75.0;
            athree.Duration = new Duration(TimeSpan.FromSeconds(4));
            athree.AutoReverse = true;
            athree.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation bthree = new DoubleAnimation();
            bthree.From = 100.0;
            bthree.To = 75.0;
            bthree.Duration = new Duration(TimeSpan.FromSeconds(4));
            bthree.AutoReverse = true;
            bthree.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation afour = new DoubleAnimation();
            afour.From = 1.0;
            afour.To = 0.1;
            afour.Duration = new Duration(TimeSpan.FromSeconds(4));
            afour.AutoReverse = true;
            afour.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation afive = new DoubleAnimation();
            afive.From = 100.0;
            afive.To = 75.0;
            afive.Duration = new Duration(TimeSpan.FromSeconds(1));
            afive.AutoReverse = true;
            afive.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation bfive = new DoubleAnimation();
            bfive.From = 100.0;
            bfive.To = 75.0;
            bfive.Duration = new Duration(TimeSpan.FromSeconds(1));
            bfive.AutoReverse = true;
            bfive.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation asix = new DoubleAnimation();
            asix.From = 1.0;
            asix.To = 0.1;
            asix.Duration = new Duration(TimeSpan.FromSeconds(3));
            asix.AutoReverse = true;
            asix.RepeatBehavior = RepeatBehavior.Forever;

            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(aone);
            Storyboard.SetTargetName(aone, eone.Name);
            Storyboard.SetTargetProperty(aone, new PropertyPath(Ellipse.HeightProperty));
            myStoryboard.Children.Add(bone);
            Storyboard.SetTargetName(bone, eone.Name);
            Storyboard.SetTargetProperty(bone, new PropertyPath(Ellipse.WidthProperty));
            myStoryboard.Children.Add(atwo);
            Storyboard.SetTargetName(atwo, etwo.Name);
            Storyboard.SetTargetProperty(atwo, new PropertyPath(Ellipse.OpacityProperty));
            myStoryboard.Children.Add(athree);
            Storyboard.SetTargetName(athree, ethree.Name);
            Storyboard.SetTargetProperty(athree, new PropertyPath(Ellipse.HeightProperty));
            myStoryboard.Children.Add(bthree);
            Storyboard.SetTargetName(bthree, ethree.Name);
            Storyboard.SetTargetProperty(bthree, new PropertyPath(Ellipse.WidthProperty));
            myStoryboard.Children.Add(afour);
            Storyboard.SetTargetName(afour, efour.Name);
            Storyboard.SetTargetProperty(afour, new PropertyPath(Ellipse.OpacityProperty));
            myStoryboard.Children.Add(afive);
            Storyboard.SetTargetName(afive, efive.Name);
            Storyboard.SetTargetProperty(afive, new PropertyPath(Ellipse.HeightProperty));
            myStoryboard.Children.Add(bfive);
            Storyboard.SetTargetName(bfive, efive.Name);
            Storyboard.SetTargetProperty(bfive, new PropertyPath(Ellipse.WidthProperty));
            myStoryboard.Children.Add(asix);
            Storyboard.SetTargetName(asix, esix.Name);
            Storyboard.SetTargetProperty(asix, new PropertyPath(Ellipse.OpacityProperty));

            // Use the Loaded event to start the Storyboard.
            eone.Loaded += new RoutedEventHandler(oneLoaded);
            myPanel.Children.Add(eone);
            etwo.Loaded += new RoutedEventHandler(twoLoaded);
            myPanel.Children.Add(etwo);
            ethree.Loaded += new RoutedEventHandler(oneLoaded);
            myPanel.Children.Add(ethree);
            efour.Loaded += new RoutedEventHandler(twoLoaded);
            myPanel.Children.Add(efour);
            efive.Loaded += new RoutedEventHandler(oneLoaded);
            myPanel.Children.Add(efive);
            esix.Loaded += new RoutedEventHandler(twoLoaded);
            myPanel.Children.Add(esix);
            //this.Content = myPanel;

            /*ione = one;
            itwo = two;
            ithree = three;
            ifour = four;
            ifive = five;
            isix = six;*/
        }

        private void oneLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }
        private void twoLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }
        private void threeLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }
        private void fourLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }
        private void fiveLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }
        private void sixLoaded(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin(this);
        }

        #endregion

        public Ellipse getCursor()
        {
            return myCursor;
        }
    }
}

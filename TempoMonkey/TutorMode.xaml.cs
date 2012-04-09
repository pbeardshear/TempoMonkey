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
using System.Windows.Media.Animation;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class TutorMode : Page
    {
        bool isPaused = false;
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

            Processing.Audio.Initialize();
            Processing.Audio.LoadFile("C:\\Users\\Doboy\\Desktop\\Minh\\TempoMonkey\\bin\\Debug\\Music\\Enough To Fly With You.mp3");
            Processing.Audio.Play();

            initVisualizer();
            tutorPlayer = new KinectGesturePlayer();
            tutorPlayer.registerCallBack(tutorPlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handsUppenListener, tempoTrackingHandler, tempoChangeHandler);
            tutorPlayer.registerCallBack(tutorPlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
       
            Instructions.Text = TaskInstructions[currentTaskIndex];
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
        String[] Task = {"Seek", "Volume", "Pitch", "Switch Tracks" };
        String[] TaskInstructions = {"To Seek through a track just place your right hand up and move it left and right",
                                    "To change volume put both of your hands into your mid section and expand/impact them",
                                    "To change the pitch put both hands over your head and move your head up or down",
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
                Instructions.Text = "You have completed everything.. good job";
            }
            else
            {
                currentTaskIndex++;
                Instructions.Text = TaskInstructions[currentTaskIndex];
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


        int previousTrack = 1;
        void changeTrackHandler(double value)
        {
            if (!wasSeeking)
            {
                SeekSlider.Value += .1; // THIS IS NOT REALLY TRUE
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
            proceedIfGood(exist, "Volume");
        }

        void tempoChangeHandler(double change)
        {
            TempoSlider.Value += change / 2;
            Processing.Audio.ChangeTempo(TempoSlider.Value);

        }


        void tempoTrackingHandler(bool exist)
        {
            Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            proceedIfGood(exist, "Tempo");
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
                    if (Task[currentTaskIndex] == "Seek")
                    {
                        proceed();
                    }
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
            proceedIfGood(exist, "Pitch");
        }
        //Tell the MainWindow which menu button has been selected

        public void unPause()
        {
            isPaused = false;
            Processing.Audio.Play();
            Border.Visibility = System.Windows.Visibility.Hidden;
            Resume.Visibility = System.Windows.Visibility.Hidden;
            Quit.Visibility = System.Windows.Visibility.Hidden;
        }

        public void Pause()
        {
            isPaused = true;
            Processing.Audio.Pause();
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
    }
}

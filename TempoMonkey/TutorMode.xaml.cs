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
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Threading;

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
        public delegate bool check();
        private Dictionary<Tutorial, check> _checkers = new Dictionary<Tutorial, check>();


        #region Tutorials
        public class Tutorial
        {
            static List<Tutorial> _tutorials = new List<Tutorial>();
            public static int _tutorialIndex;
            string _name, _instructions;
            Uri _source;
            check _checker;

            public Tutorial(string name, string instructions, Uri source, check checker)
            {
                _name = name;
                _instructions = instructions;
                _source = source;
                _checker = checker;
                _tutorials.Add(this);
            }

            public Uri getSource()
            {
                return _source;
            }

            public string getInstructions()
            {
                return _instructions;
            }

            public string getName()
            {
                return _name;
            }

            public static Tutorial getCurrentTutorial()
            {
                return _tutorials[_tutorialIndex];
            }

            public static string getCurrentTutorialName()
            {
                return _tutorials[_tutorialIndex].getName();
            }

            public static void addTutorial(Tutorial tutorial)
            {
                _tutorials.Add(tutorial);
            }

            public static bool checkTask()
            {
                if (doNext)
                {
                    doNext = false;
                    return true;
                }
                return _tutorials[_tutorialIndex]._checker();
            }

            public static Tutorial nextTutorial()
            {
                _tutorialIndex++;
                if (_tutorialIndex < _tutorials.Count)
                {
                    return _tutorials[_tutorialIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public void startTutorial(Tutorial tutorial)
        {
            myMediaElement.Source = tutorial.getSource();
            Instructions.Content = tutorial.getInstructions();
            myMediaElement.Play();
        }

        DispatcherTimer Timer;
        public void initTutorials(int index){
            Seek.Content = index;
            System.Windows.Forms.Cursor.Hide();


            startTutorial(Tutorial.getCurrentTutorial());
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(2);
            Timer.Tick += (delegate(object s, EventArgs args){
                //Checks if the user has finished the task, and queues up the next task
                if (Tutorial.checkTask())
                {
                    Tutorial next = Tutorial.nextTutorial();
                    if (next != null)
                    {
                        showTutorialChooser(next);
                    }
                    else
                    {
                        showTutorialsFinished();
                        Timer.Stop();
                    }
                }
            });
            Timer.Start();
        }


        public void showTutorialsFinished()
        {
            throw new Exception();
        }

        public void showTutorialChooser(Tutorial tutorial)
        {
            MainWindow.isManipulating = false;
            Next.Visibility = Tutorials.Visibility = Quit.Visibility = Border.Visibility = System.Windows.Visibility.Visible;
        }

        bool donePause = false;
        public bool pauseChecker()
        {
            return donePause;
        }

        bool doneTempo = false;
        public bool tempoChecker()
        {
            return doneTempo;
        }

        bool doneVolume = false;
        public bool volumeChecker()
        {
            return doneVolume;
        }

        bool donePitch = false;
        public bool pitchChecker()
        {
            return donePitch;
        }

        bool doneSwitchTrack = false;
        public bool switchTrackChecker()
        {
            return doneSwitchTrack;
        }

        bool doneSeeking = false;
        public bool seekChecker()
        {
            return doneSeeking;
        }
        #endregion

        public void initTutor(int index)
        {
            Processing.Audio.LoadFile(@"..\..\Resources\Music\Chasing Pavements.mp3");
            Processing.Audio.LoadFile(@"..\..\Resources\Music\Enough To Fly With You.mp3");
            Processing.Audio.Play();
            initTutorials(index);
        }

        public void tearDown()
        {
            Timer.Stop();
            // TODO: TEARDOWN MUSIC... unload all files and whatever else that needs to be done
            // so that a user can navigate between pages that uses music
        }

        public TutorMode()
        {
            InitializeComponent();



            Tutorial pause, volume, tempo, pitch, switch_tracks, seek;
            Tutorial._tutorialIndex = 0;
            string tutorials_base = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tutorials\\";

            pause = new Tutorial("Pause", "To pause move your left arm to a 35 degree angle with your body",
                new Uri(tutorials_base + "pause.m4v"), pauseChecker);
            tempo = new Tutorial("Changing the Tempo", "To increase the tempo lean towards the right, to decrease the tempo lean towards the left",
                new Uri(tutorials_base + "tempo.m4v"), tempoChecker);
            pitch = new Tutorial("Changing the Pitch", "To increase/decrease the pitch put your arms above your head and move your body up/down",
                new Uri(tutorials_base + "pitch.m4v"), pitchChecker);
            switch_tracks = new Tutorial("Switching Tracks", "To switch between your tracks jump to the left/right",
                new Uri(tutorials_base + "switch_tracks.m4v"), switchTrackChecker);
            volume = new Tutorial("Changing the Volume", "To change the volume put both your arms in the midsection of your body and expand/intract your hands",
                new Uri(tutorials_base + "volume.m4v"), volumeChecker);
            seek = new Tutorial("Changing the Position of the track", "To seek around the track put your right hand up and hover it left and right",
                new Uri(tutorials_base + "seek.m4v"), seekChecker);

            Tutorial.addTutorial(pause);
            Tutorial.addTutorial(tempo);
            Tutorial.addTutorial(pitch);
            Tutorial.addTutorial(switch_tracks);
            Tutorial.addTutorial(volume);
            Tutorial.addTutorial(seek);

            tutoree = new KinectGesturePlayer();
            tutoree.registerCallBack(tutoree.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            tutoree.registerCallBack(tutoree.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutoree.registerCallBack(tutoree.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutoree.registerCallBack(tutoree.leanListener, tempoTrackingHandler, tempoChangeHandler);
            tutoree.registerCallBack(tutoree.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
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

        #region Gesture Handlers
        void pauseTrackingHandler(bool exist)
        {
            if (isPaused)
            {
                return;
            }

            if (exist)
            {
                if (Tutorial.getCurrentTutorialName() == "Pause")
                {
                    donePause = true;
                }
                Pause();
            }
        }


        int previousTrack = 1;
        int totalTrackChange = 0;
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

            if (Tutorial.getCurrentTutorialName() == "Pitch")
            {
                totalTrackChange++;
                if (totalTrackChange >= 2){
                    doneSwitchTrack = true;
                }
            }
        
            Processing.Audio.SwapTrack(previousTrack);
            Processing.Audio.Seek(SeekSlider.Value);
            Processing.Audio.ChangeVolume(VolumeSlider.Value);
            Processing.Audio.ChangeTempo(TempoSlider.Value);
            Processing.Audio.ChangePitch(PitchSlider.Value);
        }

        double totalVolumeChange = 0;
        void volumeChangeHandler(double change)
        {
            VolumeSlider.Value -= change;
            Processing.Audio.ChangeVolume(VolumeSlider.Value);
            if (Tutorial.getCurrentTutorialName() == "Volume")
            {
                totalVolumeChange += Math.Abs(change);
                if (totalVolumeChange > 10)
                {
                    doneVolume = true;
                }
            }
        }

        void volumeTrackingHandler(bool exist)
        {
            Volume.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            VolumeFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        double totalTempoChange = 0;
        void tempoChangeHandler(double change)
        {
            TempoSlider.Value += change / 2;
            Processing.Audio.ChangeTempo(TempoSlider.Value);
            if (Tutorial.getCurrentTutorialName() == "Tempo")
            {
                totalTempoChange += Math.Abs(change);
                if (totalTempoChange > 10)
                {
                    doneTempo = true;
                }
            }
        }

        void tempoTrackingHandler(bool exist)
        {
            Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            TempoFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        bool wasSeeking = false;
        double totalSeekingChange = 0;
        void seekChangeHandler(double change)
        {
            SeekSlider.Value += change;
            if (Tutorial.getCurrentTutorialName() == "Seek")
            {
                totalSeekingChange += Math.Abs(change);
            }
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
                    if (totalSeekingChange >= 10)
                    {
                        doneSeeking = true;
                    }
                }
                wasSeeking = false;
            }
        }

        double totalPitchChange = 0;
        void pitchChangeHandler(double change)
        {
            PitchSlider.Value -= change * 3;
            Processing.Audio.ChangePitch(PitchSlider.Value);
            if (Tutorial.getCurrentTutorialName() == "Pitch")
            {
                totalPitchChange += Math.Abs(change);
                if (totalPitchChange > 10)
                {
                    donePitch = true;
                }
            }
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
            MainWindow.isManipulating = true;
        }

        public void Pause()
        {
            isPaused = true;
            Processing.Audio.Play();
            Border.Visibility = System.Windows.Visibility.Visible;
            Resume.Visibility = System.Windows.Visibility.Visible;
            Quit.Visibility = System.Windows.Visibility.Visible;
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
            MainWindow.isManipulating = false;
            MainWindow.currentPage = new HomePage();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        void Resume_Click(object sender, RoutedEventArgs e)
        {
            Resumee();
        }

        void Tutorials_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.isManipulating = false;
            MainWindow.currentPage = new BrowseTutorials();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        void Next_Click(object sender, RoutedEventArgs e)
        {
            Next.Visibility = Tutorials.Visibility = Quit.Visibility = Border.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.isManipulating = true;
            startTutorial(Tutorial.getCurrentTutorial());
        }

        static bool doNext = false;
        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            doNext = true;
        }
        #endregion

    }
}

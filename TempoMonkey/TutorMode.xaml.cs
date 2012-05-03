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
using Visualizer;
using slidingMenu;
using System.Windows.Threading;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class TutorMode : Page, KinectPage
    {
        public mySlider VolumeSlider, PitchSlider, TempoSlider;
        BrushConverter bc = new BrushConverter();
        KinectGesturePlayer tutoree;
        ArrayList _nameList = new ArrayList();
        bool isPaused = false;
        public delegate bool check();
        public Spectrum Visualizer;
        private Dictionary<Tutorial, check> _checkers = new Dictionary<Tutorial, check>();


        public void playTutorial(Tutorial tutorial)
        {
            myMediaElement.Source = tutorial.getSource();
            Instructions.Content = tutorial.getInstructions();
            myMediaElement.Play();
        }

        DispatcherTimer Timer;

        public void showTutorialsFinished()
        {
            MainWindow.setManipulating(false);
            System.Windows.Forms.Cursor.Show();
            Timer.Stop();
        }

        public void showTutorialChooser(Tutorial tutorial)
        {
            MainWindow.setManipulating(false);

            NextOverLay.Visibility = System.Windows.Visibility.Visible;
            System.Windows.Forms.Cursor.Show();
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

        Label[] SongTitles = new Label[3];
        Panel[] waveFormContainers = new Panel[3];
        int[] positions = { 260, 525, 850 };
        public void initCommon()
        {
            waveFormContainers[0] = SongContainer0;
            waveFormContainers[1] = SongContainer1;
            waveFormContainers[2] = SongContainer2;
            waveFormContainers[0].Visibility = System.Windows.Visibility.Hidden;
            waveFormContainers[1].Visibility = System.Windows.Visibility.Hidden;
            waveFormContainers[2].Visibility = System.Windows.Visibility.Hidden;
            SongTitles[0] = SongTitle0;
            SongTitles[1] = SongTitle1;
            SongTitles[2] = SongTitle2;
            SongTitles[0].Content = null;
            SongTitles[1].Content = null;
            SongTitles[2].Content = null;

            Visualizer = new Spectrum(mainCanvas);
            Visualizer.RegisterSoundPlayer();
            MainWindow.setManipulating(true);
        }

        public void initWaveForm(Panel waveFormContainer, string uri)
        {
            Visualizer.Timeline.WaveformTimeline wave = new Visualizer.Timeline.WaveformTimeline(waveFormContainer, uri);
            wave.Draw();
        }

        public void initTutor(int index)
        {
            List<string> nameList = new List<string>{ "Chasing Pavements", "Enough To Fly With You" };
            List<string> addrList = new List<string>{ @"..\..\Resources\Music\Chasing Pavements.mp3", @"..\..\Resources\Music\Enough To Fly With You.mp3" };
            initCommon();

            // Load and set the song titles
            for (int i = 0; i < addrList.Count; i++)
            {
                string address = addrList[i] as String;
                string name = nameList[i] as String;
                initWaveForm(waveFormContainers[i], address);
                _nameList.Add(name);
                SongTitles[i].Content = name;
                Processing.Audio.LoadFile(address);
            }

            // Set the current track & also plays it
            Processing.Audio.Play();
            currentTrackIndex = _nameList.Count > 1 ? 1 : 0;

            // connected to gestures
            tutoree = new KinectGesturePlayer();
            tutoree.registerCallBack(tutoree.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            tutoree.registerCallBack(tutoree.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutoree.registerCallBack(tutoree.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutoree.registerCallBack(tutoree.leanListener, tempoTrackingHandler, tempoChangeHandler);
            tutoree.registerCallBack(tutoree.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);

            Tutorial.setIndex(index);
            playTutorial(Tutorial.getCurrentTutorial());
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(2);
            Timer.Tick += (delegate(object s, EventArgs args)
            {
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

        public void tearDown()
        {
            tutoree = null;
            Timer.Stop();
            MainWindow.setManipulating(false);
            Processing.Audio.End();
            PauseOverlay.Visibility = System.Windows.Visibility.Hidden;
            NextOverLay.Visibility = System.Windows.Visibility.Hidden;
        }

        public void initSliders()
        {
            VolumeSlider = new mySlider("Volume", 10, 100, 25, 250);
            PitchSlider = new mySlider("Pitch", 0, 100, 50, 250);
            TempoSlider = new mySlider("Tempo", 40, 200, 100, 250);

            Canvas.SetLeft(VolumeSlider, 450);
            Canvas.SetLeft(PitchSlider, 450);
            Canvas.SetLeft(TempoSlider, 450);

            Canvas.SetTop(VolumeSlider, 200);
            Canvas.SetTop(PitchSlider, 300);
            Canvas.SetTop(TempoSlider, 400);

            mainCanvas.Children.Add(VolumeSlider);
            mainCanvas.Children.Add(PitchSlider);
            mainCanvas.Children.Add(TempoSlider);
        }

        public int currentTrackIndex
        {
            set
            {
                waveFormContainers[_currentTrackIndex].Visibility = System.Windows.Visibility.Hidden;
                SongTitles[_currentTrackIndex].Foreground = ((System.Windows.Media.Brush)bc.ConvertFrom("#666"));
                _currentTrackIndex = value;
                waveFormContainers[_currentTrackIndex].Visibility = System.Windows.Visibility.Visible;
                SongTitles[_currentTrackIndex].Foreground = ((System.Windows.Media.Brush)bc.ConvertFrom("#FFF"));
                Canvas.SetLeft(BlueDot, positions[_currentTrackIndex]);
                Processing.Audio.SwapTrack(_currentTrackIndex);
                Processing.Audio.Seek(SeekSlider.Value);
                Processing.Audio.ChangeVolume(VolumeSlider.Value);
                Processing.Audio.ChangeTempo(TempoSlider.Value);
                Processing.Audio.ChangePitch(PitchSlider.Value);
            }
            get
            {
                return _currentTrackIndex;
            }
        }

        int _currentTrackIndex;

        Tutorial pause, volume, tempo, pitch, switch_tracks, seek;
        public TutorMode()
        {
            InitializeComponent();
            initSliders();
            Tutorial._tutorialIndex = 0;
            string tutorials_base = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tutorials\\";

            pause = new Tutorial("Pause", "To pause move your left arm to a 35 degree angle with your body",
                new Uri(tutorials_base + "pause.m4v"), pauseChecker);
            tempo = new Tutorial("Changing the Tempo", "To increase the tempo lean towards the right, to decrease the tempo lean towards the left",
                new Uri(tutorials_base + "01tempo.m4v"), tempoChecker);
            pitch = new Tutorial("Changing the Pitch", "To increase/decrease the pitch put your arms above your head and move your body up/down",
                new Uri(tutorials_base + "02pitch.m4v"), pitchChecker);
            switch_tracks = new Tutorial("Switching Tracks", "To switch between your tracks jump to the left/right",
                new Uri(tutorials_base + "03switchtracks.m4v"), switchTrackChecker);
            volume = new Tutorial("Changing the Volume", "To change the volume put both your arms in the midsection of your body and expand/intract your hands",
                new Uri(tutorials_base + "04volume.m4v"), volumeChecker);
            seek = new Tutorial("Changing the Position of the track", "To seek around the track put your right hand up and hover it left and right",
                new Uri(tutorials_base + "05seek.m4v"), seekChecker);

            //Tutorial.addTutorial(pause);
            Tutorial.addTutorial(tempo);
            Tutorial.addTutorial(pitch);
            Tutorial.addTutorial(switch_tracks);
            Tutorial.addTutorial(volume);
            Tutorial.addTutorial(seek);

            quitButton = new NavigationButton(QuitButton, delegate()
            {
                tearDown();
                return MainWindow.homePage;
            });

            tutorialsButton = new NavigationButton(TutorialsButton, delegate()
            {
                tearDown();
                return MainWindow.browseTutorialsPage;
            });

            new NavigationButton(ResumeButton, delegate()
            {
                Resume();
                return null;
            });

            new NavigationButton(NextTutorial, delegate()
            {
                NextOverLay.Visibility = System.Windows.Visibility.Hidden;
                MainWindow.setManipulating(true);
                mainCanvas.Background = new SolidColorBrush(Colors.Gray);
                playTutorial(Tutorial.getCurrentTutorial());
                return null;
            });
        }

        NavigationButton quitButton, tutorialsButton;
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
                if (Tutorial.getCurrentTutorial() == pause)
                {
                    donePause = true;
                }
                Pause();
            }
        }

        int totalTrackChange = 0;
        int midPoint = 325;
        int span = 190;		
        void changeTrackHandler(double value)
        {
            if (!wasSeeking)
            {
                SeekSlider.Value += .05;
            }

            if (value < midPoint - span && currentTrackIndex != 0 && _nameList.Count > 1)
            {
                currentTrackIndex = 0;
            }
            else if (value > midPoint + span && currentTrackIndex != 2 && _nameList.Count > 2)
            {
                currentTrackIndex = 2;
            }
            else if (value >= midPoint - span && value <= midPoint + span && currentTrackIndex != 1)
            {
                currentTrackIndex = 1;
            }
            else
            {
                return;
            }

            if (Tutorial.getCurrentTutorial() == switch_tracks)
            {
                totalTrackChange++;
                if (totalTrackChange >= 3){
                    doneSwitchTrack = true;
                }
            }
        }

        double totalVolumeChange = 0;
        void volumeChangeHandler(double change)
        {
            VolumeSlider.Value -= change;
            Processing.Audio.ChangeVolume(VolumeSlider.Value);
            if (Tutorial.getCurrentTutorial() == volume)
            {
                totalVolumeChange += Math.Abs(change);
                if (totalVolumeChange > 100)
                {
                    doneVolume = true;
                }
            }
        }

        void volumeTrackingHandler(bool exist)
        {
            VolumeSlider.player1Exists = exist;
            // SetAvatarState(exist, volumeAvatar, exist ? loadedImages["volumeAvatar"] : loadedImages["volumeAvatarDisabled"]);
        }

        double totalTempoChange = 0;
        void tempoChangeHandler(double change)
        {
            TempoSlider.Value += change / 2;
            Processing.Audio.ChangeTempo(TempoSlider.Value);
            if (Tutorial.getCurrentTutorial() == tempo)
            {
                totalTempoChange += Math.Abs(change);
                Seek.Content = totalTempoChange;
                if (totalTempoChange > 150)
                {
                    doneTempo = true;
                }
            }
        }

        void tempoTrackingHandler(bool exist)
        {
            TempoSlider.player1Exists = exist;
            // SetAvatarState(exist, tempoAvatar, exist ? loadedImages["tempoAvatar"] : loadedImages["tempoAvatarDisabled"]);
        }

        bool wasSeeking = false;
        double totalSeekingChange = 0;
        void seekChangeHandler(double change)
        {
            SeekSlider.Value += change;
            if (Tutorial.getCurrentTutorial() == seek)
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
                    if (totalSeekingChange >= 100)
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
            if (Tutorial.getCurrentTutorial() == pitch)
            {
                totalPitchChange += Math.Abs(change);
                Seek.Content = totalPitchChange;
                if (totalPitchChange > 50)
                {
                    donePitch = true;
                }
            }
        }

        void pitchTrackingHandler(bool exist)
        {
            PitchSlider.player1Exists = exist;
            // SetAvatarState(exist, pitchAvatar, exist ? loadedImages["pitchAvatar"] : loadedImages["pitchAvatarDisabled"]);
        }

        public void Resume()
        {
            isPaused = false;
            Processing.Audio.Resume();

            mainCanvas.Background = new SolidColorBrush(Colors.Gray);
            PauseOverlay.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.setManipulating(true);
        }

        public void Pause()
        {
            isPaused = true;
            Processing.Audio.Pause();

            mainCanvas.Background = new SolidColorBrush(Colors.Gray);
            PauseOverlay.Visibility = System.Windows.Visibility.Visible;
            MainWindow.setManipulating(false);       
        }
        #endregion

        private void Media_Ended(object sender, RoutedEventArgs e)
        {
            myMediaElement.Position = TimeSpan.Zero;
            myMediaElement.LoadedBehavior = MediaState.Play;
        }

        public void debugDoNext()
        {
            Tutorial.doNext = true;
        }
    }
}

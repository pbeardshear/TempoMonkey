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
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Navigation;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Media.Animation;
using System.IO;
using System.Drawing;
using System.Windows.Threading;
using slidingMenu;
using Visualizer;

namespace TempoMonkey
{
	/// <summary>
	/// Interaction logic for FreeFormMode.xaml
	/// </summary>
    public partial class FreeFormMode : Page, KinectPage
	{
        KinectGesturePlayer freePlayer, freePlayer2, tutoree;
		bool _isPaused = false;
        ArrayList _nameList = new ArrayList();
        string _type;
        public mySlider VolumeSlider, PitchSlider, TempoSlider;
		public Spectrum Visualizer;

        public void initCommon()
        {
            PauseCircle.Stroke = System.Windows.Media.Brushes.White;
            PauseCircle.StrokeThickness = 8;

            waveFormContainers[0] = SongContainer0;
            waveFormContainers[1] = SongContainer1;
            waveFormContainers[2] = SongContainer2;
            waveFormContainers[0].Visibility = System.Windows.Visibility.Hidden;
            waveFormContainers[1].Visibility = System.Windows.Visibility.Hidden;
            waveFormContainers[2].Visibility = System.Windows.Visibility.Hidden;
            SongTitles[0] = SongTitle0;
            SongTitles[1] = SongTitle1;
            SongTitles[2] = SongTitle2;
            SongTitles[0].Text = null;
            SongTitles[1].Text = null;
            SongTitles[2].Text = null;

			Visualizer = new Spectrum(mainCanvas);
			Visualizer.RegisterSoundPlayer();
            MainWindow.setManipulating(true);
        }

        public void initSliders()
        {
            VolumeSlider = new mySlider("Volume", 10, 100, 25, 250);
            PitchSlider = new mySlider("Pitch", 0, 100, 50, 250);
            TempoSlider = new mySlider("Tempo", 40, 200, 100, 250);

            Canvas.SetLeft(VolumeSlider, 240);
            Canvas.SetLeft(PitchSlider, 513);
            Canvas.SetLeft(TempoSlider, 792);

            Canvas.SetTop(VolumeSlider, 256);
            Canvas.SetTop(PitchSlider, 256);
            Canvas.SetTop(TempoSlider, 256);

            
            mainCanvas.Children.Add(VolumeSlider);
            mainCanvas.Children.Add(PitchSlider);
            mainCanvas.Children.Add(TempoSlider);
            
        }

        BrushConverter bc = new BrushConverter();
        TextBlock[] SongTitles = new TextBlock[3];
        Panel[] waveFormContainers = new Panel[3];
        int[] positions = { 260, 525, 850 };
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
            }
            get
            {
                return _currentTrackIndex;
            }
        }
        
        public void initWaveForm(Panel waveFormContainer, string uri,
            Visualizer.Timeline.WaveformTimeline.CompletionCallback callback = null)
        {
            Visualizer.Timeline.WaveformTimeline wave = new Visualizer.Timeline.WaveformTimeline(waveFormContainer, uri, callback);
            wave.Draw();
			// This might be more appropriate to call after Audio.Play() is called, but currently there is no
			// reference to these wave objects.  Tracking checks if the Audio is playing, so it shouldn't break.
			wave.StartTracking();
        }

        public void initBuddyForm(string address, string name)
        {
            _type = "Buddy"; 
            initCommon();

            // Load and set the song titles
            SongTitles[1].Text = name;
            Processing.Audio.LoadFile(address);

            // NavigationService.Navigate(MainWindow.freeFormPage);
            // Initlaize the wave form
            initWaveForm(waveFormContainers[1], address, delegate()
            {
                MainWindow.currentPage = MainWindow.freeFormPage;
                MainWindow.loadingPage.NavigationService.Navigate(MainWindow.currentPage);

                // Sets the current track & also plays it
                currentTrackIndex = 1;
                Processing.Audio.Play(0);
            });
            _nameList.Add(address);

            // connected to gestures
            freePlayer = new KinectGesturePlayer();
            freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, pauseChangeHandler);
            freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            freePlayer.registerCallBack(freePlayer.leanListener, tempoTrackingHandler, tempoChangeHandler);
            freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);

            freePlayer2 = new KinectGesturePlayer();
            freePlayer2.registerCallBack(freePlayer2.kinectGuideListener, pauseTrackingHandler, pauseChangeHandler);
            freePlayer2.registerCallBack(freePlayer2.handsAboveHeadListener, pitchTrackingHandler2, pitchChangeHandler);
            freePlayer2.registerCallBack(freePlayer2.leanListener, tempoTrackingHandler2, tempoChangeHandler);
            freePlayer2.registerCallBack(freePlayer2.handsWidenListener, volumeTrackingHandler2, volumeChangeHandler);
        }

        public void initSoloForm(ArrayList addrList, ArrayList nameList){
            _type = "Solo"; 
            initCommon();
            int doneCount = 0;
            // Load and set the song titles
            for( int i=0; i < addrList.Count; i++)
			{
               
                string address = addrList[i] as String;
                string name = nameList[i] as String;
                _nameList.Add(name);
                SongTitles[i].Text = name;
                Processing.Audio.LoadFile(address);

                // Initlaize the wave form
                initWaveForm(waveFormContainers[i], address, delegate()
                {
                    doneCount++;
                    if (doneCount == _nameList.Count)
                    {
                        MainWindow.currentPage = MainWindow.freeFormPage;
                        MainWindow.loadingPage.NavigationService.Navigate(MainWindow.currentPage);

                        // Sets the current track & also plays it
                        currentTrackIndex = _nameList.Count > 1 ? 1 : 0;
                        Processing.Audio.Play(currentTrackIndex);
                    }
                });
			}

            // connected to gestures
            freePlayer = new KinectGesturePlayer();
			freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, pauseChangeHandler);           
			freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
			freePlayer.registerCallBack(freePlayer.leanListener, tempoTrackingHandler, tempoChangeHandler);
			freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
            freePlayer.registerCallBack(freePlayer.trackMoveListener, changeTrackTrackingHandler, null);
        }

        public void initTutor(int index)
        {
            _type = "Tutor";
            initCommon();
            List<string> nameList = new List<string> { "Chasing Pavements", "Enough To Fly With You" };
            ArrayList addrList = new ArrayList { @"..\..\Resources\Music\Chasing Pavements.mp3", @"..\..\Resources\Music\Enough To Fly With You.mp3" };
            int doneCount = 0;

            myMediaElement.Visibility = System.Windows.Visibility.Visible;
            Instructions.Visibility = System.Windows.Visibility.Visible;
            Facts.Visibility = System.Windows.Visibility.Visible;

            NextTutorial.Visibility = System.Windows.Visibility.Visible;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(2);
            /*
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
             * */

            // Load and set the song titles
            for (int i = 0; i < addrList.Count; i++)
            {
                string address = addrList[i] as String;
                string name = nameList[i] as String;
                _nameList.Add(name);
                SongTitles[i].Text = name;
                Processing.Audio.LoadFile(address);

                // Initlaize the wave form
                initWaveForm(waveFormContainers[i], address, delegate()
                {
                    doneCount++;
                    if (doneCount == _nameList.Count)
                    {
                        MainWindow.currentPage = MainWindow.freeFormPage;
                        MainWindow.loadingPage.NavigationService.Navigate(MainWindow.currentPage);

                        // Sets the current track & also plays it
                        currentTrackIndex = _nameList.Count > 1 ? 1 : 0;
                        Processing.Audio.Play(currentTrackIndex);
                        Timer.Start();

                        Tutorial.TutorialIndex = index;
                        playTutorial(Tutorial.getCurrentTutorial());
                    }
                });
            }


            tutoree = new KinectGesturePlayer();
            tutoree.registerCallBack(tutoree.kinectGuideListener, pauseTrackingHandler, pauseChangeHandler);
            tutoree.registerCallBack(tutoree.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            tutoree.registerCallBack(tutoree.handSwingListener, seekTrackingHandler, seekChangeHandler);
            tutoree.registerCallBack(tutoree.leanListener, tempoTrackingHandler, tempoChangeHandler);
            tutoree.registerCallBack(tutoree.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
            tutoree.registerCallBack(tutoree.trackMoveListener, changeTrackTrackingHandler, volumeChangeHandler);
        }

        public void tearDown()
        {
            freePlayer = null;
            freePlayer2 = null;
            tutoree = null;

            myMediaElement.Visibility = System.Windows.Visibility.Visible;
            Instructions.Visibility = System.Windows.Visibility.Visible;
            Facts.Visibility = System.Windows.Visibility.Visible;
            NextTutorial.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.setManipulating(false);
			Processing.Audio.End();
        }

        NavigationButton quitButton, resumeButton, tutorialsButton, homeButton;
		public FreeFormMode()
		{
			InitializeComponent();
            InitializeAvatars();

            initSliders();
            quitButton = new NavigationButton(QuitButton, delegate()
            {
                tearDown();
                return MainWindow.homePage;
            });


            homeButton = new NavigationButton(HomeButton, delegate()
            {
                tearDown();
                return MainWindow.homePage;
            });


            resumeButton = new NavigationButton(ResumeButton, delegate()
            {
                Resume();
                return null;
            });

            tutorialsButton = new NavigationButton(TutorialsButton, delegate()
            {
                tearDown();
                return MainWindow.browseTutorialsPage;
            });

            new NavigationButton(NextTutorial, delegate()
            {
                Tutorial.nextTutorial();
                _isPaused = false;
                Processing.Audio.Resume();
                // NextOverLay.Visibility = System.Windows.Visibility.Hidden;
                PauseOverlay.Visibility = System.Windows.Visibility.Hidden;
                MainWindow.setManipulating(true);
                mainCanvas.Background = new SolidColorBrush(Colors.Black);
                playTutorial(Tutorial.getCurrentTutorial());
                return null;
            });

            // Tutor stuff
            string tutorials_base = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tutorials\\";
            pause = new Tutorial("Pause", "To pause move your left arm to a 45 degree angle with your body",
                    new Uri(tutorials_base + "00pause.m4v"), pauseChecker, null);
            tempo = new Tutorial("Changing the Tempo", "To increase the tempo lean towards the right, to decrease the tempo lean towards the left",
                    new Uri(tutorials_base + "01tempo.m4v"), tempoChecker, "Tempo determins the speed of a song");
            pitch = new Tutorial("Changing the Pitch", "To increase/decrease the pitch put your arms above your head and move your body up/down",
                    new Uri(tutorials_base + "02pitch.m4v"), pitchChecker, "Pitch determines how low or high the sounds of a song becomes");
            volume = new Tutorial("Changing the Volume", "To change the volume put both your arms in the midsection of your body and expand/intract your hands",
                    new Uri(tutorials_base + "03volume.m4v"), volumeChecker);
            seek = new Tutorial("Changing the Position of the track", "To seek around the track put your right hand up and hover it left and right",
                    new Uri(tutorials_base + "04seek.m4v"), seekChecker);
            switch_tracks = new Tutorial("Switching Tracks", "To switch between your tracks jump to the left/right",
                  new Uri(tutorials_base + "05swaptracks.m4v"), switchTrackChecker);

            Tutorial.addTutorial(pause);
            Tutorial.addTutorial(tempo);
            Tutorial.addTutorial(pitch);
            Tutorial.addTutorial(volume);
            Tutorial.addTutorial(seek);
            Tutorial.addTutorial(switch_tracks);
		}

        Tutorial seek, volume, switch_tracks, pitch, tempo, pause;

        public void allFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!_isPaused)
            {
                if (_type == "Solo")
                {
                    Skeleton skeleton = KinectGesturePlayer.getFristSkeleton(e);
                    if (skeleton != null)
                    {
                        freePlayer.skeletonReady(e, skeleton);
                    }
                }
                else if (_type == "Buddy")
                {
                    Skeleton[] skeletons = KinectGesturePlayer.getFirstTwoSkeletons(e);
                    Skeleton leftSkeleton;
                    Skeleton rightSkeleton;
                    if (skeletons == null)
                    {
                        return;
                    }
                    if (skeletons.Length == 0)
                    {
                        return;
                    }
                    else if (skeletons.Length == 1)
                    {
                        leftSkeleton = skeletons[0];
                        freePlayer.skeletonReady(e, leftSkeleton);
                    }
                    else
                    {
                        leftSkeleton = skeletons[0];
                        rightSkeleton = skeletons[1];
                        freePlayer.skeletonReady(e, leftSkeleton);
                        freePlayer2.skeletonReady(e, rightSkeleton);
                    }
                }
                else if (_type == "Tutor")
                {
                    Skeleton skeleton = KinectGesturePlayer.getFristSkeleton(e);
                    if (skeleton != null)
                    {
                        tutoree.skeletonReady(e, skeleton);
                    }
                }
            }
        }

		private Dictionary<string, BitmapImage> loadedImages = new Dictionary<string, BitmapImage>();
		private BitmapImage InitializeResource(Bitmap source, string name)
		{
			BitmapImage image = new BitmapImage();
			MemoryStream stream = new MemoryStream();
			source.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			image.BeginInit();
			image.StreamSource = new MemoryStream(stream.ToArray());
			image.EndInit();
			loadedImages.Add(name, image);
			return image;
		}

		public void InitializeAvatars()
		{
			// Active images
			InitializeResource(Properties.Resources.volume_avatar, "volumeAvatar");
			InitializeResource(Properties.Resources.seek_avatar, "seekAvatar");
			InitializeResource(Properties.Resources.pitch_avatar, "pitchAvatar");
			InitializeResource(Properties.Resources.tempo_avatar, "tempoAvatar");

			// Disabled images
			volumeAvatar.Source = InitializeResource(Properties.Resources.volume_avatar_disabled, "volumeAvatarDisabled");
			seekAvatar.Source = InitializeResource(Properties.Resources.seek_avatar_disabled, "seekAvatarDisabled");
			pitchAvatar.Source = InitializeResource(Properties.Resources.pitch_avatar_disabled, "pitchAvatarDisabled");
			tempoAvatar.Source = InitializeResource(Properties.Resources.tempo_avatar_disabled, "tempoAvatarDisabled");
		}

		public void SetAvatarState(bool active, System.Windows.Controls.Image imageControl, BitmapImage image)
		{
			imageControl.Source = image;
			imageControl.Height = active ? 75 : 50;
			imageControl.Width = active ? 75 : 50;
			Canvas.SetTop(imageControl, Canvas.GetTop(imageControl) + (active ? -12.5 : 12.5));
			Canvas.SetLeft(imageControl, Canvas.GetLeft(imageControl) + (active ? -12.5 : 12.5));
		}

		#region Gesture Handlers
		void pauseTrackingHandler(bool exist)
		{
            /*
            if ( _type == "Tutor" && Tutorial.getCurrentTutorial() == pause && exist)
            {
                donePause = true;
                return;
            }*/

			if (_isPaused)
			{
				return;
			}

			if (exist)
			{
				Pause();
			}
		}

        public void pauseChangeHandler(double angle)
        {
            if (angle > 0 && angle < 360)
            {
                PauseCircle.Visibility = System.Windows.Visibility.Visible;

                double x = Canvas.GetLeft(PauseLabel) + 45;
                double y = Canvas.GetTop(PauseLabel) + 45;

                System.Windows.Point center = new System.Windows.Point(x, y);

                System.Windows.Point endPoint = new System.Windows.Point(center.X + 40 * Math.Sin(angle / 180.0 * Math.PI), center.Y - 40 * Math.Cos(angle / 180.0 * Math.PI));

                PathFigure figure = new PathFigure();
                figure.StartPoint = new System.Windows.Point(center.X, center.Y - 40);

                figure.Segments.Add(new ArcSegment(
                    endPoint,
                    new System.Windows.Size(40, 40),
                    0,
                    angle >= 180,
                    SweepDirection.Clockwise,
                    true
                ));

                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);

                PauseCircle.Data = geometry;

            } else {
                PauseCircle.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        void changeTrackTrackingHandler(bool right)
        {
            if (right)
            {
                if (currentTrackIndex < _nameList.Count - 1)
                {
                    currentTrackIndex += 1;
                }
            }
            else
            {
                if (currentTrackIndex != 0)
                {
                    currentTrackIndex -= 1;
                }
            }
            if (_type == "Tutor" && Tutorial.getCurrentTutorial() == switch_tracks)
            {
                totalTrackChange++;
                if (totalTrackChange >= 3)
                {
                    doneSwitchTrack = true;
                }
            }
        }

        int totalTrackChange = 0;

		int _currentTrackIndex;

		void volumeChangeHandler(double change)
		{
			VolumeSlider.Value -= change;
			Processing.Audio.ChangeVolume(VolumeSlider.Value);

            if ( _type == "Tutor" && Tutorial.getCurrentTutorial() == volume)
            {
                totalVolumeChange += Math.Abs(change);
                if (totalVolumeChange > 100)
                {
                    doneVolume = true;
                }
            }
		}

        double totalVolumeChange = 0;

		void volumeTrackingHandler(bool exist)
		{
            VolumeSlider.player1Exists = exist;
			SetAvatarState(exist, volumeAvatar, exist ? loadedImages["volumeAvatar"] : loadedImages["volumeAvatarDisabled"]);
		}

        void volumeTrackingHandler2(bool exist)
        {
            VolumeSlider.player2Exists = exist;
        }

		void tempoChangeHandler(double change)
		{
			TempoSlider.Value += change / 2;
			Processing.Audio.ChangeTempo(TempoSlider.Value);

            if (_type == "Tutor" && Tutorial.getCurrentTutorial() == tempo)
            {
                totalTempoChange += Math.Abs(change);
                if (totalTempoChange > 150)
                {
                    doneTempo = true;
                }
            }
		}

        double totalTempoChange = 0;

		void tempoTrackingHandler(bool exist)
		{
            TempoSlider.player1Exists = exist;
			SetAvatarState(exist, tempoAvatar, exist ? loadedImages["tempoAvatar"] : loadedImages["tempoAvatarDisabled"]);
		}

        void tempoTrackingHandler2(bool exist)
        {
            TempoSlider.player2Exists = exist;
        }

        void pitchTrackingHandler(bool exist)
        {
            PitchSlider.player1Exists = exist; 
            SetAvatarState(exist, pitchAvatar, exist ? loadedImages["pitchAvatar"] : loadedImages["pitchAvatarDisabled"]);
        }

        void pitchTrackingHandler2(bool exist)
        {
            PitchSlider.player2Exists = exist; 
        }

		bool wasSeeking = false;
		void seekChangeHandler(double change)
		{
			//SeekSlider.Value += change; //UNCOMMENT THIS IF WE WANT TO SEEK
		}



		void seekTrackingHandler(bool exist)
		{
			if (exist)
			{
				wasSeeking = true;
			}
			else
			{
				if (wasSeeking)
				{
					//Processing.Audio.Seek(SeekSlider.Value); // UNCOMMENT THIS OUT IF WE WANT TO SEEK IN THE FUTURE
				}
				wasSeeking = false;
			}

			SetAvatarState(exist, seekAvatar, exist ? loadedImages["seekAvatar"] : loadedImages["seekAvatarDisabled"]);
		}

		void pitchChangeHandler(double change)
		{
			PitchSlider.Value -= change * 3;
			Processing.Audio.ChangePitch(PitchSlider.Value);

            if (_type == "Tutor" && Tutorial.getCurrentTutorial() == pitch)
            {
                totalPitchChange += Math.Abs(change);
                // Seek.Content = totalPitchChange;
                if (totalPitchChange > 50)
                {
                    donePitch = true;
                }
            }
		}

        double totalPitchChange = 0;

		public void Resume()
		{
			_isPaused = false;
            Processing.Audio.Resume();

            mainCanvas.Background = new SolidColorBrush(Colors.Black);
            PauseOverlay.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.setManipulating(true);

		}

        public void Pause()
        {
            _isPaused = true;
            Processing.Audio.Pause();

            mainCanvas.Background = new SolidColorBrush(Colors.Gray);
            PauseOverlay.Visibility = System.Windows.Visibility.Visible;
            MainWindow.setManipulating(false);
        }
		#endregion

        #region Tutor stuff

        private void Media_Ended(object sender, RoutedEventArgs e)
        {
            myMediaElement.Position = TimeSpan.Zero;
            myMediaElement.LoadedBehavior = MediaState.Play;
        }

        private Dictionary<Tutorial, check> _checkers = new Dictionary<Tutorial, check>();


        public void playTutorial(Tutorial tutorial)
        {
            myMediaElement.Source = tutorial.getSource();
            Instructions.Text = tutorial.getInstructions();
            Facts.Text = tutorial.getFacts();
            myMediaElement.Play();
        }

        DispatcherTimer Timer;

        public void showTutorialsFinished()
        {
            MainWindow.setManipulating(false);
            Timer.Stop();

        }

        public void showTutorialChooser(Tutorial tutorial)
        {
            MainWindow.setManipulating(false);
            mainCanvas.Background = new SolidColorBrush(Colors.Gray);
            NextOverLay.Visibility = System.Windows.Visibility.Visible;
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

        public delegate bool check();
        #endregion

    }
}

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
        KinectGesturePlayer freePlayer, freePlayer2;
		bool isPaused = false;
        ArrayList _nameList = new ArrayList();
        string _type;
        Spectrum spectrumVisualizer;
        box leftBox, midBox, rightBox;
        public mySlider VolumeSlider, PitchSlider, TempoSlider;

        // Number of bars, this should be an odd number
        public const int BarCount = 11;
        public Bar[] bars = new Bar[BarCount];
        // Distance between bars
        public const int BarDist = 60;

        public void InitBars()
        {
            Bar.canvas = mainCanvas;
            for (int i = 0, position = -(BarCount/2) * BarDist; i < BarCount/2; i++, position += BarDist)
            {
                bars[i] = new Bar(position);
                bars[BarCount - 1 - i] = new Bar(-position);
            }

            bars[ BarCount / 2] = new Bar(0);
        }

        public void changeBars()
        {
            // This is only for testing purposes only...
            // To see how changes affect things
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += (delegate(object s, EventArgs e)
            {
                for (int i = 0; i < 11; i++)
                {
                    bars[i].Height = (bars[i].Height + i * 20) % 188;
                }
            });
            Timer.Start();
        }

        public void initCommon()
        {
            initSliders();
            InitBars();
            changeBars();
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

        public void initBuddyForm(string address, string name)
        {
            initCommon();
            _type = "Buddy";
            Processing.Audio.LoadFile(address);
            _nameList.Add(name);
            currentTrackIndex = 0;
            System.Windows.Forms.Cursor.Hide();
            Processing.Audio.Play();
            //spectrumVisualizer = new Spectrum(mainCanvas);
            //spectrumVisualizer.RegisterSoundPlayer();

            Track.Content = _nameList[currentTrackIndex];

            freePlayer = new KinectGesturePlayer();
            freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
            freePlayer.registerCallBack(freePlayer.leanListener, tempoTrackingHandler, tempoChangeHandler);
            freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);

            freePlayer2 = new KinectGesturePlayer();
            freePlayer2.registerCallBack(freePlayer2.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
            freePlayer2.registerCallBack(freePlayer2.handsAboveHeadListener, pitchTrackingHandler2, pitchChangeHandler);
            freePlayer2.registerCallBack(freePlayer2.leanListener, tempoTrackingHandler2, tempoChangeHandler);
            freePlayer2.registerCallBack(freePlayer2.handsWidenListener, volumeTrackingHandler2, volumeChangeHandler);
        }

        public void initSoloForm(ArrayList addrList, ArrayList nameList){
            initCommon();
			// Load the audio files
            _type = "Solo";
			foreach (string uri in addrList)
			{
				Processing.Audio.LoadFile(uri);
			}

            foreach (string song in nameList)
            {
                _nameList.Add(song);
            }


            if (_nameList.Count > 1)
            {
                currentTrackIndex = 1;
            }
            else
            {
                currentTrackIndex = 0;
            }

			Processing.Audio.Play();
			System.Windows.Forms.Cursor.Hide();
			// Initialize the visualizer
			//spectrumVisualizer = new Spectrum(mainCanvas);
			//spectrumVisualizer.RegisterSoundPlayer();
            Track.Content = _nameList[currentTrackIndex];
            freePlayer = new KinectGesturePlayer();
			freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
			freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
			freePlayer.registerCallBack(freePlayer.leanListener, tempoTrackingHandler, tempoChangeHandler);
			freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);


            string path;
            int top = 400;
            if (currentTrackIndex > 0)
            {
                leftBox = new box(150);
                //mainCanvas.Children.Add(leftBox);
                path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Album_Art\\" + _nameList[currentTrackIndex - 1] + ".jpg";
                leftBox.setImage(path);
                Canvas.SetLeft(leftBox, 400 - 75 - 300);
                Canvas.SetTop(leftBox, top);
            }
            if (currentTrackIndex <= _nameList.Count - 2)
            {
                rightBox = new box(150);
                //mainCanvas.Children.Add(rightBox);
                path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Album_Art\\" + _nameList[currentTrackIndex + 1] + ".jpg";
                rightBox.setImage(path);
                Canvas.SetLeft(rightBox, 400 - 75);
                Canvas.SetTop(rightBox, top);
            }
            midBox = new box(150);
            //mainCanvas.Children.Add(midBox);
            path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Album_Art\\" + _nameList[currentTrackIndex] + ".jpg";
            midBox.setImage(path);
            Canvas.SetLeft(midBox, 400 - 75 + 300);
            Canvas.SetTop(midBox, top);

            updateSongs(0);
        }

        int offSet = 300;
        int offSetMid = 300;
        int pageWidth = 800;

        public void updateSongs(double value)
        {
            if (leftBox != null)
            {
                Canvas.SetLeft(leftBox, value + pageWidth / 2 + 300 - leftBox.Width / 2);
            }
            if (midBox != null)
            {
                Canvas.SetLeft(midBox, value + pageWidth / 2 - midBox.Width / 2);
            }
            if (rightBox != null)
            {
                Canvas.SetLeft(rightBox, value + pageWidth / 2 - 300 - rightBox.Width / 2);
            }
        }

        public void tearDown()
        {
            spectrumVisualizer = null;
            freePlayer = null;
            freePlayer2 = null;
            VolumeSlider = null;
            PitchSlider = null;
            TempoSlider = null;
            // TODO: TEARDOWN MUSIC... unload all files and whatever else that needs to be done
            // so that a user can navigate between pages that uses music
			Processing.Audio.End();
        }

        NavigationButton quitButton;
		public FreeFormMode()
		{		
			InitializeComponent();
            InitializeAvatars();

            quitButton = new NavigationButton(QuitButton, delegate()
            {
                tearDown();
                return MainWindow.homePage;
            });
		}

        public void allFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!isPaused)
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
			if (isPaused)
			{
				return;
			}

			if (exist)
			{
				Pause();
			}
		}


		int currentTrackIndex;

        int midPoint = 325;
        int span = 190;
		void changeTrackHandler(double value)
		{
            Seek.Content = value;
            updateSongs(value);
			if (!wasSeeking)
			{
				SeekSlider.Value += .05;
			}

			if (value < midPoint - span && currentTrackIndex != 0 && _nameList.Count > 1)
			{
				currentTrackIndex = 0;
                rightBox.unHighlightBox();
                midBox.unHighlightBox();
                leftBox.highlightBox();
			}
			else if (value > midPoint + span && currentTrackIndex != 2 && _nameList.Count > 2)
			{
				currentTrackIndex = 2;
                leftBox.unHighlightBox();
                midBox.unHighlightBox(); 
                rightBox.highlightBox();
			}
			else if (value >= midPoint - span && value <= midPoint + span && currentTrackIndex != 1)
			{
				currentTrackIndex = 1;
                leftBox.unHighlightBox();
                rightBox.unHighlightBox();
                midBox.highlightBox();
			}
			else
			{
				return;
			}

            Track.Content = _nameList[currentTrackIndex];
			Processing.Audio.SwapTrack(currentTrackIndex);
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
		}


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
			SeekSlider.Value += change;
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
					Processing.Audio.Seek(SeekSlider.Value);
				}
				wasSeeking = false;
			}

			SetAvatarState(exist, seekAvatar, exist ? loadedImages["seekAvatar"] : loadedImages["seekAvatarDisabled"]);
		}

		void pitchChangeHandler(double change)
		{
			PitchSlider.Value -= change * 3;
			Processing.Audio.ChangePitch(PitchSlider.Value);
		}

		public void Resume()
		{
			isPaused = false;
			Processing.Audio.Play();
            mainCanvas.Background = new SolidColorBrush(Colors.White);
			ResumeButton.Visibility = System.Windows.Visibility.Hidden;
			QuitButton.Visibility = System.Windows.Visibility.Hidden;
            MainWindow.setManipulating(true);
		}

		public void Pause()
		{
			isPaused = true;
            Processing.Audio.Pause();
            mainCanvas.Background = new SolidColorBrush(Colors.Gray);
			Border.Visibility = System.Windows.Visibility.Visible;
			ResumeButton.Visibility = System.Windows.Visibility.Visible;
			QuitButton.Visibility = System.Windows.Visibility.Visible;
            MainWindow.setManipulating(false);;
		}

		#endregion

		#region Navigation

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            Resume();
        }

		#endregion



    }
}

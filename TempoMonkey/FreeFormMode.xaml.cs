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
using Visualizer;

namespace TempoMonkey
{
	/// <summary>
	/// Interaction logic for FreeFormMode.xaml
	/// </summary>
    public partial class FreeFormMode : Page, KinectPage
	{
		BrushConverter bc = new BrushConverter();
        KinectGesturePlayer freePlayer, freePlayer2;
		bool isPaused = false;
        ArrayList _nameList = new ArrayList();
        string _type;
        Spectrum spectrumVisualizer;

        public void initBuddyForm(string address, string name)
        {
            _type = "Buddy";
            Processing.Audio.LoadFile(address);
            _nameList.Add(name);
            currentTrackIndex = 0;
            System.Windows.Forms.Cursor.Hide();
            Processing.Audio.Play();

            Spectrum spectrumVisualizer = new Spectrum(mainCanvas);
            spectrumVisualizer.RegisterSoundPlayer();
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
			spectrumVisualizer = new Spectrum(mainCanvas);
			spectrumVisualizer.RegisterSoundPlayer();
            Track.Content = _nameList[currentTrackIndex];
            freePlayer = new KinectGesturePlayer();
			freePlayer.registerCallBack(freePlayer.kinectGuideListener, pauseTrackingHandler, changeTrackHandler);
			freePlayer.registerCallBack(freePlayer.handsAboveHeadListener, pitchTrackingHandler, pitchChangeHandler);
			freePlayer.registerCallBack(freePlayer.leanListener, tempoTrackingHandler, tempoChangeHandler);
			freePlayer.registerCallBack(freePlayer.handsWidenListener, volumeTrackingHandler, volumeChangeHandler);
        }

        public void tearDown()
        {
            spectrumVisualizer = null;
            freePlayer = null;
            freePlayer2 = null;
            // TODO: TEARDOWN MUSIC... unload all files and whatever else that needs to be done
            // so that a user can navigate between pages that uses music
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
		void changeTrackHandler(double value)
		{
			if (!wasSeeking)
			{
				SeekSlider.Value += .05;
			}

			if (value < 250 && currentTrackIndex != 1 && _nameList.Count > 1)
			{
				currentTrackIndex = 1;
			}
			else if (value > 450 && currentTrackIndex != 2 && _nameList.Count > 2)
			{
				currentTrackIndex = 2;
			}
			else if (value >= 250 && value <= 450 && currentTrackIndex != 0)
			{
				currentTrackIndex = 0;
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
			Volume.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            VolumeFocus.BorderBrush = bc.ConvertFromString("Green") as System.Windows.Media.Brush;
			VolumeFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            //Throws a KeyError for some reason
			SetAvatarState(exist, volumeAvatar, exist ? loadedImages["volumeAvatar"] : loadedImages["volumeAvatarDisabled"]);
		}


        void volumeTrackingHandler2(bool exist)
        {
            Volume.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            if (exist)
            {
                VolumeFocus.BorderBrush = bc.ConvertFromString("Blue") as System.Windows.Media.Brush;
                VolumeFocus.Visibility = System.Windows.Visibility.Visible;
            }
        }

		void tempoChangeHandler(double change)
		{
			TempoSlider.Value += change / 2;
			Processing.Audio.ChangeTempo(TempoSlider.Value);
		}


		void tempoTrackingHandler(bool exist)
		{
			Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            TempoFocus.BorderBrush = bc.ConvertFromString("Green") as System.Windows.Media.Brush;
			TempoFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
			SetAvatarState(exist, tempoAvatar, exist ? loadedImages["tempoAvatar"] : loadedImages["tempoAvatarDisabled"]);
		}


        void tempoTrackingHandler2(bool exist)
        {
            Tempo.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            if (exist)
            {
                TempoFocus.BorderBrush = bc.ConvertFromString("Blue") as System.Windows.Media.Brush;
                TempoFocus.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void pitchTrackingHandler2(bool exist)
        {
            Pitch.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
            if (exist)
            {
                PitchFocus.BorderBrush = bc.ConvertFromString("Blue") as System.Windows.Media.Brush;
                PitchFocus.Visibility = System.Windows.Visibility.Visible;
            }
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

		void pitchTrackingHandler(bool exist)
		{
			Pitch.FontStyle = exist ? FontStyles.Oblique : FontStyles.Normal;
			PitchFocus.Visibility = exist ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            SetAvatarState(exist, pitchAvatar, exist ? loadedImages["pitchAvatar"] : loadedImages["pitchAvatarDisabled"]);
		}

		public void Resume()
		{
			isPaused = false;
			Processing.Audio.Play();
			Border.Visibility = System.Windows.Visibility.Hidden;
			ResumeButton.Visibility = System.Windows.Visibility.Hidden;
			QuitButton.Visibility = System.Windows.Visibility.Hidden;
			MainWindow.isManipulating = true;
		}

		public void Pause()
		{
			isPaused = true;
            Processing.Audio.Pause();
			Border.Visibility = System.Windows.Visibility.Visible;
			ResumeButton.Visibility = System.Windows.Visibility.Visible;
			QuitButton.Visibility = System.Windows.Visibility.Visible;
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

        void Resume_Click(object sender, RoutedEventArgs e)
		{
			Resume();
		}

		#endregion

	}
}

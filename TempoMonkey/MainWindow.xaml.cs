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
using Processing;
using System.Diagnostics;
using BigMansStuff.PracticeSharp.Core;
using BigMansStuff.PracticeSharp.UI;
using BigMansStuff.PracticeSharp.Properties;
using System.Threading;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			// Processing.Audio.LoadFile("test.mp3");
        }

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			// Processing.Program.Reset();
			Processing.Audio.Reset();
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			Processing.Audio.Initialize();
			// Initialize Time Stretch Profiles (required for changing tempo)
			TimeStretchProfileManager.Initialize();

			int defaultProfileIndex = 0;
			foreach (TimeStretchProfile timeStretchProfile in TimeStretchProfileManager.TimeStretchProfiles.Values)
			{
				int itemIndex = timeStretchProfileComboBox.Items.Add(timeStretchProfile);
				if (timeStretchProfile == TimeStretchProfileManager.DefaultProfile)
				{
					defaultProfileIndex = itemIndex;
				}
			}

			// Select default profile
			timeStretchProfileComboBox.SelectedIndex = defaultProfileIndex;
			// Processing.Program.Mp3ToWav("test.mp3", "test.wav");
			Processing.Audio.LoadFile("test.mp3");
			// Processing.Audio.LoadFile("test2.mp3");
		}

		private void button3_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Processing.Audio.Play();
			}
			catch (Exception processingRunException)
			{
				ErrorText.Text = processingRunException.Message;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Processing.Audio.Cleanup();
		}

		private void button4_Click(object sender, RoutedEventArgs e)
		{
			Processing.Audio.Pause();
		}

		// Swap
		string current = "test.mp3";
		private void button5_Click(object sender, RoutedEventArgs e)
		{
			if (current == "test.mp3")
			{
				Processing.Audio.SwapTrack("test2.mp3");
				current = "test2.mp3";
			}
			else
			{
				Processing.Audio.SwapTrack("test.mp3");
				current = "test.mp3";
			}
		}

		private void Tempo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Processing.Audio.isInitialized)
			{
				Processing.Audio.ChangeTempo(Tempo.Value);
			}
		}

		private void Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Processing.Audio.isInitialized)
			{
				Processing.Audio.ChangePitch(Pitch.Value);
			}
		}

		/*
		 * 
		 *	Code taken from PracticeSharpLibrary
		 * 
		 * 
		 */
		
    }
}

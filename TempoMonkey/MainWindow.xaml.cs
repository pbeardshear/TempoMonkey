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
using Processing;

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
			// Processing.Program.Mp3ToWav("test.mp3", "test.wav");
			Processing.Audio.LoadFile("test.mp3");
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
    }
}

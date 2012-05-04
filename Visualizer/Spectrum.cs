using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using NAudio;
using NAudio.Wave;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Processing;

namespace Visualizer
{
	public class Spectrum
	{
		private DispatcherTimer timer;
		private Canvas canv;
		private readonly List<Bar> barShapes = new List<Bar>();
		private readonly List<Bar> peakShapes = new List<Bar>();
		private int min;
		private int max = 2047;
		private int[] barIndexMax;
		private int[] barLogScaleIndexMax;
		private float[] channelPeakData;
		private double[] barHeights;
		private double[] peakHeights;
		private float[] channelData = new float[2048];
		private double bandWidth = 1.0;
		private double barWidth = 1;
		private int BarCount = 13;
		private double BarSpacing = 65.0;
		private int maxBarHeight = 160;
		private const int scaleFactorLinear = 15;
		private Random rand = new Random();
		private byte[] MainColors = new byte[3];
		private byte[] SideColors = new byte[3];

		private int colorUpdate = 0;
		private int intervalSize = 100;
		private int energyOffset = 50;
		private const double stepSize = 100.0 / 6;
		private double colorIntervalPosition = 0;
		private Dictionary<double, SolidColorBrush> ColorMap = new Dictionary<double, SolidColorBrush>()
		{
				  {stepSize * 0, Brushes.Purple},
				  {stepSize * 1, Brushes.Cyan},
				  {stepSize * 2, Brushes.GreenYellow},
				  {stepSize * 3, Brushes.Yellow},
				  {stepSize * 4, Brushes.Orange},
				  {stepSize * 5, Brushes.DarkRed},
				  {stepSize * 6, Brushes.White}
		};

		public float currentTempo;
		public float currentPitch;
		public float currentVolume;

		public Spectrum(Canvas spectrumContainer)
		{
			this.canv = spectrumContainer;
			timer = new DispatcherTimer(DispatcherPriority.Background)
			{
				Interval = TimeSpan.FromMilliseconds(30),
			};
			timer.Tick += timerTick;
		}

		public void RegisterSoundPlayer()
		{
			UpdateBarLayout();
			timer.Start();
		}

		private void timerTick(object sender, EventArgs e)
		{
			// Check if the sound is playing, if not stop sampling
			if (Audio.IsPlaying)
			{
				// Get the spectrum data from the sampler
				Processing.Audio.InputSampler.GetFFT(channelData);
				Update();
			}
		}

		private void Update()
		{
			double barHeight = 0f;
			double lastPeakHeight = 0f;
			double peakYPos = 0f;
			double PeakFallDelay = 30;
			int barIndex = 0;
			double fftBucketHeight = 0f;
			double height = canv.RenderSize.Height - 20;		// This places the visualizer at the bottom of the screen
			double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
			double barHeightScale = (height - peakDotHeight);

			currentTempo = Audio.getTempo();
			currentVolume = Audio.geVolume();
			if (Audio.getPitch() != currentPitch)
			{
				UpdatePitch(Audio.getPitch());
			}
			for (int i = min; i <= max; i++)
			{
				fftBucketHeight = (channelData[i] * scaleFactorLinear) * barHeightScale;
				if (barHeight < fftBucketHeight)
					barHeight = fftBucketHeight;
				if (barHeight < 0f)
					barHeight = 0f;

				int currentIndexMax = barLogScaleIndexMax[barIndex];
				if (i == currentIndexMax)
				{
					if (barHeight > height)
						barHeight = height;
					if (barIndex > 0)
						barHeight = (lastPeakHeight + barHeight) / 1.3;// / (rand.NextDouble() < 0.3 ? 1.2 : 1.8);
					// Apply the current volume to the bar height
					barHeight += currentVolume / 5;
					// Apply the tempo to the volume
					// Depending on the value, we add a random offset to the height
					// barHeight += rand.Next(0, (int)(currentTempo / 4));
					peakYPos = barHeight;
					if (channelPeakData[barIndex] < peakYPos) 
					{
						channelPeakData[barIndex] = (float)peakYPos;
					}
					else
					{
						channelPeakData[barIndex] = (float)(peakYPos + (PeakFallDelay * channelPeakData[barIndex])) / ((float)(PeakFallDelay + 1));
					}

					double xCoord = BarSpacing + (barWidth * barIndex) + (BarSpacing * barIndex) + 1;
					
					// barShapes[barIndex].Margin = new Thickness(xCoord, (height - 1) - Math.Min(barHeight, maxBarHeight), 0, 0);
					barShapes[barIndex].Height = Math.Min(barHeight / 2, maxBarHeight);
					// Color only changes once every few updates
					// This is to prevent flickering
					colorUpdate += 1;
					if (colorUpdate >= 8)
					{
						// Set lightness based on energy
						double energy = (barShapes[barIndex].Height / maxBarHeight);
						double value = currentPitch + ((energy * intervalSize) - energyOffset);
						SolidColorBrush fill = ColorMap[Math.Max(Math.Min(value - (value % stepSize), 100), 0)];
						//barShapes[barIndex].BarFill = new SolidColorBrush(Color.FromArgb((byte)200, fill.Color.R, fill.Color.G, fill.Color.B));
						//barShapes[barIndex].sideFill = new SolidColorBrush(Color.FromArgb((byte)200, fill.Color.R, fill.Color.G, fill.Color.B)); //new SolidColorBrush(Color.FromArgb((byte)(255 * (barShapes[barIndex].Height / maxBarHeight)), red, green, blue));
						barShapes[barIndex].BarFill = fill;
						barShapes[barIndex].sideFill = fill;
						barShapes[barIndex].Opacity = Math.Min(Math.Max(energy, 0.4), 0.8);
						colorUpdate = 0;
					}
					lastPeakHeight = barHeight;
					barHeight = 0f;
					barIndex++;
				}
			}
		}

		/*
		 * This is the update method I was talking about
		 * where at the end all of the shapes are added to canvas.
		 */
		private void UpdateBarLayout()
		{
			double ActualBarWidth = 0.0d;
			int MaximumFrequency = 20000;
			int MinimumFrequency = 20;
			int fftDataSize = (int)FFTDataSize.FFT2048;
			int maxFrequency = (int)((MaximumFrequency / 22050.0) * fftDataSize / 2);
			int minFrequency = (int)((MinimumFrequency / 22050.0) * fftDataSize / 2);
			barWidth = Math.Max(((double)(canv.RenderSize.Width - (BarSpacing * (BarCount + 1))) / (double)BarCount), 1);
			max = Math.Min(maxFrequency + 1, 2047);
			min = Math.Min(minFrequency, 2047);
			bandWidth = Math.Max(((double)(max - min)) / canv.RenderSize.Width, 1.0);

			int actualBarCount;
			if (barWidth >= 1.0d)
				actualBarCount = BarCount;
			else
				actualBarCount = Math.Max((int)((canv.RenderSize.Width - BarSpacing) / (barWidth + BarSpacing)), 1);
			channelPeakData = new float[actualBarCount];

			int indexCount = max - min;
			int linearIndexBucketSize = (int)Math.Round((double)indexCount / (double)actualBarCount, 0);
			List<int> maxIndexList = new List<int>();
			List<int> maxLogScaleIndexList = new List<int>();
			double maxLog = Math.Log(actualBarCount, actualBarCount);
			for (int i = 1; i < actualBarCount; i++)
			{
				maxIndexList.Add(min + (i * linearIndexBucketSize));
				int logIndex = (int)((maxLog - Math.Log((actualBarCount + 1) - i, (actualBarCount + 1))) * indexCount) + min;
				maxLogScaleIndexList.Add(logIndex);
			}
			maxIndexList.Add(max);
			maxLogScaleIndexList.Add(max);
			barIndexMax = maxIndexList.ToArray();
			barLogScaleIndexMax = maxLogScaleIndexList.ToArray();

			barHeights = new double[actualBarCount];
			peakHeights = new double[actualBarCount];

			// canv.Children.Clear();
			barShapes.Clear();
			peakShapes.Clear();

			Bar.canvas = canv;

			SolidColorBrush MainBrush = Brushes.Blue;
			SolidColorBrush SideBrush = Brushes.DarkBlue;
			// Number of rectangles to each side of the center bar
			int sideCount = (int)Math.Floor(actualBarCount / 2.0);
			for (int i = 0; i < sideCount; i++)
			{
				double rightPos = (BarSpacing * sideCount) - (BarSpacing * i);
				double leftPos = (-BarSpacing * sideCount) + (BarSpacing * i);
				Bar rightRectangle = new Bar(rightPos, MainBrush, SideBrush, 50, 30);
				Bar leftRectangle = new Bar(leftPos, MainBrush, SideBrush, 50, 30);
				
				barShapes.Insert(0, rightRectangle);
				barShapes.Insert(0, leftRectangle);
			}

			// Center bar
			Bar centerBar = new Bar(0, MainBrush, SideBrush, 65, 30);
			barShapes.Insert(0, centerBar);

			// Set the color values
			Color MainColor = MainBrush.Color;
			Color SideColor = SideBrush.Color;
			MainColors[0] = MainColor.R;
			MainColors[1] = MainColor.G;
			MainColors[2] = MainColor.B;

			SideColors[0] = SideColor.R;
			SideColors[1] = SideColor.G;
			SideColors[2] = SideColor.B;
		}

		#region Update Style Handlers

		// Pitch affects color of bars
		public void UpdatePitch(float newValue)
		{
			// Color position goes from the interval [0, 410]
			currentPitch = newValue;
		}
		
		#endregion
	}
}

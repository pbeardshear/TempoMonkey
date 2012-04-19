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

namespace Visualizer
{
	public class Spectrum
	{
		private DispatcherTimer timer;
		private Canvas canv;
		private readonly List<Shape> barShapes = new List<Shape>();
		private readonly List<Shape> peakShapes = new List<Shape>();
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
		private int BarCount = 12;
		private double BarSpacing = 20.0;
		private int barIndex = 0;
		private const int scaleFactorLinear = 15;
		private Random rand = new Random();

		public Spectrum(Canvas spectrumContainer)
		{
			this.canv = spectrumContainer;
			timer = new DispatcherTimer(DispatcherPriority.Background)
			{
				Interval = TimeSpan.FromMilliseconds(40),
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
			// Get the spectrum data from the sampler
			Sampler.GetFFT(channelData);
			Update();
		}

		private void Update()
		{
			double barHeight = 0f;
			double lastPeakHeight = 0f;
			double peakYPos = 0f;
			double PeakFallDelay = 30;
			int barIndex = 0;
			double fftBucketHeight = 0f;
			double height = canv.RenderSize.Height;
			double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
			double barHeightScale = (height - peakDotHeight);
			for (int i = min; i <= max; i++)
			{
				fftBucketHeight = (channelData[i] * scaleFactorLinear) * barHeightScale;
				if (barHeight < fftBucketHeight)
					barHeight = fftBucketHeight;
				if (barHeight < 0f)
					barHeight = 0f;

				int currentIndexMax = false ? barIndexMax[barIndex] : barLogScaleIndexMax[barIndex];
				if (i == currentIndexMax)
				{
					if (barHeight > height)
						barHeight = height;
					if (barIndex > 0)
						barHeight = (lastPeakHeight + barHeight) / (rand.NextDouble() < 0.3 ? 1.2 : 1.8);
					peakYPos = barHeight;
					if (channelPeakData[barIndex] < peakYPos)
						channelPeakData[barIndex] = (float)peakYPos;
					else
						channelPeakData[barIndex] = (float)(peakYPos + (PeakFallDelay * channelPeakData[barIndex])) / ((float)(PeakFallDelay + 1));

					double xCoord = BarSpacing + (barWidth * barIndex) + (BarSpacing * barIndex) + 1;
					
					barShapes[barIndex].Margin = new Thickness(xCoord, (height - 1) - barHeight, 0, 0);
					barShapes[barIndex].Height = barHeight;
					lastPeakHeight = barHeight;
					barHeight = 0f;
					barIndex++;
				}
			}
		}

		private float RandomNumber()
		{
			Random random = new Random();
			return (float)(random.Next(0, 2047) * random.NextDouble());
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

			double height = canv.RenderSize.Height;
			double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
			for (int i = 0; i < actualBarCount; i++)
			{
				double xCoord = BarSpacing + (barWidth * i) + (BarSpacing * i) + 1;

				Rectangle barRectangle = new Rectangle()
				{
					Margin = new Thickness(xCoord, height, 0, 0),
					Width = 15,
					Fill = new LinearGradientBrush(Brushes.DarkSlateBlue.Color, Brushes.SlateBlue.Color, new Point(0.5, 0), new Point(0.5, 1)),
					Stroke = Brushes.Black,
					StrokeThickness = 3,
					StrokeLineJoin = PenLineJoin.Round
				};
				barShapes.Add(barRectangle);
				//Rectangle peakRectangle = new Rectangle()
				//{
				//    Margin = new Thickness(xCoord, height - peakDotHeight, 0, 0),
				//    Width = barWidth,
				//    Height = peakDotHeight,
				//};
				//peakShapes.Add(peakRectangle);
			}

			foreach (Shape shape in barShapes)
				canv.Children.Add(shape);
			foreach (Shape shape in peakShapes)
				canv.Children.Add(shape);

			ActualBarWidth = barWidth;
		}
	}
}

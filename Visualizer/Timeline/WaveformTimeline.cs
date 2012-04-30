using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using NAudio.Wave;
using Processing;
using System.Windows.Shapes;

namespace Visualizer.Timeline
{
	public class WaveformTimeline
	{
		#region Private variables
		private Panel Container;
		private Brush WaveformFill = Brushes.RoyalBlue;
		private BackgroundWorker worker = new BackgroundWorker();
		private CompletionCallback OnCompletion;

		private string FileName;
		private float[] WaveformData;
		private float[] FullLevelData;
		#endregion

		public delegate void CompletionCallback();

		public WaveformTimeline(Panel container, string fileName, CompletionCallback callback = null)
		{
			Container = container;
			FileName = fileName;
			OnCompletion = callback;
		}

		#region Public Methods
		
		/// <summary>
		/// Change the fill color of the path
		/// </summary>
		/// <param name="fill">Brush color to change to</param>
		public void SetFill(Brush fill)
		{
			WaveformFill = fill;
		}

		public void Draw()
		{
			worker.DoWork += new DoWorkEventHandler(worker_DoWork);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
			worker.WorkerSupportsCancellation = true;

			// Start the background worker
			worker.RunWorkerAsync();
		}

		#endregion



		#region Private Helpers

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Mp3FileReader reader = new Mp3FileReader(FileName);
			WaveChannel32 channel = new WaveChannel32(reader);
			channel.Sample += new EventHandler<SampleEventArgs>(channel_Sample);

			int points = 2000;

			int frameLength = (int)FFTDataSize.FFT2048;
			int frameCount = (int)((double)channel.Length / (double)frameLength);
			int waveformLength = frameCount * 2;
			byte[] readBuffer = new byte[frameLength];

			float maxLeftPointLevel = float.MinValue;
			float maxRightPointLevel = float.MinValue;
			int currentPointIndex = 0;
			float[] waveformCompressedPoints = new float[points];
			List<float> waveformData = new List<float>();
			List<int> waveMaxPointIndexes = new List<int>();

			for (int i = 1; i <= points; i++)
			{
				waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)points), 0));
			}
			int readCount = 0;
			while (currentPointIndex * 2 < points)
			{
				channel.Read(readBuffer, 0, readBuffer.Length);

				waveformData.Add(Sampler.LeftMax);
				waveformData.Add(Sampler.RightMax);

				if (Sampler.LeftMax > maxLeftPointLevel)
					maxLeftPointLevel = Sampler.LeftMax;
				if (Sampler.RightMax > maxRightPointLevel)
					maxRightPointLevel = Sampler.RightMax;

				if (readCount > waveMaxPointIndexes[currentPointIndex])
				{
					waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
					waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
					maxLeftPointLevel = float.MinValue;
					maxRightPointLevel = float.MinValue;
					currentPointIndex++;
				}
				if (readCount % 3000 == 0)
				{
					WaveformData = (float[])waveformCompressedPoints.Clone();
				}

				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				readCount++;
			}

			FullLevelData = waveformData.ToArray();
			WaveformData = (float[])waveformCompressedPoints.Clone();

			// Cleanup
			channel.Close();
			channel.Dispose();
			channel = null;
			reader.Close();
			reader.Dispose();
			reader = null;	
		}

		private void channel_Sample(object sender, SampleEventArgs e)
		{
			Sampler.Add(e.Left, e.Right);
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// Draw the path object into the container
			PathFigure figure = new PathFigure();
			figure.StartPoint = new System.Windows.Point(40, 400);
			double thickness = 1000.0 / WaveformData.Length;
			double x = 0;

			PolyLineSegment leftSegment = new PolyLineSegment();
			PolyLineSegment rightSegment = new PolyLineSegment();
			leftSegment.Points.Add(new System.Windows.Point(40, 400));
			rightSegment.Points.Add(new System.Windows.Point(40, 400));
			for (int i = 0; i < WaveformData.Length; i += 2)
			{
				x = (i / 2) * thickness;
				leftSegment.Points.Add(new System.Windows.Point(40 + x, 400 + (WaveformData[i] * 25)));
				rightSegment.Points.Add(new System.Windows.Point(40 + x, 400 - (WaveformData[i] * 25)));
			}
			figure.Segments.Add(leftSegment);
			figure.Segments.Add(rightSegment);

			PathGeometry geometry = new PathGeometry();
			geometry.Figures.Add(figure);

			Path path = new Path();
			path.Fill = WaveformFill;
			path.Data = geometry;

			Container.Children.Add(path);

			// Alert any callbacks that were registered
			if (OnCompletion != null)
			{
				OnCompletion();
			}
		}

		#endregion
	}
}

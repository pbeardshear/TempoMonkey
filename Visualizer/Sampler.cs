using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Dsp;

namespace Visualizer
{
	public class Sampler
	{
		#region Private Variables
		private static Complex[] channelData;
		private static int channelPosition;
		private static int binaryExponentiation;
		private static int bufferSize;
		#endregion

		#region Initialization

		/// <summary>
		/// Initialize the Sampler.  Call before beginning to add any samples.
		/// </summary>
		public static void Initialize(int _bufferSize)
		{
			bufferSize = _bufferSize;
			channelPosition = 0;
			binaryExponentiation = (int)Math.Log(bufferSize, 2);
			channelData = new Complex[bufferSize];
		}
		#endregion


		/// <summary>
		/// Add a sample to be processed
		/// </summary>
		/// <param name="leftValue"></param>
		/// <param name="rightValue"></param>
		public static void Add(float leftValue, float rightValue)
		{
			channelData[channelPosition].X = (leftValue + rightValue) / 2.0f;	// Real
			channelData[channelPosition].Y = 0.0f;	// Imaginary
			channelPosition++;

			// Reset the channel position if we are over the length
			// Alternatively, we could just stop sampling and not repeat
			if (channelPosition >= channelData.Length)
			{
				channelPosition = 0;
			}
		}

		/// <summary>
		/// Fills the provided buffer with FFT data taken from current channel data
		/// </summary>
		/// <param name="buffer"></param>
		public static void GetFFT(float[] buffer)
		{
			Complex[] channelClone = new Complex[bufferSize];
			channelData.CopyTo(channelClone, 0);
			FastFourierTransform.FFT(true, binaryExponentiation, channelClone);
			for (int i = 0; i < channelClone.Length / 2; i++)
			{
				buffer[i] = (float)Math.Sqrt(channelClone[i].X * channelClone[i].X + channelClone[i].Y * channelClone[i].Y);
			}
		}

		public static void StartSampling()
		{
			throw new NotImplementedException();
		}

		public static void StopSampling()
		{
			throw new NotImplementedException();
		}
	}
}

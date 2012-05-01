using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Dsp;

namespace Processing
{
	public class Sampler
	{
		#region Private Variables
		private Complex[] channelData;
		private int channelPosition;
		private int binaryExponentiation;
		private int bufferSize;
		#endregion

		#region Public Accessors
		public float LeftMax = float.MinValue;
		public float LeftMin = float.MaxValue;
		public float RightMax = float.MinValue;
		public float RightMin = float.MaxValue;
		#endregion

		#region Initialization

		/// <summary>
		/// Initialize the Sampler.  Call before beginning to add any samples.
		/// </summary>
		public Sampler(int _bufferSize)
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
		public void Add(float leftValue, float rightValue)
		{
			if (channelPosition == 0)
			{
				LeftMax = float.MinValue;
				RightMax = float.MinValue;
				LeftMin = float.MaxValue;
				RightMin = float.MaxValue;
			}

			channelData[channelPosition].X = (leftValue + rightValue) / 2.0f;	// Real
			channelData[channelPosition].Y = 0.0f;	// Imaginary
			channelPosition++;

			LeftMax = Math.Max(LeftMax, leftValue);
			LeftMin = Math.Min(LeftMin, leftValue);
			RightMax = Math.Max(RightMax, rightValue);
			RightMin = Math.Min(RightMin, rightValue);

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
		public void GetFFT(float[] buffer)
		{
			Complex[] channelClone = new Complex[bufferSize];
			channelData.CopyTo(channelClone, 0);
			FastFourierTransform.FFT(true, binaryExponentiation, channelClone);
			for (int i = 0; i < channelClone.Length / 2; i++)
			{
				buffer[i] = (float)Math.Sqrt(channelClone[i].X * channelClone[i].X + channelClone[i].Y * channelClone[i].Y);
			}
		}

		public void StartSampling()
		{
			throw new NotImplementedException();
		}

		public void StopSampling()
		{
			throw new NotImplementedException();
		}
	}
}

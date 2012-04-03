using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace Processing
{
    public class Program
    {
        private static IWavePlayer waveOutDevice;
        private static WaveStream mainOutputStream;
        public static string fileName = "pacman_intro.wav";

		public static void Run()
		{
			try
			{
				waveOutDevice = new AsioOut();
			}
			catch (Exception driverCreateException)
			{
				throw driverCreateException;
			}

			mainOutputStream = CreateInputStream(fileName);

			try
			{
				waveOutDevice.Init(mainOutputStream);
			}
			catch (Exception initException)
			{
				throw initException;
			}
			waveOutDevice.Play();
		}

		public static void Reset()
		{
			mainOutputStream.CurrentTime = TimeSpan.FromSeconds(0);
		}

		private static WaveStream CreateInputStream(string fileName)
		{
			WaveChannel32 inputStream;
			if (fileName.EndsWith(".wav"))
			{
				WaveStream readerStream = new WaveFileReader(fileName);
				if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
					readerStream = new BlockAlignReductionStream(readerStream);
				}
				if (readerStream.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(readerStream.WaveFormat.SampleRate,
					   16, readerStream.WaveFormat.Channels);
					readerStream = new WaveFormatConversionStream(format, readerStream);
				}
				inputStream = new WaveChannel32(readerStream);
			}
			else
			{
				throw new InvalidOperationException("Unsupported extension");
			}
			return inputStream;
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.IO;
using System.Threading;

namespace Processing
{
    public class Program
    {
        private static IWavePlayer waveOutDevice;
        private static WaveStream mainOutputStream;
        public static string fileName = "test.wav";

		public static void Run()
		{
			/*
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
			 */

			// Try playing mp3 directly (probably simpler)
			using (FileStream fs = File.OpenRead("test.mp3"))
			{
				using (Mp3FileReader reader = new Mp3FileReader(fs))
				{
					using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(reader))
					{
						using (BlockAlignReductionStream blockStream = new BlockAlignReductionStream(waveStream))
						{
							using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
							{
								waveOut.Init(blockStream);
								waveOut.Play();
								//while (waveOut.PlaybackState == PlaybackState.Playing)
								//{
								//    Thread.Sleep(100);
								//}
							}
						}
					}
				}
			}

		}

		public static void Reset()
		{
			mainOutputStream.CurrentTime = TimeSpan.FromSeconds(0);
		}

		public static void Mp3ToWav(string inputFile, string outputFile)
		{
			using (Mp3FileReader reader = new Mp3FileReader(inputFile))
			{
				using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
				{
					WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
				}
			}
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace Processing
{
	/// <summary>
	/// Public interface for exposing audio manipulation methods
	/// </summary>
	public class Audio
	{
		#region Private variables
		private static Dictionary<string, AudioStream> _files = new Dictionary<string, AudioStream>();
		private static AudioStream _currentOutputStream;
		private static WaveChannel32 _volumeStream;
		private static WaveMixerStream32 _mixStream = new WaveMixerStream32();
		private static WaveOut _device = new WaveOut();

		private int test = 2;
		#endregion

		#region Initialization Methods

		/// <summary>
		/// Initialize the processing library with an .mp3 file
		/// </summary>
		/// <param name="fileName">The filename of the song to load</param>
		public static void LoadFile(string fileName)
		{
			if (fileName.EndsWith(".mp3"))
			{
				_files[fileName] = new AudioStream(new WaveChannel32(new Mp3FileReader(fileName)));
				if (_currentOutputStream == null)
				{
					_currentOutputStream = _files[fileName];
					_volumeStream = new WaveChannel32(_currentOutputStream.Stream);
					_mixStream.AddInputStream(_currentOutputStream.Stream);
					_device.Init(_mixStream);
				}
			}
			else
			{
				throw new InvalidOperationException("Unsupported file extension");
			}
		}

		/// <summary>
		/// Removes a song from the list of playable songs
		/// </summary>
		/// <param name="fileName">The filename of the song to remove</param>
		public static void UnLoadFile(string fileName)
		{
			if (fileName.EndsWith(".mp3"))
			{
				try
				{
					_files.Remove(fileName);
				}
				catch (System.Exception)
				{
					throw new ArgumentException(String.Format("File of name '{0}' could not be removed", fileName));
				}
			}
			else
			{
				throw new InvalidOperationException("Unsupported file extension");
			}
		}
		#endregion

		#region Cleanup Methods
		public static void Cleanup()
		{
			if (_device != null)
			{
				_device.Stop();
			}
			if (_currentOutputStream != null)
			{
				// Close the file and ACM conversion
				_volumeStream.Close();
				_volumeStream = null;
				// Close the metering stream
				_currentOutputStream.Stream.Close();
				_currentOutputStream = null;
			}
			if (_mixStream != null)
			{
				_mixStream.Close();
				_mixStream.Dispose();
				_mixStream = null;
			}
			if (_device != null)
			{
				_device.Dispose();
				_device = null;
			}
		}
		#endregion

		#region Basic Audio Methods
		public static void Play()
		{
			if (_device.PlaybackState == PlaybackState.Stopped)
			{
				_device.Play();
			}
		}

		public static void Pause()
		{
			if (_device.PlaybackState == PlaybackState.Playing)
			{
				// Stop seems to work better than _device.Pause() here..., and doesn't reset the track
				_device.Stop();
			}
		}

		public static void Reset()
		{
			_currentOutputStream.CurrentTime = TimeSpan.FromSeconds(0);
		}

		/// <summary>
		/// Change the currently playing track
		/// </summary>
		/// <param name="fileName">The filename of the song to start playing</param>
		/// <param name="keepOldPosition">Maintain the position of the track being swapped out (defaults to true)</param>
		/// <param name="resetNewPosition">Begin at the last stopping position of the track being swapped in (defaults to true)</param>
		public static void SwapTrack(string fileName, bool keepOldPosition = true, bool keepNewPosition = true)
		{
			// Stop the current stream
			_currentOutputStream.CurrentTime = _mixStream.CurrentTime;
			_mixStream.RemoveInputStream(_currentOutputStream.Stream);
			// Switch tracks
			_currentOutputStream = _files[fileName];
			_mixStream.AddInputStream(_currentOutputStream.Stream);
			_mixStream.CurrentTime = _currentOutputStream.CurrentTime;
			// Resume
			_device.Play();
		}
		#endregion


		#region Manipulations
		/// <summary>
		/// Change the volume of the current song
		/// </summary>
		/// <returns>The new volume</returns>
		public static int ChangeVolume()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Change the tempo (BPM) of the current song
		/// </summary>
		/// <returns>The new tempo</returns>
		public static int ChangeTempo()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Change the pitch of the current song
		/// </summary>
		/// <returns>The new pitch</returns>
		public static int ChangePitch()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Change the play position of the current song
		/// </summary>
		/// <returns>The new current time of the song</returns>
		public static TimeSpan Seek(long offset)
		{
			_currentOutputStream.Stream.Seek(offset, SeekOrigin.Current);
			return _currentOutputStream.CurrentTime;
		}

		#endregion

		#region Private Helper Classes
		/// <summary>
		/// Helper class to consolidate an output stream's bitstream + its current play time
		/// </summary>
		private class AudioStream
		{
			public WaveStream Stream;
			public TimeSpan CurrentTime;

			public AudioStream(WaveStream outputStream)
			{
				Stream = outputStream;
				CurrentTime = TimeSpan.FromSeconds(0);
			}
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using TagLib;

namespace NoteEditor
{
    class Music : IDisposable
    {
        private EventWaitHandle WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private IWavePlayer wavePlayer;
        private AudioFileReader reader;
        private Time TotalTime;
        private byte[] MusicData;

        public string MusicName
        {
            get; set;
        }
        private bool _IsPlaying;
        public bool IsPlaying
        {
            get
            {
                return _IsPlaying;
            }
        }

        public Music(string path)
        {
            wavePlayer = new WaveOut();
            reader = new AudioFileReader(path);
            //Console.WriteLine("Total Play Time): " + reader.TotalTime);
            wavePlayer.Init(reader);

            //MusicData = new byte[reader.Length];
            MusicData = System.IO.File.ReadAllBytes(path);

            //for(var i = 0; i<MusicData.Length; ++i)
            //Console.WriteLine(MusicData[i]);

            reader.CurrentTime = TimeSpan.Zero;
            wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            TotalTime = new Time($"{reader.TotalTime.Hours}.{reader.TotalTime.Minutes}." +
                                 $"{reader.TotalTime.Seconds}.{reader.TotalTime.Milliseconds}");
            TagLib.File file = TagLib.File.Create(path);
            MusicName = file.Tag.Title;
            
        }

        public Music(byte[] data)
        {
            wavePlayer = new WaveOut();
            //Console.WriteLine("Total Play Time): " + reader.TotalTime);
        }

        public void Play()
        {
            wavePlayer.Play();
            _IsPlaying = true;
        }

        public void Pause()
        {
            wavePlayer.Pause();
            _IsPlaying = false;
        }

        public void Stop()
        {
            wavePlayer.Stop();
            reader.CurrentTime = TimeSpan.Zero;
            _IsPlaying = false;
        }
        public TimeSpan GetTotalTime()
        {
            return reader.TotalTime;
        }

        public string GetMusicProgressPer()
        {
            return (reader.CurrentTime.TotalMilliseconds / reader.TotalTime.TotalMilliseconds).ToString();
        }
        public string GetMusicProgress()
        {
            return $"{reader.CurrentTime.TotalSeconds} / {reader.TotalTime.TotalSeconds}";
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            WaitHandle.Set();
        }

        internal bool IsMusicEnd()
        {
            return reader.CurrentTime.Equals(reader.TotalTime) ? true : false;
        }

        public void Dispose()
        {
            wavePlayer.Dispose();
            reader.Dispose();
            WaitHandle.Close();
        }
    }
}

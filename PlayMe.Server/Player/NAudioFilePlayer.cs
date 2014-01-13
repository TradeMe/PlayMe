using System;
using System.IO;
using System.Net;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace PlayMe.Server.Player
{
    public class NAudioFilePlayer : NAudioPlayer, IStreamedPlayer
    {
        private WaveChannel32 volumeStream;
        private readonly IVolumeControl volumeControl;
        private bool reset;
        public NAudioFilePlayer(IVolumeControl volumeControl)
        {
            this.volumeControl = volumeControl;
        }
        
        public event EventHandler PlaybackEnded;

        public void PlayFromUrl(Uri url)
        {
            var response = WebRequest.Create(url).GetResponse();
            Stream ms = new MemoryStream();
            new Thread(delegate(object o)
            {                
                using (var responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[65536]; // 64KB chunks
                    int read;
                    while (responseStream != null && (read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        var pos = ms.Position;
                        ms.Position = ms.Length;
                        ms.Write(buffer, 0, read);
                        ms.Position = pos;
                    }
                }
            }).Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (ms.Length < response.ContentLength &&  ms.Length < 65536 * 10)
                Thread.Sleep(100);

            ms.Position = 0;
            PlayStream(new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))));
        }

        public void PlayFromFile(string fileName)
        {
            PlayStream(new WaveFileReader(fileName));
        }

        private void PlayStream(WaveStream waveStream)
        {
            new Thread(() =>
                           {
                               using (waveStream)
                               {
                                   volumeStream = new WaveChannel32(waveStream) { Volume = volumeControl.CurrentVolume, PadWithZeroes = true };
                                   Output = new WasapiOut(AudioClientShareMode.Shared, false, 300);
                                   using (Output)
                                   {                                       
                                       Output.Init(volumeStream);
                                       Output.Play();

                                       while (volumeStream.Position < volumeStream.Length & !reset)
                                       {
                                           Thread.Sleep(100);
                                       }
                                   }
                                   Output = null;
                                   if(!reset) RaisePlaybackEnded();
                                   reset = false;
                               }
                           }).Start();
        }

        public void IncreaseVolume()
        {
            if (volumeStream != null)
            {
                volumeStream.Volume = volumeControl.IncreaseVolume();
            }
        }

        public void DecreaseVolume()
        {
            if (volumeStream != null)
            {
                volumeStream.Volume = volumeControl.DecreaseVolume();
            }
        }

        public override void Reset()
        {
            Output.Stop();
            reset = true;
        }

        protected void RaisePlaybackEnded()
        {
            if (PlaybackEnded != null)
            {
                PlaybackEnded(this, new EventArgs());
            }
        }
    }
}

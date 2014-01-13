using System;
using NAudio.Wave;
using PlayMe.Server.Player;

namespace PlayMe.Server
{
    class NAudioBufferedPlayer : NAudioPlayer, IBufferedPlayer
    {

        private VolumeWaveProvider16 volumeWaveProvider;
        private BufferedWaveProvider bufferedWaveProvider;
        private readonly IVolumeControl volumeControl;

        public NAudioBufferedPlayer(IVolumeControl volumeControl)
        {
            this.volumeControl = volumeControl;            
        }

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            if (bufferedWaveProvider == null)
            {                
                bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(rate, channels));
                volumeWaveProvider = new VolumeWaveProvider16(bufferedWaveProvider) {Volume = volumeControl.CurrentVolume};                
                //Output = new WasapiOut(AudioClientShareMode.Shared, false, 0);                
                Output = new DirectSoundOut(70);
                Output.Init(volumeWaveProvider);
                Output.Play();
            }
            int space = bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes;
            if (space > samples.Length)
            {
                bufferedWaveProvider.AddSamples(samples, 0, samples.Length);
                return frames;
            }
            return 0;
        }

        public event EventHandler PlaybackEnded;

        public override void Reset()
        {
            if (bufferedWaveProvider != null)
            {
                bufferedWaveProvider.ClearBuffer();
                bufferedWaveProvider = null;
            }
        }


        public void IncreaseVolume()
        {
            volumeWaveProvider.Volume = volumeControl.IncreaseVolume();
        }

        public void DecreaseVolume()
        {
            volumeWaveProvider.Volume = volumeControl.DecreaseVolume();
        }
        
    }
}

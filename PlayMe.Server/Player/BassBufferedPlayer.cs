using System;
using Un4seen.Bass;

namespace PlayMe.Server.Player
{
    public class BassBufferedPlayer : IBufferedPlayer
    {
        private readonly IVolumeControl volumeControl;
        private BASSBuffer basbuffer;
        private STREAMPROC streamproc;
        private int currentHandle;

        public BassBufferedPlayer(IVolumeControl volumeControl)
        {
            this.volumeControl = volumeControl;
            Bass.BASS_SetVolume(volumeControl.CurrentVolume);
        }
        public void Pause()
        {
            Bass.BASS_ChannelPause(currentHandle);
        }

        public void Resume()
        {
            Bass.BASS_ChannelPlay(currentHandle, false);
        }

        public void IncreaseVolume()
        {
            Bass.BASS_SetVolume(volumeControl.IncreaseVolume());
        }

        public void DecreaseVolume()
        {
            Bass.BASS_SetVolume(volumeControl.DecreaseVolume());
        }

        public void Reset()
        {
            if (basbuffer != null)
                basbuffer.Clear();
        }

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;
            if (basbuffer == null)
            {
                Bass.BASS_Init(-1, rate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                basbuffer = new BASSBuffer(0.5f, rate, channels, 2);
                streamproc = Reader;
                currentHandle = Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, streamproc, IntPtr.Zero);
                Bass.BASS_ChannelPlay(currentHandle, false);
            }

            if (basbuffer.Space(0) > samples.Length)
            {
                basbuffer.Write(samples, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        private int Reader(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return basbuffer.Read(buffer, length, user.ToInt32());
        }

        public event EventHandler PlaybackEnded;
    }
}

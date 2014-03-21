using System;
using System.IO;
using Un4seen.Bass;

namespace PlayMe.Server.Player
{
    public class BassStreamedPlayer : IStreamedPlayer
    {
        private int handle;
        private SYNCPROC sync;
        private readonly IVolumeControl volumeControl;

        public event EventHandler PlaybackEnded;
        
        public BassStreamedPlayer(IVolumeControl volumeControl)
        {
            this.volumeControl = volumeControl;
        }

        public void Pause()
        {
            Bass.BASS_ChannelPause(handle);
        }

        public void Resume()
        {
            Bass.BASS_ChannelPlay(handle, false);
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
            if (handle > 0)
            {
                Bass.BASS_ChannelStop(handle);
            }
        }

        public void PlayFromUrl(Uri url)
        {
            handle = Bass.BASS_StreamCreateURL(url.OriginalString, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);
            PlayStream();
        }

        public void PlayFromFile(string fileName)
        {            
            var fileInfo = new FileInfo(fileName);
            handle = Bass.BASS_StreamCreateFile(fileName, 0, fileInfo.Length, BASSFlag.BASS_DEFAULT);            
            PlayStream();
        }

        protected void PlayStream()
        {
            if (handle > 0)
            {
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                sync = BASS_PlaybackEnded;
                Bass.BASS_ChannelSetSync(handle, BASSSync.BASS_SYNC_END, 0, sync, IntPtr.Zero);
                Bass.BASS_ChannelPlay(handle, false);
            }
        }
        
        private void BASS_PlaybackEnded(int hnd, int channel, int data, IntPtr user)
        {
            if (PlaybackEnded != null)
            {
                PlaybackEnded(this, new EventArgs());
            }
        }
    }
}

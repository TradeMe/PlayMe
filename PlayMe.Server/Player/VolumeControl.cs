using System;

namespace PlayMe.Server.Player
{
    public class VolumeControl : IVolumeControl
    {
        private const float VolumeStep = 0.05f;

        public float CurrentVolume { get; set; }

        public VolumeControl(ISettings settings)
        {
            CurrentVolume = settings.StartUpVolume;
        }

        public float IncreaseVolume()
        {
            CurrentVolume += VolumeStep;
            if (CurrentVolume > 1) CurrentVolume = 1;
            return CurrentVolume;
        }

        public float DecreaseVolume()
        {
            CurrentVolume -= VolumeStep;
            if (CurrentVolume < 0.05f) CurrentVolume = 0;
            return CurrentVolume;
        }
    }
}

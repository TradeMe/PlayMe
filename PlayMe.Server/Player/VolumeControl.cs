namespace PlayMe.Server.Player
{
    public class VolumeControl : IVolumeControl
    {
        private const float VolumeStep = 0.05f;

        private float currentVolume = 0.5f;
        public float CurrentVolume
        {
            get { return currentVolume; }
        }
        public float IncreaseVolume()
        {
            currentVolume += VolumeStep;
            if (currentVolume > 1) currentVolume = 1;
            return currentVolume;
        }

        public float DecreaseVolume()
        {
            currentVolume -= VolumeStep;
            if (currentVolume < 0) currentVolume = 0;
            return currentVolume;
        }
    }
}

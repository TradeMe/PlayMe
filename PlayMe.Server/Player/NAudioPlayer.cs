using NAudio.Wave;

namespace PlayMe.Server.Player
{
    public abstract class NAudioPlayer
    {
        protected IWavePlayer Output { get; set; }

        public void Pause()
        {
           if(Output!=null) Output.Pause();
        }

        public void Resume()
        {
            if(Output!=null) Output.Play();            
        }

        public abstract void Reset();
    }
}

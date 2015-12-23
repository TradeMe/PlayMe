using System.Collections.Generic;

namespace PlayMe.Server
{
    public interface ISettings
    {
        string AdminUsers { get; }
        int MinAutoplayableTracks { get; }
        int MaxAutoplayableTracks { get; }
        int VetoCount { get; }
        int Randomizer { get; }
        List<int> Randomizers { get; }
        int RandomizerRatio { get; }
        decimal RandomWeighting { get; }
        int DontRepeatTrackForHours { get; }
        bool AutoStart { get; }
        float StartUpVolume { get; }
        bool ForgetTrackThatExceedsMaxVetoes { get; }
    }
}
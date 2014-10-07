using System;
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
        int RandomizerRatio { get; }
        decimal RandomWeighting { get; }
        int DontRepeatTrackForHours { get; }
        bool AutoStart { get; }
        float StartUpVolume { get; }
    }
}

namespace PlayMe.Server.SoundBoard
{
    public interface ISoundBoardSettings
    {
        bool IsEnabled { get; }
        int SecondsBetweenSkipThreshold { get; }
    }
}

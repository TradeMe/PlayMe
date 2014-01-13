namespace SpotiFire.SpotifyLib
{
    public interface ILink : ISpotifyObject
    {
        sp_linktype Type { get; }
        object Object { get; }
    }

    public interface ILink<out T> : ILink
    {
        T Object { get; }
    }
}

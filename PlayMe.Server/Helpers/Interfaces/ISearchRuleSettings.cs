namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISearchRuleSettings
    {
        string ForbiddenTrackNames { get; }
        string ForbiddenAlbumNames { get; }
        string ForbiddenArtistNames { get; }
    }
}

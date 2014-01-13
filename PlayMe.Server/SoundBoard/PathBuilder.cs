using System.IO;

namespace PlayMe.Server.SoundBoard
{
    public class PathBuilder : IPathBuilder
    {
        private const string soundFolder = "SoundBoard\\Sounds";
        private readonly IFolderHelper folderHelper;

        public PathBuilder(IFolderHelper folderHelper)
        {
            this.folderHelper = folderHelper;
        }

        public string BuildFilePath(string fileName)
        {
            return Path.Combine(folderHelper.ApplicationFolder, soundFolder, fileName);
        }
    }
}

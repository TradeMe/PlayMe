using System;

namespace PlayMe.Server.Providers.SpotifyProvider
{
    public class ImageHelper
    {
        public static string GetImageUrl(string id, int size)
        {
            string result=string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                result = string.Format("http://d3rt1990lpmkn.cloudfront.net/{0}/{1}", size, id.ToLower());
            }
            return result;
        }
    }
}
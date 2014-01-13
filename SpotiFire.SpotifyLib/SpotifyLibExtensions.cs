using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire.SpotifyLib
{

    public static class ISessionExtensions
    {
        // IImage methods
        public static IImage GetImageFromId(this ISession session, string id)
        {
            return Image.FromId(session, id);
        }

        public static System.Drawing.Image GetImage(this IImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (!image.IsLoaded)
                throw new InvalidOperationException("Can't convert to Image before data is loaded.");

            if (image.Format != sp_imageformat.SP_IMAGE_FORMAT_JPEG)
                throw new InvalidOperationException("Can only parse Jpeg image data.");

            return System.Drawing.Image.FromStream(new MemoryStream(image.Data));
        }

        public static void Save(this IImage image, string location)
        {
            image.GetImage().Save(location);
        }

        // IImage Synchronously
        public static void WaitForLoaded(this IImage image)
        {
            var reset = new ManualResetEvent(image.IsLoaded);
            ImageEventHandler handler = (i, e) => reset.Set();
            image.Loaded += handler;
            reset.WaitOne();
            image.Loaded -= handler;
        }


        // Search methods made Synchronously
        public static void WaitForCompletion(this ISearch search)
        {
            var reset = new ManualResetEvent(search.IsComplete);
            SearchEventHandler handler = (s, e) => reset.Set();
            search.Complete += handler;
            reset.WaitOne(2000);
            search.Complete -= handler;
        }
        public static ISearch SearchTracks(this ISession session, string query, int trackOffset, int trackCount)
        {
            return session.Search(query, trackOffset, trackCount, 0, 0, 0, 0, 0, 0, sp_search_type.STANDARD);
        }
        public static ISearch SearchAlbums(this ISession session, string query, int albumOffset, int albumCount)
        {
            return session.Search(query, 0, 0, albumOffset, albumCount, 0, 0, 0, 0, sp_search_type.STANDARD);
        }
        public static ISearch SearchArtists(this ISession session, string query, int artistOffset, int artistCount)
        {
            return session.Search(query, 0, 0, 0, 0, artistOffset, artistCount, 0, 0, sp_search_type.STANDARD);
        }
        public static ISearch SearchPlaylist(this ISession session, string query, int playlistOffset, int playlistCount)
        {
            return session.Search(query, 0, 0, 0, 0, 0, 0, playlistOffset, playlistCount, sp_search_type.STANDARD);
        }

        // ArtistBrowse methods made Synchronously
        public static void WaitForCompletion(this IArtistBrowse artistBrowse)
        {
            var reset = new ManualResetEvent(artistBrowse.IsComplete);
            ArtistBrowseEventHandler handler = (a, e) => reset.Set();
            artistBrowse.Complete += handler;
            bool result=reset.WaitOne(10000);
            artistBrowse.Complete -= handler;
        }

        // AlbumBrowse methods made Synchronously
        public static void WaitForCompletion(this IAlbumBrowse albumBrowse)
        {
            var reset = new ManualResetEvent(albumBrowse.IsComplete);
            AlbumBrowseEventHandler handler = (a, e) => reset.Set();
            albumBrowse.Complete += handler;
            reset.WaitOne(10000);
            albumBrowse.Complete -= handler;
        }

        // Load made a task
        private readonly static List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> waiting = new List<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>();
        private static bool running = false;
        public static Task<IAsyncLoaded> Load(this IAsyncLoaded loadable)
        {
            Action start = () =>
            {
                running = true;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    while (true)
                    {
                        Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>[] l;
                        Thread.Sleep(100);
                        lock (waiting)
                            l = waiting.ToArray();

                        HashSet<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>> r = new HashSet<Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>>();

                        foreach (var i in l)
                            if (i.Item1.IsLoaded)
                            {
                                i.Item2.SetResult(i.Item1);
                                r.Add(i);
                            }

                        lock (waiting)
                        {
                            waiting.RemoveAll(e => r.Contains(e));
                            if (waiting.Count == 0)
                            {
                                running = false;
                                break;
                            }
                        }
                    }
                });
            };

            TaskCompletionSource<IAsyncLoaded> tcs = new TaskCompletionSource<IAsyncLoaded>();
            if (loadable.IsLoaded)
                tcs.SetResult(loadable);
            else if (running)
                lock (waiting)
                    if (running)
                        waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                    else
                    {
                        waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                        start();
                    }
            else
                lock (waiting)
                {
                    waiting.Add(new Tuple<IAsyncLoaded, TaskCompletionSource<IAsyncLoaded>>(loadable, tcs));
                    start();
                }

            return tcs.Task;
        }

        public static Task<ITrack> Load(this ITrack track)
        {
            return ((IAsyncLoaded)track).Load().ContinueWith(task => (ITrack)task.Result);
        }

        public static sp_error WaitForLoginComplete(this ISession session)
        {
            var result = sp_error.OK;
            var reset = new ManualResetEvent(false);
            SessionEventHandler handler = (a, e) =>
            {
                result = e.Status;
                reset.Set();
            };
            session.LoginComplete += handler;
            reset.WaitOne();
            session.LoginComplete -= handler;
            return result;
        }
    }

}

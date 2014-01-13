using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    public delegate void ImageEventHandler(IImage sender, EventArgs e);
    internal class Image : CountedDisposeableSpotifyObject, IImage
    {
        #region Wrapper
        private class ImageWrapper : DisposeableSpotifyObject, IImage
        {
            internal Image image;
            public ImageWrapper(Image image)
            {
                this.image = image;
                image.Loaded += new ImageEventHandler(image_Loaded);
            }

            void image_Loaded(IImage sender, EventArgs e)
            {
                if (Object.ReferenceEquals(sender, image))
                    if (Loaded != null)
                        Loaded(this, e);
            }

            protected override void OnDispose()
            {
                image.Loaded -= new ImageEventHandler(image_Loaded);
                Image.Delete(image.imagePtr);
                image = null;
            }

            public byte[] Data
            {
                get { IsAlive(true); return image.Data; }
            }

            public sp_error Error
            {
                get { IsAlive(true); return image.Error; }
            }

            public sp_imageformat Format
            {
                get { IsAlive(true); return image.Format; }
            }

            public string ImageId
            {
                get { IsAlive(true); return image.ImageId; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return image.IsLoaded; }
            }

            public event ImageEventHandler Loaded;

            public ISession Session
            {
                get { IsAlive(true); return image.Session; }
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? image.imagePtr.GetHashCode() : 0;
            }
        }

        internal static IntPtr GetPointer(IImage image)
        {
            if (image.GetType() == typeof(ImageWrapper))
                return ((ImageWrapper)image).image.imagePtr;
            throw new ArgumentException("Invalid image");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Image> images = new Dictionary<IntPtr, Image>();
        private static readonly object imagesLock = new object();

        internal static IImage Get(Session session, IntPtr imagePtr)
        {
            Image image;
            lock (imagesLock)
            {
                if (!images.ContainsKey(imagePtr))
                {
                    images.Add(imagePtr, new Image(session, imagePtr));
                }
                image = images[imagePtr];
                image.AddRef();
            }
            return new ImageWrapper(image);
        }

        internal static void Delete(IntPtr imagePtr)
        {
            lock (imagesLock)
            {
                Image image = images[imagePtr];
                int count = image.RemRef();
                if (count == 0)
                    images.Remove(imagePtr);
            }
        }
        #endregion

        #region Delegates
        private delegate void image_loaded_cb(IntPtr imagePtr, IntPtr userdataPtr);
        #endregion

        #region Declarations
        internal IntPtr imagePtr = IntPtr.Zero;
        private Session session;
        private image_loaded_cb image_loaded;
        private IntPtr imageLoadedPtr = IntPtr.Zero;
        #endregion

        #region Constructor
        private Image(Session session, IntPtr imagePtr)
        {
            if (imagePtr == IntPtr.Zero)
                throw new ArgumentException("imagePtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.imagePtr = imagePtr;
            this.image_loaded = new image_loaded_cb(ImageLoadedCallback);
            this.imageLoadedPtr = Marshal.GetFunctionPointerForDelegate(this.image_loaded);
            lock (libspotify.Mutex)
            {
                libspotify.sp_image_add_ref(imagePtr);
                libspotify.sp_image_add_load_callback(imagePtr, this.imageLoadedPtr, IntPtr.Zero);
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_image_is_loaded(imagePtr);
            }
        }

        public sp_error Error
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_image_error(imagePtr);
            }
        }

        public sp_imageformat Format
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_image_format(imagePtr);
            }
        }

        public byte[] Data
        {
            get
            {
                IsAlive(true);
                try
                {
                    IntPtr lengthPtr = IntPtr.Zero;
                    IntPtr dataPtr = IntPtr.Zero;
                    lock (libspotify.Mutex)
                        dataPtr = libspotify.sp_image_data(imagePtr, out lengthPtr);

                    int length = lengthPtr.ToInt32();

                    if (dataPtr == IntPtr.Zero)
                        return null;
                    if (length == 0)
                        return new byte[0];

                    byte[] imageData = new byte[length];
                    Marshal.Copy(dataPtr, imageData, 0, length);
                    return imageData;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string ImageId
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.ImageIdToString(libspotify.sp_image_image_id(imagePtr));
            }
        }

        public ISession Session
        {
            get
            {
                IsAlive(true);
                return session;
            }
        }
        #endregion

        #region Private Methods
        private static Delegate CreateDelegate<T>(Expression<Func<Image, Action<T>>> expr, Image s) where T : EventArgs
        {
            return expr.Compile().Invoke(s);
        }
        private void ImageLoadedCallback(IntPtr imagePtr, IntPtr userdataPtr)
        {
            if (imagePtr == this.imagePtr)
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(i => i.OnLoaded, this), new EventArgs()));
        }
        #endregion

        #region Protected Methods
        protected virtual void OnLoaded(EventArgs args)
        {
            if (this.Loaded != null)
                this.Loaded(this, args);
        }
        #endregion

        #region Events
        public event ImageEventHandler Loaded;
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                {
                    libspotify.sp_image_remove_load_callback(imagePtr, imageLoadedPtr, IntPtr.Zero);
                    libspotify.sp_image_release(imagePtr);
                }

            imageLoadedPtr = IntPtr.Zero;
            imagePtr = IntPtr.Zero;
        }
        #endregion

        #region Static Public Methods
        // TODO: Move to somewhere public.
        internal static IImage FromId(ISession session, string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (session.GetType() != typeof(Session))
                throw new ArgumentException("Session must be a spotify-session.");

            if (id.Length != 40)
                throw new ArgumentException("invalid id", "id");

            byte[] idArray = libspotify.StringToImageId(id);

            if (idArray.Length != 20)
                throw new Exception("Internal error in FromId");

            IntPtr imagePtr = IntPtr.Zero;
            IntPtr idPtr = IntPtr.Zero;
            IImage image = null;
            try
            {
                idPtr = Marshal.AllocHGlobal(idArray.Length);
                Marshal.Copy(idArray, 0, idPtr, idArray.Length);

                lock (libspotify.Mutex)
                    imagePtr = libspotify.sp_image_create(((Session)session).sessionPtr, idPtr);

                image = Image.Get((Session)session, imagePtr);

                lock (libspotify.Mutex)
                    libspotify.sp_image_release(imagePtr);

                return image;
            }
            catch
            {
                if (imagePtr != IntPtr.Zero)
                    try
                    {
                        lock (libspotify.Mutex)
                            libspotify.sp_image_release(imagePtr);
                    }
                    catch { }
                return null;
            }
            finally
            {
                if (idPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(idPtr);
            }
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return imagePtr.GetHashCode();
        }
    }
}

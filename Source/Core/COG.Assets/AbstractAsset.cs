using System;
using System.Collections.Generic;
using System.Text;

namespace COG.Assets
{
    public abstract class AbstractAsset<T> : IAsset<T>
        where T : IAssetData
    {
        private AssetUri m_uri;

        public AssetUri Uri { get { return m_uri; } }

        public AbstractAsset(AssetUri uri)
            : this()
        {
            m_uri = uri;

            if (!uri.isValid())
                return;
        }

        public abstract void Reload(T t);

        public override bool Equals(object obj)
        {
            if (obj is AbstractAsset<T>)
                return Equals(obj as AbstractAsset<T>);

            if (obj is IAsset)
                return Equals(obj as IAsset);

            return false;
        }

        public bool Equals(IAsset<T> p)
        {
            // If parameter is null, return false. 
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case. 
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false. 
            if (this.GetType() != p.GetType())
                return false;

            return this.Uri == p.Uri;
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        public static bool operator ==(AbstractAsset<T> a, AbstractAsset<T> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                if (object.ReferenceEquals(b, null))
                    return true;

                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(AbstractAsset<T> a, AbstractAsset<T> b)
        {
            return !(a == b);
        }

        public static implicit operator bool(AbstractAsset<T> t)
        {
            if (Object.ReferenceEquals(t, null))
                return false;

            return !t.IsDisposed;
        }

        /// <summary>
        /// default parameterless constructor
        /// </summary>
        /// <remarks>
        /// Provides tracking information when subclasses are instantiated
        /// </remarks>
        protected AbstractAsset()
        {
            IsDisposed = false;
            //#if !(XBOX || XBOX360 || WINDOWS_PHONE)
            //            ObjectManager.Instance.Add(this, Environment.StackTrace);
            //#else
            //ObjectManager.Instance.Add(this, String.Empty);
            //#endif
        }

        /// <summary>
        /// Base object destructor
        /// </summary>
        ~AbstractAsset()
        {
            if (!IsDisposed)
            {
                Dispose(false);
            }

        }

        #region IDisposable Implementation

        private bool m_isDisposed = false;

        /// <summary>
        /// Determines if this instance has been disposed of already.
        /// </summary>
        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (m_isDisposed)
                return;

            if (disposing)
            {
                DisposeManaged();
#if DEBUG
                //ObjectManager.Instance.Remove(this);
#endif
            }

            DisposedUnmanaged();

            m_isDisposed = true;
        }

        protected virtual void DisposeManaged() { }

        protected virtual void DisposedUnmanaged() { }

        /// <summary>
        /// Used to destroy the object and release any managed or unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation
    }
}

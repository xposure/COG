using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Framework
{
    public interface IDisposableObject : IDisposable
    {
        bool IsDisposed { get; }
    }

//    /// <summary>
//    /// Monitors the object lifetime of objects that are in control of unmanaged resources
//    /// </summary>
//    internal class ObjectManager : Singleton<ObjectManager>
//    {
//        public static readonly Logger logger = Logger.getLogger(typeof(ObjectManager));

//        private struct ObjectEntry
//        {
//            public WeakReference Instance;
//            public string ConstructionStack;
//        }

//        private readonly Dictionary<Type, List<ObjectEntry>> _objects = new Dictionary<Type, List<ObjectEntry>>();

//        /// <summary>
//        /// Add an object to be monitored
//        /// </summary>
//        /// <param name="instance">
//        /// A <see cref="DisposableObject"/> to monitor for proper disposal
//        /// </param>
//        public void Add(DisposableObject instance, string stackTrace)
//        {
//            List<ObjectEntry> objectList = GetOrCreateObjectList(instance.GetType());

//            objectList.Add(new ObjectEntry
//            {
//                Instance = new WeakReference(instance),
//                ConstructionStack = stackTrace
//            });
//        }

//        /// <summary>
//        ///  Remove an object from monitoring
//        /// </summary>
//        /// <param name="instance"></param>
//        public void Remove(DisposableObject instance)
//        {
//            var objectList = GetOrCreateObjectList(instance.GetType());
//            var objectEntry = from entry in objectList
//                              where entry.Instance.IsAlive && entry.Instance.Target == instance
//                              select entry;

//            objectList.Remove(objectEntry.First());
//        }

//        private List<ObjectEntry> GetOrCreateObjectList(Type type)
//        {
//            List<ObjectEntry> objectList;
//            if (_objects.ContainsKey(type))
//            {
//                objectList = _objects[type];
//            }
//            else
//            {
//                objectList = new List<ObjectEntry>();
//                _objects.Add(type, objectList);
//            }
//            return objectList;
//        }

//        #region Singleton<ObjectManager> Implementation

//        #endregion Singleton<ObjectManager> Implementation

//        #region IDisposable Implementation

//        protected override void dispose(bool disposeManagedResources)
//        {
//            if (!isDisposed)
//            {
//                if (disposeManagedResources)
//                {
//#if DEBUG
//                    long objectCount = 0;
//                    Dictionary<string, int> perTypeCount = new Dictionary<string, int>();
//                    StringBuilder msg = new StringBuilder();
//                    // Dispose managed resources.
//                    foreach (KeyValuePair<Type, List<ObjectEntry>> item in this._objects)
//                    {
//                        string typeName = item.Key.Name;
//                        List<ObjectEntry> objectList = item.Value;
//                        foreach (ObjectEntry objectEntry in objectList)
//                        {
//                            if (objectEntry.Instance.IsAlive && !((DisposableObject)objectEntry.Instance.Target).IsDisposed)
//                            {
//                                if (perTypeCount.ContainsKey(typeName))
//                                    perTypeCount[typeName]++;
//                                else
//                                    perTypeCount.Add(typeName, 1);

//                                objectCount++;
//                                msg.AppendFormat("\nAn instance of {0} was not disposed properly, creation stacktrace :\n{1}", typeName, objectEntry.ConstructionStack);
//                            }
//                        }
//                    }

//                    logger.info("[ObjectManager] Disposal Report:");

//                    if (objectCount > 0)
//                    {
//                        logger.warn("Total of {0} objects still alive.", objectCount);
//                        logger.warn("Types of not disposed objects count: " + perTypeCount.Count);

//                        foreach (KeyValuePair<string, int> currentPair in perTypeCount)
//                            logger.warn("{0} occurrence of type {1}", currentPair.Value, currentPair.Key);

//                        logger.warn("Creation Stacktraces:\n" + msg.ToString());
//                    }
//                    else
//                        logger.info("Everything went right! Congratulations!!");
//#endif
//                }

//                // There are no unmanaged resources to release, but
//                // if we add them, they need to be released here.
//            }

//            // If it is available, make the call to the
//            // base class's Dispose(Boolean) method
//            base.dispose(disposeManagedResources);
//        }

//        #endregion IDisposable Implementation
//    }

    public class Object
    {
        public static implicit operator bool(Object t)
        {
            return !System.Object.ReferenceEquals(t, null);
        }
    }

    /// <summary>
    /// Base class for all resource classes that require deterministic finalization and resource cleanup
    /// </summary>
    abstract public class DisposableObject : Object, IDisposableObject
    {
        /// <summary>
        /// default parameterless constructor
        /// </summary>
        /// <remarks>
        /// Provides tracking information when subclasses are instantiated
        /// </remarks>
        protected DisposableObject()
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
        ~DisposableObject()
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

            m_isDisposed = true;

            if (disposing)
            {
                DisposeManaged();
#if DEBUG
                //ObjectManager.Instance.Remove(this);
#endif
            }

            DisposedUnmanaged();

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

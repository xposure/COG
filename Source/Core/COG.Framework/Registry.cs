using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Framework
{
    public interface IRegistry : IDisposable
    {
        void SetRegistry(RegistryManager registry, SimpleUri uri);
    }

    public class Registry : DisposableObject
    {
        protected SimpleUri m_uri;
        protected RegistryManager m_registry;

        protected Registry()
        {
        }

        public virtual void SetRegistry(RegistryManager registry, SimpleUri uri)
        {
            Contract.Requires(!m_registry, "Registry was already set!");
            m_uri = uri;
            m_registry = registry;
        }
    }

    public class RegistryManager : DisposableObject
    {
        private Dictionary<SimpleUri, IRegistry> m_registries = new Dictionary<SimpleUri, IRegistry>();

        public void Add<T>(SimpleUri uri, T t)
            where T : IRegistry
        {
            if(m_registries.ContainsKey(uri))
                    throw new Exception(string.Format("Duplicate uri[{0}] added to registry", uri));

            m_registries.Add(uri, t);
            t.SetRegistry(this, uri);
        }

        public T Find<T>(SimpleUri uri)
            where T : IRegistry
        {
            IRegistry t;
            if (m_registries.TryGetValue(uri, out t) && t is T)
                    return (T)t;

            return default(T);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            foreach (var kvp in m_registries)
                kvp.Value.Dispose();

            m_registries.Clear();
            m_registries = null;
        }


    }
}

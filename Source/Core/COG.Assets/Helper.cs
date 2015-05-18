using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Assets
{
    internal static class Helper
    {
        public static bool Remove<T, U>(this Dictionary<T, U> target, T t, U u)
        {
            if (target == null)
                return false;

            return target.Remove(t, u);
        }

        public static U Find<T, U>(this Dictionary<T, U> target, T t)
        {
            if (target == null)
                return default(U);

            U u;
            if (target.TryGetValue(t, out u))
                return u;

            return default(U);
        }

        public static U FindOrCreate<T, U>(this Dictionary<T, U> target, T t)
            where U : new()
        {
            if (target == null)
                return default(U);

            U u;
            if (!target.TryGetValue(t, out u))
            {
                u = new U();
                target.Add(t, u);
                return u;
            }

            return u;
        }
    }
}

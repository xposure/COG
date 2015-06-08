using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COG.Dredger
{
    public static class Utils
    {
        public static void InitArray<T>(this T[] array)
            where T: new()
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = new T();
        }

        public static void InitArray<T>(this T[] array, Func<int, T> ctor)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = ctor(i);
        }


    }
}

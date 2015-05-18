
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace COG.Framework
{
    #region ExceptionFactory
    /// <summary>
    /// Factory class for some exception classes that have variable constructors based on the 
    /// framework that is targeted. Rather than use <c>#if</c> around the different constructors
    /// use the least common denominator, but wrap it in an easier to use method.
    /// </summary>
    internal static class ExceptionFactory
    {
        /// <summary>
        /// Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string name, object value, string message)
        {
            return new ArgumentOutOfRangeException(name, string.Format("{0} (actual value is '{1}')", message, value));
        }

        /// <summary>
        /// Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <returns></returns>
        public static ArgumentNullException CreateArgumentItemNullException(int index, string arrayName)
        {
            return new ArgumentNullException(String.Format("{0}[{1}]", arrayName, index));
        }
    }
    #endregion

    #region Proclaim
    /// <summary>
    /// 
    /// </summary>
    public static class Proclaim
    {
        /// <summary>
        /// Asserts if this statement is reached.
        /// </summary>
        /// <exception cref="InvalidOperationException">Code is supposed to be unreachable.</exception>
        public static Exception Unreachable
        {
            get
            {
                Debug.Assert(false, "Unreachable");
                return new InvalidOperationException("Code is supposed to be unreachable.");
            }
        }

        /// <summary>
        /// Asserts if any argument is <c>null</c>.
        /// </summary>
        /// <param name="vars"></param>
        [Conditional("DEBUG")]
        public static void NotNull(params object[] vars)
        {
            var result = true;
            foreach (var obj in vars)
            {
                result &= (obj != null);
            }
            Debug.Assert(result);
        }

        [Conditional("DEBUG")]
        public static void NotNull<T>(T t)
        {
            Debug.Assert(t != null, typeof(T).Name + " was null.");
        }

        /// <summary>
        /// Asserts if the string is <c>null</c> or zero length.
        /// </summary>
        /// <param name="str"></param>
        [Conditional("DEBUG")]
        public static void NotEmpty(string str)
        {
            Debug.Assert(!String.IsNullOrEmpty(str));
        }

        /// <summary>
        /// Asserts if the collection is <c>null</c> or the <c>Count</c> is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        [Conditional("DEBUG")]
        public static void NotEmpty<T>(ICollection<T> items)
        {
            Debug.Assert(items != null && items.Count > 0);
        }

        /// <summary>
        /// Asserts if any item in the collection is <c>null</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        [Conditional("DEBUG")]
        public static void NotNullItems<T>(IEnumerable<T> items) where T : class
        {
            Debug.Assert(items != null);
            foreach (object item in items)
            {
                Debug.Assert(item != null);
            }
        }
    }
    #endregion

    #region Contract
    /// <summary>
    /// This class is used to enforce that preconditions are met for method calls
    /// using clear and consice semantics.
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// Requires that a condition evaluates to <c>true</c>.
        /// </summary>
        /// <param name="condition"></param>
        /// <exception cref="ArgumentException">Condition is <c>false</c>.</exception>
        [Conditional("DEBUG")]
        public static void Requires(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException("Method condition violated.");
            }
        }

        /// <overloads>
        /// <param name="condition"></param>
        /// <param name="name">Name of the requirement, this should be something unique to make it easy to find.</param>
        /// </overloads>
        [Conditional("DEBUG")]
        public static void Requires(bool condition, string name)
        {
            Proclaim.NotEmpty(name);

            if (!condition)
            {
                throw new ArgumentException("Invalid parameter value.", name);
            }
        }

        /// <overloads>
        /// <param name="condition"></param>
        /// <param name="name">Name of the requirement, this should be something unique to make it easy to find.</param>
        /// <param name="message">Message if the condition isn't met</param>
        /// </overloads>
        [Conditional("DEBUG")]
        public static void Requires(bool condition, string name, string message)
        {
            Proclaim.NotEmpty(name);

            if (!condition)
            {
                throw new ArgumentException(message, name);
            }
        }

        /// <summary>
        /// Requires that a value not be <c>null</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">Value is <c>null</c>.</exception>
        [Conditional("DEBUG")]
        public static void RequiresNotNull<T>(T value, string name) where T : class
        {
            Proclaim.NotEmpty(name);

            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Requires that the string not be <c>null</c> and not zero length.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException">String is <c>null</c> or zero length.</exception>
        [Conditional("DEBUG")]
        public static void RequiresNotEmpty(string str, string name)
        {
            RequiresNotNull(str, name);
            if (str.Length == 0)
            {
                throw new ArgumentException("Non-empty string required.", name);
            }
        }

        /// <summary>
        /// Requires that the collection not be <c>null</c> and has at least one element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException">Collection is <c>null</c> or has no elements.</exception>
        [Conditional("DEBUG")]
        public static void RequiresNotEmpty<T>(ICollection<T> collection, string name)
        {
            RequiresNotNull(collection, name);
            if (collection.Count == 0)
            {
                throw new ArgumentException("Non-empty collection required.", name);
            }
        }

        /// <summary>
        /// Requires the specified index to point inside the array.
        /// </summary>
        /// <exception cref="ArgumentNullException">Array is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Index is outside the array.</exception>
        [Conditional("DEBUG")]
        public static void RequiresArrayIndex<T>(IList<T> array, int index, string indexName)
        {
            Proclaim.NotEmpty(indexName);
            Proclaim.NotNull(array);

            if (index < 0 || index >= array.Count)
            {
                throw new ArgumentOutOfRangeException(indexName);
            }
        }

        /// <summary>
        /// Requires the specified index to point inside the array or at the end.
        /// </summary>
        /// <exception cref="ArgumentNullException">Array is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Index is outside the array.</exception>
        [Conditional("DEBUG")]
        public static void RequiresArrayInsertIndex<T>(IList<T> array, int index, string indexName)
        {
            Proclaim.NotEmpty(indexName);
            Proclaim.NotNull(array);

            if (index < 0 || index > array.Count)
            {
                throw new ArgumentOutOfRangeException(indexName);
            }
        }

        /// <summary>
        /// Requires the range [offset, offset + count] to be a subset of [0, array.Count].
        /// </summary>
        /// <exception cref="ArgumentNullException">Array is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Offset or count are out of range.</exception>
        [Conditional("DEBUG")]
        public static void RequiresArrayRange<T>(IList<T> array, int offset, int count, string offsetName, string countName)
        {
            Proclaim.NotEmpty(offsetName);
            Proclaim.NotEmpty(countName);
            Proclaim.NotNull(array);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(countName);
            }
            if (offset < 0 || array.Count - offset < count)
            {
                throw new ArgumentOutOfRangeException(offsetName);
            }
        }

        /// <summary>
        /// Requires the range [offset, offset + count] to be a subset of [0, array.Count].
        /// </summary>
        /// <exception cref="ArgumentNullException">String is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Offset or count are out of range.</exception>
        [Conditional("DEBUG")]
        public static void RequiresArrayRange(string str, int offset, int count, string offsetName, string countName)
        {
            Proclaim.NotEmpty(offsetName);
            Proclaim.NotEmpty(countName);
            Proclaim.NotNull(str);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(countName);
            }
            if (offset < 0 || str.Length - offset < count)
            {
                throw new ArgumentOutOfRangeException(offsetName);
            }
        }

        /// <summary>
        /// Requires the array and all its items to be non-null.
        /// </summary>
        [Conditional("DEBUG")]
        public static void RequiresNotNullItems<T>(IList<T> items, string name)
        {
            Proclaim.NotNull(name);
            RequiresNotNull(items, name);

            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    throw ExceptionFactory.CreateArgumentItemNullException(i, name);
                }
            }
        }
    }
    #endregion
}

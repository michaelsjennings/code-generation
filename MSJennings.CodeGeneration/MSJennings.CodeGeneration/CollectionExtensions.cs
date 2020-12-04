using System;
using System.Collections.Generic;
using System.Linq;

namespace MSJennings.CodeGeneration
{
    public static class CollectionExtensions
    {
        #region IsIn - string

        public static bool IsIn(this string value, StringComparison comparisonType, IEnumerable<string> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Any(x => x.Equals(value, comparisonType));
        }

        public static bool IsIn(this string value, StringComparison comparisonType, params string[] collection)
        {
            return value.IsIn(comparisonType, collection.AsEnumerable());
        }

        public static bool IsIn(this string value, IEnumerable<string> collection)
        {
            return value.IsIn(StringComparison.OrdinalIgnoreCase, collection);
        }

        public static bool IsIn(this string value, params string[] collection)
        {
            return value.IsIn(StringComparison.OrdinalIgnoreCase, collection);
        }

        #endregion

        #region IsIn - <T>

        public static bool IsIn<T>(this T value, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Any(x => x.Equals(value));
        }

        public static bool IsIn<T>(this T value, params T[] collection)
        {
            return value.IsIn(collection.AsEnumerable());
        }

        #endregion

        #region IsFirstIn - string

        public static bool IsFirstIn(this string value, StringComparison comparisonType, IList<string> list)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count < 1)
            {
                return false;
            }

            return value.Equals(list[0], comparisonType);
        }

        public static bool IsFirstIn(this string value, StringComparison comparisonType, params string[] list)
        {
            return value.IsFirstIn(comparisonType, list.ToList());
        }

        public static bool IsFirstIn(this string value, IList<string> list)
        {
            return value.IsFirstIn(StringComparison.OrdinalIgnoreCase, list);
        }

        public static bool IsFirstIn(this string value, params string[] list)
        {
            return value.IsFirstIn(StringComparison.OrdinalIgnoreCase, list.ToList());
        }

        #endregion

        #region IsFirstIn - <T>

        public static bool IsFirstIn<T>(this T value, IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count < 1)
            {
                return false;
            }

            return value.Equals(list[0]);
        }

        public static bool IsFirstIn<T>(this T value, params T[] list)
        {
            return value.IsFirstIn(list.ToList());
        }

        #endregion

        #region IsLastIn - string

        public static bool IsLastIn(this string value, StringComparison comparisonType, IList<string> list)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count < 1)
            {
                return false;
            }

            return value.Equals(list.Last(), comparisonType);
        }

        public static bool IsLastIn(this string value, StringComparison comparisonType, params string[] list)
        {
            return value.IsLastIn(comparisonType, list.ToList());
        }

        public static bool IsLastIn(this string value, IList<string> list)
        {
            return value.IsLastIn(StringComparison.OrdinalIgnoreCase, list);
        }

        public static bool IsLastIn(this string value, params string[] list)
        {
            return value.IsLastIn(StringComparison.OrdinalIgnoreCase, list.ToList());
        }

        #endregion

        #region IsLastIn - <T>

        public static bool IsLastIn<T>(this T value, IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return value.Equals(list.Last());
        }

        public static bool IsLastIn<T>(this T value, params T[] list)
        {
            return value.IsLastIn(list.ToList());
        }

        #endregion

        #region Concat

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params T[] items)
        {
            var second = new List<T>(items);
            return collection.Concat(second);
        }

        public static IEnumerable<string> Concat(this IEnumerable<string> collection, params string[] items)
        {
            var second = new List<string>(items);
            return collection.Concat(second);
        }

        #endregion
    }
}

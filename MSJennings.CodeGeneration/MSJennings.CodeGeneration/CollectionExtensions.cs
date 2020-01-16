using System;
using System.Collections.Generic;
using System.Linq;

namespace MSJennings.CodeGeneration
{
    public static class CollectionExtensions
    {
        #region IsIn - string

        public static bool IsIn(this string value, StringComparison comparisonType, IEnumerable<string> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return list.Any(x => x.Equals(value, comparisonType));
        }

        public static bool IsIn(this string value, StringComparison comparisonType, params string[] list)
        {
            return value.IsIn(comparisonType, list.AsEnumerable());
        }

        public static bool IsIn(this string value, IEnumerable<string> list)
        {
            return value.IsIn(StringComparison.OrdinalIgnoreCase, list);
        }

        public static bool IsIn(this string value, params string[] list)
        {
            return value.IsIn(StringComparison.OrdinalIgnoreCase, list);
        }

        #endregion

        #region IsIn - <T>

        public static bool IsIn<T>(this T value, IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return list.Any(x => x.Equals(value));
        }

        public static bool IsIn<T>(this T value, params T[] list)
        {
            return value.IsIn(list.AsEnumerable());
        }

        #endregion

        #region IsFirstIn - string

        public static bool IsFirstIn(this string value, StringComparison comparisonType, IEnumerable<string> list)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return value.Equals(list.First(), comparisonType);
        }

        public static bool IsFirstIn(this string value, StringComparison comparisonType, params string[] list)
        {
            return value.IsFirstIn(comparisonType, list.AsEnumerable());
        }

        public static bool IsFirstIn(this string value, IEnumerable<string> list)
        {
            return value.IsFirstIn(StringComparison.OrdinalIgnoreCase, list);
        }

        public static bool IsFirstIn(this string value, params string[] list)
        {
            return value.IsFirstIn(StringComparison.OrdinalIgnoreCase, list.AsEnumerable());
        }

        #endregion

        #region IsFirstIn - <T>

        public static bool IsFirstIn<T>(this T value, IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return value.Equals(list.First());
        }

        public static bool IsFirstIn<T>(this T value, params T[] list)
        {
            return value.Equals(list.AsEnumerable().First());
        }

        #endregion

        #region isLastIn - string

        public static bool IsLastIn(this string value, StringComparison comparisonType, IEnumerable<string> list)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return value.Equals(list.Last(), comparisonType);
        }

        public static bool IsLastIn(this string value, StringComparison comparisonType, params string[] list)
        {
            return value.IsLastIn(comparisonType, list.AsEnumerable());
        }

        public static bool IsLastIn(this string value, IEnumerable<string> list)
        {
            return value.IsLastIn(StringComparison.OrdinalIgnoreCase, list);
        }

        public static bool IsLastIn(this string value, params string[] list)
        {
            return value.IsLastIn(StringComparison.OrdinalIgnoreCase, list.AsEnumerable());
        }

        #endregion

        #region IsLastIn - <T>

        public static bool IsLastIn<T>(this T value, IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return value.Equals(list.Last());
        }

        public static bool IsLastIn<T>(this T value, params T[] list)
        {
            return value.Equals(list.AsEnumerable().Last());
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

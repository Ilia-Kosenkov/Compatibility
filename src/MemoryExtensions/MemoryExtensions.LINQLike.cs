using System;

namespace MemoryExtensions
{
    public static partial class MemoryExtensions
    {
        #region LINQ-ish methods

        public static bool Any<T>(this ReadOnlySpan<T> @this, Func<T, bool> predicate)
        {
            foreach (var item in @this)
                if (predicate(item))
                    return true;

            return false;
        }

        public static bool Any<T>(this Span<T> @this, Func<T, bool> predicate)
        {
            foreach (var item in @this)
                if (predicate(item))
                    return true;

            return false;
        }

        public static bool Any(this ReadOnlySpan<bool> @this)
        {
            foreach (var condition in @this)
                if (condition)
                    return true;

            return false;
        }

        public static bool Any(this Span<bool> @this)
        {
            foreach (var condition in @this)
                if (condition)
                    return true;

            return false;
        }

        public static bool All<T>(this ReadOnlySpan<T> @this, Func<T, bool> predicate)
        {
            foreach (var item in @this)
                if (!predicate(item))
                    return false;

            return true;
        }

        public static bool All<T>(this Span<T> @this, Func<T, bool> predicate)
        {
            foreach (var item in @this)
                if (!predicate(item))
                    return false;

            return true;
        }

        public static bool All(this ReadOnlySpan<bool> @this)
        {
            foreach (var condition in @this)
                if (!condition)
                    return false;

            return true;
        }

        public static bool All(this Span<bool> @this)
        {
            foreach (var condition in @this)
                if (!condition)
                    return false;

            return true;
        }


        public static bool Contains<T>(this ReadOnlySpan<T> @this, T value) where T : unmanaged, IEquatable<T>
        {
            if (@this.IsEmpty)
                return false;

            foreach (var item in @this)
                if (item.Equals(value))
                    return true;

            return false;
        }

        public static bool Contains<T>(this Span<T> @this, T value) where T : unmanaged, IEquatable<T>
            => Contains((ReadOnlySpan<T>) @this, value);


        #endregion
    }
}
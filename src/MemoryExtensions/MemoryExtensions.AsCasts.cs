#nullable enable

using System;

namespace MemoryExtensions
{
    public static partial class MemoryExtensions
    {
        #region AsCast

        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array)
            => array;

        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new ReadOnlyMemory<T>(array, offset, length);
        }

        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array, int start, int length)
            => new ReadOnlyMemory<T>(array, start, length);

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
            => array;

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new ReadOnlySpan<T>(array, offset, length);
        }

        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int start, int length)
            => new ReadOnlySpan<T>(array, start, length);

        public static ReadOnlySpan<char> AsSpan(this string text, Range range)
        {
            if(string.IsNullOrEmpty(text))
                return ReadOnlySpan<char>.Empty;
            var (offset, length) = range.GetOffsetAndLength(text.Length);
            return text.AsSpan(offset, length);
        }


        public static ReadOnlyMemory<char> AsMemory(this string text, Range range)
        {
            if(string.IsNullOrEmpty(text))
                return ReadOnlyMemory<char>.Empty;
            var (offset, length) = range.GetOffsetAndLength(text.Length);
            return text.AsMemory(offset, length);
        }


        public static Span<T> AsSpan<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new Span<T>(array, offset, length);
        }

        public static Memory<T> AsMemory<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new Memory<T>(array, offset, length);
        }


        #endregion
    }
}
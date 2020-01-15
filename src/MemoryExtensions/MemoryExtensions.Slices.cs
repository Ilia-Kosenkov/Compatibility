using System;

namespace MemoryExtensions
{
    public static partial class MemoryExtensions
    {
        #region Slices

        public static Span<T> Slice<T>(this Span<T> @this, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(@this.Length);
            return @this.Slice(offset, length);
        }

        public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> @this, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(@this.Length);
            return @this.Slice(offset, length);
        }

        public static Memory<T> Slice<T>(this Memory<T> @this, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(@this.Length);
            return @this.Slice(offset, length);
        }

        public static ReadOnlyMemory<T> Slice<T>(this ReadOnlyMemory<T> @this, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(@this.Length);
            return @this.Slice(offset, length);
        }

        #endregion
    }
}
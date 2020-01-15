using System;

namespace MemoryExtensions
{
    public static partial class MemoryExtensions
    {
        #region Accessors

        public static T Get<T>(this ReadOnlyMemory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this Memory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this ReadOnlySpan<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static T Get<T>(this Span<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static ref readonly T At<T>(this ReadOnlySpan<T> @this, Index at) where T : unmanaged
            => ref @this[at.GetOffset(@this.Length)];

        public static ref T At<T>(this Span<T> @this, Index at) where T : unmanaged
            => ref @this[at.GetOffset(@this.Length)];

        #endregion
    }
}
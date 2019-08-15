//     MIT License
//     
//     Copyright(c) 2019 Ilia Kosenkov
//     
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of bytes software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and bytes permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.

using System;
using System.Collections.Generic;

namespace Compatibility.Bridge
{
    public static class IndexRangeExtensions
    {
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new ReadOnlySpan<T>(array, offset, length);
        }
        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array, Range range)
        {
            var (offset, length) =
                range.GetOffsetAndLength(array?.Length ?? throw new ArgumentNullException(nameof(array)));
            return new ReadOnlyMemory<T>(array, offset, length);
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

        public static T[] Subarray<T>(this T[] @this, Range range)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));
            if (!range.IsValidRange(@this.Length))
                throw new ArgumentOutOfRangeException(nameof(range));

            var (offset, length) = range.GetOffsetAndLength(@this.Length);

            var array = new T[length];

            for (var i = 0; i < length; i++)
                array[i] = @this[i + offset];

            return array;
        }

        public static string Substring(this string @this, Range range)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));
            if (!range.IsValidRange(@this.Length))
                throw new ArgumentOutOfRangeException(nameof(range));
            var (offset, length) = range.GetOffsetAndLength(@this.Length);

            return @this.Substring(offset, length);
        }

        public static bool IsValidRange(this Range @this, int length)
        {
            if(length <= 0)
                throw new ArgumentException("Length should be positive", nameof(length));
            var start = @this.Start.GetOffset(length);
            var end = @this.End.GetOffset(length);

            return
                start >= 0
                && end > start
                && end <= length;
        }

        public static bool IsValidRange(this Range @this, int length,
            out Range.OffsetAndLength offset)
        {
            if (length <= 0)
                throw new ArgumentException("Length should be positive", nameof(length));
            offset = default;

            var start = @this.Start.GetOffset(length);
            if (start < 0 || start >= length)
                return false;

            var end = @this.End.GetOffset(length);
            if (end <= start || end > length)
                return false;

            offset = new Range.OffsetAndLength(start, end - start);
            return true;
        }

        public static Maybe<Range.OffsetAndLength> MaybeValid(this Range @this, int length)
        {
            if (length <= 0)
                return Maybe<Range.OffsetAndLength>.None;

            var start = @this.Start.GetOffset(length);
            if (start < 0 || start >= length)
                return Maybe<Range.OffsetAndLength>.None;

            var end = @this.End.GetOffset(length);
            if (end <= start || end > length)
                return Maybe<Range.OffsetAndLength>.None;

            return new Range.OffsetAndLength(start, end - start);
        }

        public static T Get<T>(this ReadOnlyMemory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this Memory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this ReadOnlySpan<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static T Get<T>(this Span<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static char Get(this string @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static T Get<T>(this T[] @this, Index at)
            => (@this ?? throw new ArgumentNullException(nameof(@this)))[at.GetOffset(@this.Length)];

        public static T Get<T>(this IList<T> @this, Index at)
            => (@this ?? throw new ArgumentNullException(nameof(@this)))[at.GetOffset(@this.Count)];
    }
}

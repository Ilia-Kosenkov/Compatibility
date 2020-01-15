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

namespace IndexRangeExtensions
{
    public static class IndexRangeExtensions
    {
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
            out (int Offset, int Length) offset)
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

            offset = (Offset: start, Length: end - start);
            return true;
        }

        public static (int Offset, int Length)? ValidRange(this Range @this, int length)
        {
            if (length <= 0)
                return null;

            var start = @this.Start.GetOffset(length);
            if (start < 0 || start >= length)
                return null;

            var end = @this.End.GetOffset(length);
            if (end <= start || end > length)
                return null;

            return (Offset: start, Length: end - start);
        }

        public static char Get(this string @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static T Get<T>(this T[] @this, Index at)
            => (@this ?? throw new ArgumentNullException(nameof(@this)))[at.GetOffset(@this.Length)];

        public static T Get<T>(this IReadOnlyList<T> @this, Index at)
            => (@this ?? throw new ArgumentNullException(nameof(@this)))[at.GetOffset(@this.Count)];

        public static Index Add(this Index i1, Index i2, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var pos1 = i1.GetOffset(length);
            var pos2 = i2.GetOffset(length);

            var sum = pos1 + pos2;

            if(sum >= 0 && sum <= length)
                return new Index(sum);

            throw new IndexOutOfRangeException();
        }

        public static Index Subtract(this Index i1, Index i2, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var pos1 = i1.GetOffset(length);
            var pos2 = i2.GetOffset(length);

            var diff = pos1 - pos2;

            if (diff >= 0 && diff <= length)
                return new Index(diff);

            throw new IndexOutOfRangeException();
        }

        public static Index Multiply(this Index i, int m, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var pos = i.GetOffset(length);

            var newVal = pos * m;

            if (newVal >= 0 && newVal <= length)
                return new Index(newVal);

            throw new IndexOutOfRangeException();
        }

        public static Index Add(this Index i, int value)
            => i.IsFromEnd
                ? Index.FromEnd(i.Value - value)
                : Index.FromStart(i.Value + value);
        public static Index Subtract(this Index i, int value)
            => i.Add(-value);

        public static bool IsStart(this Index i, int length)
            => i.IsFromEnd
                ? i.GetOffset(length) == 0
                : i.Value == 0;

        public static bool IsEnd(this Index i, int length)
            => i.IsFromEnd
                ? i.Value == 0
                : i.GetOffset(length) == length;

        public static Range SliceFromStart(this Range rng, int size, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            return new Range(rng.Start, rng.Start.Add(size, length));
        }

        public static Range SliceFromEnd(this Range rng, int size, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            return new Range(rng.End.Subtract(size, length), rng.End);
        }

        public static Range RangeFrom(this Index i, int size, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            return new Range(i, i.Add(size, length));
        }

        public static Range RangeTo(this Index i, int size, int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));
            return new Range(i.Subtract(size, length), i);
        }

        public static RangeEnumerableProxy Enumerate(this Range range, int length = 1)
            => new RangeEnumerableProxy(range, length);
        
    }
}

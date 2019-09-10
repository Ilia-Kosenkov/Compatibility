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
using System.Linq;
using IndexRange;

namespace MemoryExtensions
{
    public static class MemoryExtensions
    {
        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array)
            => (ReadOnlyMemory<T>) array;
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
            => (ReadOnlySpan<T>) array;
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int start, int length)
            => new ReadOnlySpan<T>(array, start, length);
        public static ReadOnlyMemory<T> AsReadOnlyMemory<T>(this T[] array, int start, int length)
            => new ReadOnlyMemory<T>(array, start, length);

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
        
        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> @this)
        {
            if(@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;
            
            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for(; start < @this.Length; start++)
                if (!char.IsWhiteSpace(span[start]))
                    break;

            for (; end >= start; end--)
                if (!char.IsWhiteSpace(span[end]))
                    break;

            Range range = (start, end + 1);
            return !range.IsValidRange(@this.Length) 
                ? ReadOnlyMemory<char>.Empty 
                : @this.Slice(range);
        }

        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> @this, params char[] symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for (; start < @this.Length; start++)
                if (!symbols.Contains(span[start]))
                    break;

            for (; end >= start; end--)
                if (!symbols.Contains(span[end]))
                    break;

            Range range = (start, end + 1);
            return !range.IsValidRange(@this.Length) 
                ? ReadOnlyMemory<char>.Empty 
                : @this.Slice(range);
        }

        [Obsolete("Use " + nameof(System.MemoryExtensions.TrimEnd), true)]
        public static Span<char> TrimEnd(this Span<char> @this)
        {
            if (@this.IsEmpty)
                return Span<char>.Empty;

            var end = @this.Length - 1;
           
            for (; end >= 0; end--)
                if (!char.IsWhiteSpace(@this[end]))
                    break;

            var range = Range.EndAt(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Span<char>.Empty
                : @this.Slice(range);
        }


        public static bool SequenceEqual<T>(this ReadOnlyMemory<T> src, ReadOnlyMemory<T> tar) where T:IEquatable<T>
            => src.Span.SequenceEqual(tar.Span);

        [Obsolete("Use " + nameof(System.MemoryExtensions.SequenceEqual), true)]
        public static bool SequenceEqual<T>(this Memory<T> src, ReadOnlyMemory<T> tar) where T : IEquatable<T>
            => src.Span.SequenceEqual(tar.Span);
        
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

        public static T Get<T>(this ReadOnlyMemory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this Memory<T> @this, Index at)
            => @this.Span[at.GetOffset(@this.Length)];

        public static T Get<T>(this ReadOnlySpan<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];

        public static T Get<T>(this Span<T> @this, Index at)
            => @this[at.GetOffset(@this.Length)];
        
    }
}

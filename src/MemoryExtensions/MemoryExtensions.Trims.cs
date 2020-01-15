using System;
using System.Linq;
using IndexRangeExtensions;

namespace MemoryExtensions
{
    public static partial class MemoryExtensions
    {
        #region Trim

        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> @this)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for (; start < @this.Length; start++)
                if (!char.IsWhiteSpace(span[start]))
                    break;

            for (; end >= start; end--)
                if (!char.IsWhiteSpace(span[end]))
                    break;

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for (; start < @this.Length; start++)
                if (span[start] != symbol)
                    break;

            for (; end >= start; end--)
                if (span[end] != symbol)
                    break;

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> @this, ReadOnlySpan<char> symbols)
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

            var range = start..(end + 1);
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

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }

        public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> @this)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!char.IsWhiteSpace(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (span[start] != symbol)
                    break;

            

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> @this, ReadOnlySpan<char> symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!symbols.Contains(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> @this, params char[] symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!symbols.Contains(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }

        public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> @this)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!char.IsWhiteSpace(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return ReadOnlyMemory<char>.Empty;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (span[end] != symbol)
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> @this, ReadOnlySpan<char> symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!symbols.Contains(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }
        public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> @this, params char[] symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!symbols.Contains(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? ReadOnlyMemory<char>.Empty
                : @this.Slice(range);
        }



        public static Memory<char> Trim(this Memory<char> @this)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for (; start < @this.Length; start++)
                if (!char.IsWhiteSpace(span[start]))
                    break;

            for (; end >= start; end--)
                if (!char.IsWhiteSpace(span[end]))
                    break;

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> Trim(this Memory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            var end = @this.Length - 1;
            for (; start < @this.Length; start++)
                if (span[start] != symbol)
                    break;

            for (; end >= start; end--)
                if (span[end] != symbol)
                    break;

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> Trim(this Memory<char> @this, ReadOnlySpan<char> symbols)
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

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> Trim(this Memory<char> @this, params char[] symbols)
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

            var range = start..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }

        public static Memory<char> TrimStart(this Memory<char> @this)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!char.IsWhiteSpace(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimStart(this Memory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (span[start] != symbol)
                    break;



            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimStart(this Memory<char> @this, ReadOnlySpan<char> symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!symbols.Contains(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimStart(this Memory<char> @this, params char[] symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var start = 0;
            for (; start < @this.Length; start++)
                if (!symbols.Contains(span[start]))
                    break;

            var range = start..;
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }

        public static Memory<char> TrimEnd(this Memory<char> @this)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!char.IsWhiteSpace(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimEnd(this Memory<char> @this, char symbol)
        {
            if (@this.IsEmpty)
                return Memory<char>.Empty;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (span[end] != symbol)
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimEnd(this Memory<char> @this, ReadOnlySpan<char> symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!symbols.Contains(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }
        public static Memory<char> TrimEnd(this Memory<char> @this, params char[] symbols)
        {
            if (symbols.Length == 0)
                return @this;

            var span = @this.Span;
            var end = @this.Length - 1;

            for (; end >= 0; end--)
                if (!symbols.Contains(span[end]))
                    break;

            var range = ..(end + 1);
            return !range.IsValidRange(@this.Length)
                ? Memory<char>.Empty
                : @this.Slice(range);
        }


        #endregion
    }
}
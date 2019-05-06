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
using System.Runtime.CompilerServices;
using System.Text;

namespace Compatibility.Bridge
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

        public static unsafe int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
                return 0;
            fixed (byte* ptr = &bytes.GetPinnableReference())
            {
                return encoding.GetCharCount(ptr, bytes.Length);
            }
        }

        public static unsafe int GetCharCount(this Encoding encoding, ReadOnlyMemory<byte> bytes)
        {
            if (bytes.IsEmpty)
                return 0;
            using (var handle = bytes.Pin())
            {
                return encoding.GetCharCount((byte*)handle.Pointer, bytes.Length);
            }
        }

        public static unsafe int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            if (bytes.IsEmpty)
                return 0;

            fixed (byte* dataPtr = &bytes.GetPinnableReference())
                fixed (char* strPtr = &chars.GetPinnableReference())
                {
                    return encoding.GetChars(dataPtr, bytes.Length, strPtr, chars.Length);
                }
        }

        public static unsafe int GetChars(this Encoding encoding, ReadOnlyMemory<byte> bytes, Memory<char> chars)
        {
            if (bytes.IsEmpty)
                return 0;

            using (var dataHandle = bytes.Pin())
                using (var strHandle = chars.Pin())
                {
                    return encoding.GetChars((byte*) dataHandle.Pointer, bytes.Length, (char*) strHandle.Pointer,
                        chars.Length);
                }
        }

        public static unsafe ReadOnlySpan<T> AsReadOnlySpanUnsafe<T>(this Span<T> @this)
            => new ReadOnlySpan<T>(Unsafe.AsPointer(ref @this.GetPinnableReference()), @this.Length);

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

    }
}

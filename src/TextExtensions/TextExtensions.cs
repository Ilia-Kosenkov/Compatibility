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
using System.Text;

namespace TextExtensions
{
    public static class TextExtensions
    {
        public static int CountChars(this string @this, char symbol)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));
            if (@this.Length == 0)
                return 0;

            var counter = 0;
            for (var i = 0; i < @this.Length; i++)
                if (i == symbol)
                    counter++;
            return counter;
        }

        public static unsafe int GetByteCount(this Encoding encoding, ReadOnlySpan<char> chars)
        {
            fixed (char* ptr = chars)
                return encoding.GetByteCount(ptr, chars.Length);
        }

        public static unsafe int GetByteCount(this Encoding encoding, ReadOnlyMemory<char> chars)
        {
            using (var mem = chars.Pin())
                return encoding.GetByteCount((char*) mem.Pointer, chars.Length);
        }

        public static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            fixed(char* cPtr = chars)
            fixed (byte* bPtr = bytes)
                return encoding.GetBytes(cPtr, chars.Length, bPtr, bytes.Length);
        }

        public static unsafe int GetBytes(this Encoding encoding, ReadOnlyMemory<char> chars, Memory<byte> bytes)
        {
            using(var cMem = chars.Pin())
            using (var bMem = bytes.Pin())
                return encoding.GetBytes((char*) cMem.Pointer, chars.Length, (byte*) bMem.Pointer, bytes.Length);
        }

        public static unsafe int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
                return 0;
            fixed (byte* ptr = bytes)
                return encoding.GetCharCount(ptr, bytes.Length);
        }

        public static unsafe int GetCharCount(this Encoding encoding, ReadOnlyMemory<byte> bytes)
        {
            if (bytes.IsEmpty)
                return 0;
            using (var handle = bytes.Pin())
                return encoding.GetCharCount((byte*)handle.Pointer, bytes.Length);
            
        }

        public static unsafe int GetChars(this Encoding encoding, 
            ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            if (bytes.IsEmpty)
                return 0;

            fixed (byte* dataPtr = bytes)
                fixed (char* strPtr = chars)
                    return encoding.GetChars(dataPtr, bytes.Length, strPtr, chars.Length);
        }

        public static unsafe int GetChars(this Encoding encoding, 
            ReadOnlyMemory<byte> bytes, Memory<char> chars)
        {
            if (bytes.IsEmpty)
                return 0;

            using (var dataHandle = bytes.Pin())
                using (var strHandle = chars.Pin())
                    return encoding.GetChars((byte*) dataHandle.Pointer, bytes.Length, (char*) strHandle.Pointer,
                        chars.Length);
        }

        public static unsafe int GetCharCount(this Decoder decoder, 
            ReadOnlySpan<byte> bytes, bool flush)
        {
            if (bytes.IsEmpty)
                return 0;

            fixed (byte* ptr = bytes)
                return decoder.GetCharCount(ptr, bytes.Length, flush);
        }

        public static unsafe int GetCharCount(this Decoder decoder, 
            ReadOnlyMemory<byte> bytes, bool flush)
        {
            if (bytes.IsEmpty)
                return 0;
            using(var handle = bytes.Pin())
                return decoder.GetCharCount((byte*) handle.Pointer, bytes.Length, flush);
        }

        public static unsafe int GetChars(this Decoder decoder, 
            ReadOnlySpan<byte> bytes, Span<char> chars, bool flush)
        {
            if (bytes.IsEmpty)
                return 0;

            fixed (byte* dataPtr = bytes)
                fixed (char* strPtr = chars)
                    return decoder.GetChars(dataPtr, bytes.Length, strPtr, chars.Length, flush);
        }

        public static unsafe int GetChars(this Decoder decoder, 
            ReadOnlyMemory<byte> bytes, Memory<char> chars, bool flush)
        {
            if (bytes.IsEmpty)
                return 0;

            using (var dataHandle = bytes.Pin())
                using (var strHandle = chars.Pin())
                    return decoder.GetChars((byte*) dataHandle.Pointer, bytes.Length, (char*) strHandle.Pointer,
                        chars.Length, flush);
        }

        public static unsafe void Convert(this Decoder decoder, 
            ReadOnlySpan<byte> bytes, Span<char> chars, 
            bool flush,
            out int bytesUsed,
            out int charsUsed,
            out bool completed)
        {
            fixed(byte* dataPtr = bytes)
                fixed (char* strPtr = chars)
                    decoder.Convert(dataPtr, bytes.Length, strPtr, chars.Length, flush,
                        out bytesUsed, out charsUsed, out completed);
        }

        public static unsafe void Convert(this Decoder decoder,
            ReadOnlyMemory<byte> bytes, Memory<char> chars,
            bool flush,
            out int bytesUsed,
            out int charsUsed,
            out bool completed)
        {
            using (var dataHandle = bytes.Pin())
                using (var strHandle = chars.Pin())
                    decoder.Convert(
                        (byte*)dataHandle.Pointer, bytes.Length, 
                        (char*)strHandle.Pointer, chars.Length,
                        flush,
                        out bytesUsed, out charsUsed, out completed);
        }
    }
}
using System;
using System.Globalization;

namespace TextExtensions
{
    public static class Format
    {
        public static int GetSignificantDigitsCount(this byte value)
            => SignificantDigitsCount(value);

        public static int GetSignificantDigitsCount(this uint value)
            => SignificantDigitsCount(value);
        public static int GetSignificantDigitsCount(this ulong value)
            => SignificantDigitsCount(value);

        public static int GetSignificantDigitsCount(this sbyte value)
            => SignificantDigitsCount(value);

        public static int GetSignificantDigitsCount(this int value)
            => SignificantDigitsCount(value);

        public static int GetSignificantDigitsCount(this long value)
            => SignificantDigitsCount(value);

        public static bool TryFormat(this byte value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => UnsignedFormat(value, target, out charsWritten, provider);

        public static bool TryFormat(this uint value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => UnsignedFormat(value, target, out charsWritten, provider);

        public static bool TryFormat(this ulong value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => UnsignedFormat(value, target, out charsWritten, provider);


        public static bool TryFormat(this sbyte value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => SignedFormat(value, target, out charsWritten, provider);

        public static bool TryFormat(this int value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => SignedFormat(value, target, out charsWritten, provider);

        public static bool TryFormat(this long value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
            => SignedFormat(value, target, out charsWritten, provider);

        private static int SignificantDigitsCount(this ulong value)
        {
            var n = 0;
            while (value != 0)
            {
                value /= 10u;
                n++;
            }
            return n == 0 ? 1 : n;
        }

        private static int SignificantDigitsCount(this long value)
        {
            if (value < 0)
                value = -value;

            var n = 0;
            while (value != 0)
            {
                value /= 10u;
                n++;
            }
            return n == 0 ? 1 : n;
        }

        private static bool UnsignedFormat(this ulong value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
        {
            charsWritten = 0;

            var info = provider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo?? NumberFormatInfo.CurrentInfo;

            if (value == 0)
            {

                if (info.NativeDigits[0].AsSpan().TryCopyTo(target))
                {
                    charsWritten = info.NativeDigits[0].Length;
                    return true;
                }
                else
                    return false;
            }

            var ind = 0;

            var n = value.SignificantDigitsCount();

            Span<char> buff = stackalloc char[4 * n];

            while (value != 0)
            {
                var rem = value % 10u;
                var digit = info.NativeDigits[rem].AsSpan();

                digit.CopyTo(buff.Slice(ind));

                ind += digit.Length;

                value /= 10u;
            }

            if (ind >= target.Length)
                return false;

            for (var i = 0; i < ind; i++)
                target[i] = buff[ind - i - 1];

            charsWritten = ind;

            return true;
        }

        private static bool SignedFormat(this long value, Span<char> target, out int charsWritten, IFormatProvider provider = null)
        {
            charsWritten = 0;

            var info = provider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? NumberFormatInfo.CurrentInfo;


            if (value == 0)
            {
                if (info.NativeDigits[0].AsSpan().TryCopyTo(target))
                {
                    charsWritten = info.NativeDigits[0].Length;
                    return true;
                }

                return false;
            }

            var workVal = value;
            var isNegative = false;

            if (value < 0)
            {
                workVal = -value;
                isNegative = true;
            }


            var ind = 0;
            
            var n = workVal.SignificantDigitsCount();

            Span<char> buff = stackalloc char[4 * n];

            while (workVal != 0)
            {
                var rem = workVal % 10u;
                var digit = info.NativeDigits[rem].AsSpan();

                digit.CopyTo(buff.Slice(ind));

                ind += digit.Length;

                workVal /= 10u;
            }


            var offset = 0;
            if (isNegative)
            {
                var signSpan = info.NegativeSign.AsSpan();
                if (!signSpan.TryCopyTo(target))
                    return false;
                offset = signSpan.Length;
            }

            if (ind >= target.Length - offset)
                return false;

            for (var i = 0; i < ind; i++)
                target[i + offset] = buff[ind - i - 1];

            charsWritten = ind + offset;

            return true;
        }
        
    }
}

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

namespace IndexRange
{
    public readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public static Index End { get; } = FromEnd(0);

        public static Index Start { get; } = FromStart(0);


        public bool IsFromEnd => _value < 0;
        public int Value => _value < 0
            ? -(_value + 1)
            : _value;

        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
            _value = fromEnd ? -value - 1 : value;
        }

        public bool Equals(Index other)
            => _value == other._value;

        public override bool Equals(object value)
            => value is Index other && Equals(other);

        public override int GetHashCode()
            => _value.GetHashCode();

        public int GetOffset(int length)
            => IsFromEnd
                ? length + _value + 1
                : _value; 


        public static Index FromEnd(int value)
            => new Index(value, true);

        public static Index FromStart(int value)
            => new Index(value);

        public static implicit operator Index(int value)
            => value >= 0 ? new Index(value) : new Index(-value, true);

        public override string ToString() => $"{(IsFromEnd ? "^" : "")}{Value}";

    }
}

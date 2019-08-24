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
    public readonly struct Range : IEquatable<Range>
    {
        public readonly struct OffsetAndLength
        {
            public int Length { get; }
            public int Offset { get; }

            public OffsetAndLength(int offset, int length)
            {
                if (offset < 0)
                    throw new ArgumentException("Offset cannot be negative.", nameof(offset));
                if (length <= 0)
                    throw new ArgumentException("Length should be positive.", nameof(length));

                Offset = offset;
                Length = length;
            }

            public void Deconstruct(out int offset, out int length)
            {
                offset = Offset;
                length = Length;
            }
        }
        public static Range All { get; } = new Range(Index.Start, Index.End);

        public Index End { get; }
        public Index Start { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }
        
        public bool Equals(Range other)
            => Start.Equals(other.Start) && End.Equals(other.End);
        public override bool Equals(object value)
            => value is Range other && Equals(other);

        // Adopted from https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Metadata/src/System/Reflection/Internal/Utilities/Hash.cs#L11
        public override int GetHashCode()
            => unchecked(Start.GetHashCode() * (int)0xA5555529 + End.GetHashCode());

        public override string ToString() => $"[{Start}..{End}]";

        public OffsetAndLength GetOffsetAndLength(int length)
        {
            var start = Start.GetOffset(length);
            var end = End.GetOffset(length);
            return new OffsetAndLength(start, end - start);
        }
        
        public static Range StartAt(Index start) => new Range(start, Index.End);
        public static Range EndAt(Index end) => new Range(Index.Start, end);

        public static implicit  operator Range((Index Start, Index End) @this)
            => new Range(@this.Start, @this.End);
    }
}

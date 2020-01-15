using System;
using System.Collections;
using System.Collections.Generic;

namespace IndexRangeExtensions
{
    public struct RangeEnumerator : IEnumerator<int>
    {
        private readonly int _offset;
        private readonly int _length;
        private int _current;

        public RangeEnumerator(Range range, int length = 1)
        {
            (_offset, _length) = range.GetOffsetAndLength(length);
            _current = -1;
        }

        public bool MoveNext()
        {
            if (_current >= _length) return false;
            _current++;
            return true;
        }

        public void Reset()
        {
            _current = -1;
        }

        public int Current => _offset + _current;

        object IEnumerator.Current => Current;

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
        {
        }
#pragma warning restore CA1063 // Implement IDisposable Correctly
    }
}

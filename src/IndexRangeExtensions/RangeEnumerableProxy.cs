using System;
using System.Collections;
using System.Collections.Generic;

namespace IndexRangeExtensions
{
    public struct RangeEnumerableProxy : IEnumerable<int>
    {
        private readonly Range _range;
        private readonly int _length;

        public RangeEnumerableProxy(Range range, int length)
        {
            _range = range;
            _length = length;
        }

        public RangeEnumerator GetEnumerator() => new RangeEnumerator(_range, _length);

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

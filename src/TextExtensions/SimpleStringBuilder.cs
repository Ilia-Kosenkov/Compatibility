using System;
using System.Buffers;

#nullable enable

namespace TextExtensions
{
    public struct SimpleStringBuilder : IDisposable
    {
        private const int DefaultSize = 64;
        private char[] _borrowedArray;
        //private Span<char> _bufferView;
        private int _offset;

        public bool IsDisposed => _borrowedArray is null;
        public int Capacity => _borrowedArray.Length;

        public SimpleStringBuilder(int capacity)
        {
            if (capacity <= 0)
                capacity = DefaultSize;

            _borrowedArray = ArrayPool<char>.Shared.Rent(capacity);
            _offset = 0;
        }

        public void Append(char symbol)
        {
            if(_offset >= _borrowedArray.Length)
                GrowTo(2 * _borrowedArray.Length);

            _borrowedArray[_offset++] = symbol;
        }

        public void Append(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
                return;
            if(_offset + span.Length > _borrowedArray.Length)
                GrowTo(Math.Max(2 * _borrowedArray.Length, _offset + span.Length));

            span.CopyTo(_borrowedArray.AsSpan(_offset));
            _offset += span.Length;
        }

        public void Append(params char[]? array)
        {
            if (array is null || array.Length == 0)
                return;

            if (_offset + array.Length > _borrowedArray.Length)
                GrowTo(Math.Max(2 * _borrowedArray.Length, _offset + array.Length));

            array.CopyTo(_borrowedArray.AsSpan(_offset));
            _offset += array.Length;
        }

        public void Append(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            if (_offset + s.Length > _borrowedArray.Length)
                GrowTo(Math.Max(2 * _borrowedArray.Length, _offset + s.Length));

            s.AsSpan().CopyTo(_borrowedArray.AsSpan(_offset));
            _offset += s.Length;
        }

        public ReadOnlySpan<char> View() => _borrowedArray.AsSpan(0, _offset);

        public bool TryMoveTo(Span<char> target)
        {
            if (!View().TryCopyTo(target))
                return false;
            Clear();
            return true;
        }

        public void Clear()
        {
            _offset = 0;
        }

        public void Dispose()
        {
            if (_borrowedArray is null)
                return;
            ArrayPool<char>.Shared.Return(_borrowedArray, true);
        }

        public override string ToString()
            => _offset <= 0
                ? string.Empty
                : View().ToString();

        private void GrowTo(int newCapacity)
        {
            if (newCapacity <= Capacity) return;

            var newArr = ArrayPool<char>.Shared.Rent(newCapacity);
            _borrowedArray.AsSpan(0, _offset).CopyTo(newArr);
            ArrayPool<char>.Shared.Return(_borrowedArray, true);
            _borrowedArray = newArr;
        }
    }
}

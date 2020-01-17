using System;

#nullable enable

namespace TextExtensions
{
    public ref struct FixedStringBuilder
    {
        private readonly Span<char> _buffer;
        private int _offset;
        private readonly bool _clearOnDispose;
        public int Capacity => _buffer.Length;

        public FixedStringBuilder(Span<char> preAllocatedBuffer)
        {
            _buffer = preAllocatedBuffer;
            _offset = 0;
            _clearOnDispose = false;
        }

        public FixedStringBuilder(Span<char> preAllocatedBuffer, bool clearOnDispose)
        {
            _buffer = preAllocatedBuffer;
            _offset = 0;
            _clearOnDispose = clearOnDispose;
        }

        public bool TryAppend(char symbol)
        {
            if (_offset >= _buffer.Length)
                return false;

            _buffer[_offset++] = symbol;
            return true;
        }
        public bool TryAppend(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
                return true;
            if (_offset + span.Length > _buffer.Length)
                return false;

            span.CopyTo(_buffer.Slice(_offset));
            _offset += span.Length;
            return true;
        }
        public bool TryAppend(params char[]? array)
        {
            if (array is null || array.Length == 0)
                return true;

            if (_offset + array.Length > _buffer.Length)
                return false;
            
            array.CopyTo(_buffer.Slice(_offset));
            _offset += array.Length;

            return true;
        }
        public bool TryAppend(string? s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            if (_offset + s.Length > _buffer.Length)
                return false;

            s.AsSpan().CopyTo(_buffer.Slice(_offset));
            _offset += s.Length;

            return true;
        }
        
        public void DeleteBack(int count = 1) 
            => _offset = Math.Max(_offset - count, 0);

        public void Clear() => _offset = 0;

        public ReadOnlySpan<char> View() => _buffer;

        public bool TryMoveTo(Span<char> target)
        {
            if (!_buffer.TryCopyTo(target))
                return false;
            Clear();
            return true;
        }

        public override string ToString()
            => _offset <= 0
                ? string.Empty
                : _buffer.ToString();

        public void Dispose()
        {
            if(_clearOnDispose)
                _buffer.Fill('\0');
        }
    }
}

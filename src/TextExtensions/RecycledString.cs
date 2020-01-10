using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TextExtensions
{
    public class RecycledString
    {
        private readonly string _s;

        public ReadOnlySpan<char> View => _s.AsSpan();

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public string StringView => _s;

        public RecycledString(int size)
        {
            if (size < 0)
                throw new ArgumentException(nameof(size));
            _s = new string('\0', size);
        }

        public bool TryCopy(ReadOnlySpan<char> what, int startFrom = 0)
        {
            if (what.Length + startFrom > _s.Length)
                return false;

            ref var rf = ref Unsafe.AsRef(
                MemoryMarshal.GetReference(
                    MemoryMarshal.Cast<char, byte>(_s.AsSpan(startFrom))));

            ref var payload = ref MemoryMarshal.GetReference(
                MemoryMarshal.Cast<char, byte>(what));

            Unsafe.CopyBlock(ref rf, ref payload, (uint) (what.Length * sizeof(char)));

            return true;
        }

        public bool TryCopy(string what, int startFrom = 0)
            => TryCopy(what.AsSpan(), startFrom);

        public void Fill(char clearSymb = '\0')
        {
            var arr = ArrayPool<char>.Shared.Rent(_s.Length);
            try
            {
                var span = arr.AsSpan(0, _s.Length);
                span.Fill(clearSymb);
                TryCopy(span);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(arr);
            }
        }

        public void Clear() => Fill();

        public string ProxyAsString(ReadOnlySpan<char> what, char clearSymb = '\0')
        {
            Fill(clearSymb);
            return !TryCopy(what) ? null : _s;
        }
    }
}
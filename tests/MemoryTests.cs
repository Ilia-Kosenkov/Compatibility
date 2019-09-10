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
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MemoryExtensions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class MemoryTests
    {
        private string _input;
        private char[] _inputArr;

        [SetUp]
        public void SetUp()
        {
            _input = " \t    1111    klfhakljfh01928091-__IU1IJHJK  1JHR KJ       \t 1111   ";
            _inputArr = _input.ToCharArray();
        }

        [Test]
        public void Test_Trim()
        {
            var s1 = _input.Trim();
            var s2 = _inputArr.AsReadOnlySpan().Trim().ToString();
            var s3 = _inputArr.AsReadOnlyMemory().Trim().ToString();

            Assert.AreEqual(s1, s2);
            Assert.AreEqual(s1, s3);
        }

        [Test]
        //[Repeat(15)]
        public unsafe void Test_DangerousAs()
        {
            void FlipBytes(Span<long> inBytes)
            {
                var bytes = inBytes.DangerousAs<long, byte>();
                Write<byte, long>(bytes, nameof(FlipBytes));
                var buff = bytes[0];
                bytes[0] = bytes[3];
                bytes[3] = buff;

                buff = bytes[1];
                bytes[1] = bytes[2];
                bytes[2] = buff;
                Write<byte, long>(bytes, nameof(FlipBytes));
            }


            var u = new Union();

            u.Span_1 = new long[] {123}.AsSpan();
            //var array = new[] {123};
            //Span<long> val = new long[] {123};
            var val = u.Span_1;
            var sp = u.Span_2;



            //fixed(long* pptr = &array[0])
            //    val = new Span<long>(pptr, 1);

            //var h = GCHandle.Alloc(array, GCHandleType.Pinned);


            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            Assert.AreEqual(123, val[0]);
            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            FlipBytes(val);

            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            Assert.AreNotEqual(123, val[0]);

            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            FlipBytes(val);

            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            Assert.AreEqual(123, val[0]);

            Write<long, long>(val, nameof(val));
            Write<byte, long>(sp, nameof(sp));

            //h.Free();
        }

        public unsafe void Test_Unsafe()
        {
            Span<int> x = new int[] {999, 123, 11};
            var rf = new RF();
            rf.span = x;
            Write<int, int>(x);
            fixed (int* ptr = x)
            {
                var p = (void*) ptr;
                Write<int>(p);
                p = Unsafe.Add<object>(p, 1);
                Write<int>(p);

                fixed (RF* rfptr = &rf)
                {

                }
            }

            GC.Collect(2, GCCollectionMode.Forced, true, true);
            
            Write<int, int>(x);

        }

        private static unsafe object ToString(void* ptr)
            =>
                $"0x{new IntPtr(ptr).ToInt64():X8}:";

        private static unsafe void Write<T, TVal>(Span<T> span, string message = "") where T : unmanaged where TVal : unmanaged
        {
            var str =
                $"{ToString(Unsafe.AsPointer(ref span[0]))}" +
                $"  {Unsafe.ReadUnaligned<TVal>(Unsafe.AsPointer(ref span[0])), 10} {message}";

            Console.WriteLine(str);
        }

        private static unsafe void Write<TVal>(void* ptr, string message = "")  where TVal : unmanaged
        {
            var str =
                $"{ToString(ptr)}" +
                $"  {Unsafe.ReadUnaligned<TVal>(ptr),10} {message}";

            Console.WriteLine(str);
        }

        private static void Dummy(bool arg)
        {
            if (arg)
                throw new Exception();
        }

        [StructLayout(LayoutKind.Explicit)]
        private ref struct Union//<T, U> where T : unmanaged where U : unmanaged
        {
            [FieldOffset(0)]
            public Span<long> Span_1;
            [FieldOffset(0)]
            public Span<byte> Span_2;

            [FieldOffset(0)]
            public PTR P;
        }

        private struct PTR
        {
            public readonly object o;
            public readonly IntPtr P1;
            public readonly int P2;
        }

        private ref struct RF
        {
            public Span<int> span;
        }

        public static void Main()
        {
            new MemoryTests().Test_Unsafe();
        }
    }
}

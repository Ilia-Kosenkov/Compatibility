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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Compatibility.ITuple;

namespace Tests
{
    [TestFixture]
    public class TupleTests
    {
        public static IEnumerable<object> TestCaseSource 
        {
            get
            {
                yield return ValueTuple.Create(false);
                yield return ("string", 1.54d);
                yield return (-43, "string", 1.32f);
                yield return (-43, "string", 1.32, false);
                yield return (-43, "string", 1.32f, 'a', true);
                yield return (-43, "string", 1.32f, new object(), new object(), new object());
                yield return (-43, "string", 1.32f, new object(), new object(), new object(), IntPtr.Zero);
                yield return (-43, "string", 1.32f, new object(), new object(), new object(), IntPtr.Zero, UIntPtr.Zero);
                yield return (10500, 42, -43, "string", 1.32f, new object(), new object(), new object(), IntPtr.Zero, UIntPtr.Zero);

            }

        }

        [TestCaseSource(nameof(TestCaseSource))]
        [Test]
        public void Test(object values)
        {
            
            var sysT = values as ITuple;
            var cuT = values.IsValueTuple();

            Assert.NotNull(sysT);
            Assert.NotNull(cuT);
            Assert.AreEqual(cuT.Length, sysT.Length);

            for (var i = 0; i < cuT.Length; i++)
                Assert.AreEqual(cuT[i], sysT[i]);

            Assert.That(() => cuT[-1], Throws.InstanceOf<IndexOutOfRangeException>());
            Assert.That(() => cuT[cuT.Length], Throws.InstanceOf<IndexOutOfRangeException>());
        }

        [Test]
        public void Test_Fails()
        {
            Assert.IsNull(((object)null).IsValueTuple());
            Assert.IsNull(5.IsValueTuple());
        }
    }

}

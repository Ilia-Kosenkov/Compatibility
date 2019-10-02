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
using IndexRange;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class IndexTests
    {
        [Test]
        public void Test_IndexThrows()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => new Index(-10));
        }

        [Test]
        public void Test_IndexOffset()
        {
            var i1 = Index.FromStart(4);
            var i2 = Index.FromEnd(10);
            Assert.AreEqual(i1.GetOffset(14), i2.GetOffset(14));
        }

        [Test]
        public void Test_IndexEquality()
        {
            var i1 = Index.FromStart(4);
            var i2 = (Index) 4;
            var i3 = Index.FromEnd(20);
            var i4 = (Index)( - 20);

            Assert.IsTrue(i1.Equals(i2));
            Assert.IsTrue(i4.Equals(i3));

            Assert.IsFalse(i1.Equals(i3));
            Assert.IsFalse(i2.Equals(i4));
        }

    }
}

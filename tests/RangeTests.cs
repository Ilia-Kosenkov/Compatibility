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
using MemoryExtensions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RangeTests
    {
        private byte[] _testArr;
        private int Length => _testArr.Length;

        [SetUp]
        public void SetUp()
        {
            _testArr = new byte[512];
            new Random().NextBytes(_testArr);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Test_Boundaries()
        {
            var (offset, length) = Range.All.GetOffsetAndLength(Length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(Length, length);

            (offset, length) = Range.StartAt(63).GetOffsetAndLength(Length);
            Assert.AreEqual(63, offset);
            Assert.AreEqual(Length - 63, length);

            (offset, length) = Range.EndAt(63).GetOffsetAndLength(Length);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(63, length);

            (offset, length) = new Range(10, -20).GetOffsetAndLength(Length);
            Assert.AreEqual(10, offset);
            Assert.AreEqual(Length - 20 - 10, length);

        }

        [Test]
        public void Test_IsValid()
        {
            Assert.IsTrue(new Range(10, 20).IsValidRange(30));
            Assert.IsTrue(new Range(10, 20).IsValidRange(20));
            Assert.IsFalse(new Range(10, 20).IsValidRange(19));

            Assert.IsFalse(new Range(10, -20).IsValidRange(30));
            Assert.IsTrue(new Range(10, -19).IsValidRange(30));

            Assert.IsTrue(new Range(-20, -10).IsValidRange(30));
            Assert.IsTrue(Range.All.IsValidRange(1));
        }

        [Test]
        public void Test_SubSequenceEquality()
        {
            var span = _testArr.AsSpan(20, 100);
            var range = new Range(20, -(Length - 120));

            var span2 = _testArr.AsSpan().Slice(range);

            Assert.IsTrue(span.SequenceEqual(span2));
        }


        [Test]
        [TestCase(11)]
        [TestCase(20)]
        [TestCase(35)]
        public void Test_Arithmetic(int length)
        {
            Index i = 4;
            var j = Index.FromEnd(5);
            var newInd = i.Add(j, length);
            Assert.AreEqual(length - 1, newInd.GetOffset(length));

            newInd -= 5;

            Assert.AreEqual(length - 6, newInd.GetOffset(length));

            newInd = newInd.Subtract(Index.FromEnd(length - 1), length);

            Assert.AreEqual(length - 7, newInd.GetOffset(length));

        }

        [Test]
        public void Test_ToString()
        {
            var i1 = Index.FromStart(23);
            var i2 = Index.FromEnd(23);
            Assert.AreEqual("23", i1.ToString());
            Assert.AreEqual("^23", i2.ToString());

            var r1 = Range.All;
            Range r2 = (i1, i2);

            Assert.AreEqual("[0..^0]", r1.ToString());
            Assert.AreEqual($"[{i1}..{i2}]", r2.ToString());
        }

        [Test]
        public void Test_Equals()
        {
            Assert.True(Index.FromStart(5) == 5);
            Assert.True(Index.FromEnd(5) != 5);

            object o = Index.FromStart(10);

            Assert.True(((Index)10).Equals(o));

            Assert.False(((Index) 10).Equals(null));

            Assert.True(Range.EndAt(Index.FromEnd(10)) == (0, Index.FromEnd(10)));
            Assert.True(Range.EndAt(Index.FromEnd(10)) != (0, Index.FromStart(10)));

            o = Range.All;

            Assert.True(Range.All.Equals(o));
            Assert.False(Range.All.Equals(null));

        }

        [Test]
        public void Test_Hash()
        {
            Assert.AreEqual(100.GetHashCode(), Index.FromStart(100).GetHashCode());
            Assert.AreNotEqual(100.GetHashCode(), Index.FromEnd(100).GetHashCode());

            Assert.AreEqual(((Range)(0, Index.End)).GetHashCode(), Range.All.GetHashCode());
        }

        [Test]
        public void Test_OffsetAndLength_Throws()
        {
            Assert.That(() => new Range.OffsetAndLength(-10, 10), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => new Range.OffsetAndLength(10, -10), Throws.InstanceOf<ArgumentException>());
        }

    }
}

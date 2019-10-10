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
using System.Linq;
using Maybe;
using NUnit.Framework;

namespace Tests
{
    using static MaybeExtensions;
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void Test_Linq()
        {

            Maybe<int> a = 5.Some();
            Maybe<int> b = 0.Some();
            Maybe<int> c = None.Get;

            var r1 = a.Select(x => x + 5).Select(y => y - 4).Select(z => z * z);
            var r2 = b.Select(x => x + 5).Select(y => y - 4).Select(z => z * z);
            var r3 = c.Select(x => x + 5).Select(y => y - 4).Select(z => z * z);

            var q1 = r1.Match(0);
            var q2 = r2.Match(0);
            var q3 = r3.Match(0);

            Assert.That(q1, Is.EqualTo(36));
            Assert.That(q2, Is.EqualTo(1));
            Assert.That(q3, Is.EqualTo(0));


            Assert.That(r1.Match(0), Is.EqualTo(r1.Match(0)));
            Assert.That(r2.Match(0), Is.EqualTo(r2.Match(0)));
            Assert.That(r3.Match(0), Is.EqualTo(r3.Match(0)));

            var val = from i in r1
                      from j in r2
                      select i - j;

            Assert.That(val.Match(0), Is.EqualTo(35));
        }

        [Test]
        public void Test_Methods()
        {

            bool Tester(int x)
            {
                return x <= 100;
            }

            var a = new Maybe<int>(50);
            var b = new Maybe<int>(100);
            var c = new Maybe<int>(150);

            a = a.Where(Tester).Where(x => x > 60);
            b = b.Where(Tester).Where(x => x > 60);
            c = c.Where(Tester).Where(x => x > 60);

            var aa = a.Match(-1);
            var bb = b.Match(-1);
            var cc = c.Match(-1);

            Assert.That(aa, Is.EqualTo(-1));
            Assert.That(bb, Is.EqualTo(100));
            Assert.That(cc, Is.EqualTo(-1));


        }

        [Test]
        public void Test_Collections()
        {
            var r = new Random();

            var bytes = new byte[32];
            r.NextBytes(bytes);
            var collection = bytes.SelectSome<byte, int>(x => x).Where(x => x > 200).ToList();

            var result = collection.Match(200).ToList();

            foreach(var item in result)
                Assert.That(item, Is.GreaterThanOrEqualTo(200));
        }

        [Test]
        public void Test_NoneMatches()
        {
            Assert.That(() => Maybe<int>.None.Match(new InvalidOperationException()), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void Test_ArgumentNullThrows()
        {
            Assert.That(() => 5.Some().Select<int>(null), Throws.ArgumentNullException);
            Assert.That(() => 5.Some().Where(null), Throws.ArgumentNullException);
            Assert.That(() => Maybe<int>.None.Match<Exception>(null), Throws.ArgumentNullException);
            Assert.That(() => 5.Some().Match<int>(null), Throws.ArgumentNullException);

        }

        [Test]
        public void Test_ElementAtOrNone()
        {
            Assert.That(() => MaybeExtensions.ElementAtOrNone<int>(null, 0), Throws.ArgumentNullException);
            Assert.That(() => Array.Empty<int>().SelectSome(x => x).ElementAtOrNone(-10), Throws.ArgumentException);

            var coll = new[] {12, 12351, 123, 11, 0, -88};

            Assert.AreEqual(coll[3], coll.ElementAtOrNone(3).Match(int.MinValue));
            Assert.That(() => coll.ElementAtOrNone(6).Match(new IndexOutOfRangeException()),
                Throws.InstanceOf<IndexOutOfRangeException>());

        }
        [Test]
        public void Test_FirstOrNone()
        {
            Assert.That(() => MaybeExtensions.FirstOrNone<int>(null, null), Throws.ArgumentNullException);
            Assert.That(() => Array.Empty<int>().FirstOrNone(null), Throws.ArgumentNullException);

            var coll = new[] { 12, 12351, 123, 11, 0, -88 };

            Assert.AreEqual(coll[0], coll.FirstOrNone(x => true).Match(int.MinValue));
            Assert.That(() => Array.Empty<int>().FirstOrNone(x => false).Match(new IndexOutOfRangeException()),
                Throws.InstanceOf<IndexOutOfRangeException>());

        }

    }
}

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
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using Compatibility.Bridge;
using Compatibility.Bridge.Internal;
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

            LazyMaybe<int> a = Some(5);
            LazyMaybe<int> b = Some(0);
            LazyMaybe<int> c = None;

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

            var a = new LazyMaybe<int>(50);
            var b = new LazyMaybe<int>(100);
            var c = new LazyMaybe<int>(150);

            a = a.Where(Tester).WhereExpression(x => x > 60);
            b = b.Where(Tester).WhereExpression(x => x > 60);
            c = c.Where(Tester).WhereExpression(x => x > 60);

            var aa = a.Match(-1);
            var bb = b.Match(-1);
            var cc = c.Match(-1);
        }

        [Test]
        public void Test_Collections()
        {
            var r = new Random();

            var bytes = new byte[32];
            r.NextBytes(bytes);
            var collection = bytes.SelectMaybeLazy<byte, int>(x => x).WhereMaybe(x => x > 200).ToList();

            var result = collection.MatchMaybe(0).ToList();

        }

        [Test]
        public async Task Test()
        {
            var temp = new LazyMaybe<LazyMaybe<int>>(new LazyMaybe<int>(5));

            var n = await from x in temp
                          from y in x
                          from z in Some(23)
                          select y + z;

            Assert.That(() => n.Match(new Exception()), Throws.Nothing);

            var first = new LazyMaybe<int>(Task.Delay(1000).ContinueWith(x => new Maybe<int>(20)));
            var second = new LazyMaybe<int>(Task.Delay(2000).ContinueWith(x => new Maybe<int>(30)));

            first = first.Select(x => x / 10).Select(x => x + 1);
            second = second.Select(x => x / 10).Select(x => x + 3);

            var delayedResult = 
                from x in first
                from y in second
                select x * y;

            var result = await delayedResult;

            Assert.That(delayedResult.Match(-1), Is.EqualTo(18));

        }

        [Test]
        public async Task Example()
        {
            var x = new LazyMaybe<int>(Task.Run(() => Some(42)));
            var y = Some(21);

            var result = await from i in x
                               from j in y
                               where i > 40
                               where j > 20
                               select i / j;

            Assert.That(result.Match(0), Is.EqualTo(2));
        }
    }
}

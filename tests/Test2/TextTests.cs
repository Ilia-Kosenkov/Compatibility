using System;
using NUnit.Framework;
using TextExtensions;

namespace Tests
{
    [TestFixture]
    public class TextTests
    {
        [SetUp]
        public void SetUp()
        {

        }


        [Test]
        public void Test_Digits()
        {
            Assert.That(((byte)255).GetSignificantDigitsCount(), Is.EqualTo(3));
            Assert.That(((byte)25).GetSignificantDigitsCount(), Is.EqualTo(2));
            Assert.That(((byte)5).GetSignificantDigitsCount(), Is.EqualTo(1));
            Assert.That(((byte)0).GetSignificantDigitsCount(), Is.EqualTo(1));

            var buff = new char[80];

            Assert.That(((byte)205).TryFormat(buff, out var n), Is.True);

            Assert.That(1234567890u.TryFormat(buff.AsSpan().Slice(4), out n), Is.True);
            Assert.That((-1234567890).TryFormat(buff.AsSpan().Slice(16), out n), Is.True);

        }
    }
}

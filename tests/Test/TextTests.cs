using System;
using System.Diagnostics.CodeAnalysis;
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

        [Test]
        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        public void Test_StringBuilder()
        {
            using var sb = new SimpleStringBuilder(100);
            sb.Append(' ');
            sb.Append('1');
            sb.Append('2');
            sb.Append('3');
            sb.Append('4');
            sb.Append("Some text");
            sb.Append(ReadOnlySpan<char>.Empty);
            sb.Append((string)null);
            sb.Append('W', 'T', 'F');

            var s = sb.ToString();

            Assert.AreEqual(" 1234Some textWTF", s);

            Assert.IsFalse(sb.View().IsEmpty);
            sb.Clear();
            Assert.IsTrue(sb.View().IsEmpty);
        }

        [Test]
        [SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
        public void Test_StackStringBuilder()
        {
            using var sb = new FixedStringBuilder(stackalloc char[32]);
            sb.TryAppend(' ');
            sb.TryAppend('1');
            sb.TryAppend('2');
            sb.TryAppend('3');
            sb.TryAppend('4');
            sb.TryAppend("Some text");
            sb.TryAppend(ReadOnlySpan<char>.Empty);
            sb.TryAppend((string)null);
            sb.TryAppend('W', 'T', 'F');

            var s = sb.ToString();

            Assert.AreEqual(" 1234Some textWTF", s);

            Assert.IsFalse(sb.View().IsEmpty);
            sb.Clear();
            Assert.IsTrue(sb.View().IsEmpty);
        }

        [Theory]
        public void Test_EmptyStringBuilder()
        {
            var sb = new SimpleStringBuilder();
            Assume.That(sb.IsDisposed, Is.True);
            Assert.That(() => sb.Capacity, Throws.InstanceOf<NullReferenceException>());
            Assert.That(() => sb.Dispose(), Throws.Nothing);
        }
    }
}

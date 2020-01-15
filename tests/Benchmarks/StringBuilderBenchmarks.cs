using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using TextExtensions;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    public class StringBuilderBenchmarks
    {
        [Params(10, 100, 1_000, 10_000, 100_000)]
        public int N;
        public string _str;

        public char X { get; } = 'B';

        [Benchmark(Baseline = true)]
        public void StringBuilder_AppendChar()
        {
            var builder = new StringBuilder(10 * N);
            var c = X;
            for (var i = 0; i < N; i++)
            {
                builder.Append(c);
            }

            _str = builder.ToString();
        }

        [Benchmark]
        public void String_AppendChar()
        {
            var builder = @"";
            var c = X;
            for (var i = 0; i < N; i++)
            {
                builder += c;
            }

            _str = builder;
        }

        [Benchmark]
        public void SimpleStringBuilder_AppendChar()
        {
            using var builder = new SimpleStringBuilder(10 * N);
            var c = X;

            for (var i = 0; i < N; i++)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                builder.Append(c);
            }

            _str = builder.ToString();
        }
        [Benchmark]
        public void SimpleStringBuilder_AppendChar_Dispose()
        {
            var builder = new SimpleStringBuilder(10 * N);
            try
            {
                var c = X;

                for (var i = 0; i < N; i++)
                {
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                    builder.Append(c);
                }

                _str = builder.ToString();
            }
            finally
            {
                builder.Dispose();
            }
        }

    }
}

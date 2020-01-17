using System;
using System.Buffers;
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
        [Params(10, 31, 32, 33, 63, 64, 65, 127, 128, 129)]
        public int N;
        public string _str;

        public char X { get; } = 'B';

        [Benchmark(Baseline = true)]
        public void StringBuilder_AppendChar()
        {
            var builder = new StringBuilder(N + 4);
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
            using var builder = new SimpleStringBuilder(N + 4);
            var c = X;

            for (var i = 0; i < N; i++)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                builder.Append(c);
            }

            _str = builder.ToString();
        }

        [Benchmark]
        public void FixedStringBuilder_AppendChar()
        {
           
            using var builder = new FixedStringBuilder(stackalloc char[N + 4]);
            var c = X;

            for (var i = 0; i < N; i++)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                builder.TryAppend(c);
            }

            _str = builder.ToString();
            
        }
        [Benchmark]
        public void SimpleStringBuilder_AppendChar_Dispose()
        {
            var builder = new SimpleStringBuilder(N + 4);
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

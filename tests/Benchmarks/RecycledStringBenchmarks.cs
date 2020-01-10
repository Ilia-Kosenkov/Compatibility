using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using TextExtensions;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    public class RecycledStringBenchmarks
    {
        private char[] _data;

        [Params(1000)]
        public int N;

        [Params(1000, 10_000)]
        public int M;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = new char[10 * N];

            for (var i = 0; i < N; i++)
                _data.AsSpan(10 * i, 10).Fill((char)('A' + i % 50));
        }

        [Benchmark(Baseline = true)]
        public void MangedString()
        {
            string s = null;

            for (var i = 0; i < M; i++)
            {
                var id = i % N;
                s = _data.AsSpan(10 * id, 10).ToString();
            }
        }

        [Benchmark]
        public void RecycledString()
        {

            var rs = new RecycledString(10);

            for (var i = 0; i < M; i++)
            {
                var id = i % N;
                rs.TryCopy(_data.AsSpan(10 * id, 10));
            }
        }
    }
}

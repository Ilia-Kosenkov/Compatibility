using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using TextExtensions;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    public class StringConcatBenchmarks
    {
        public string s = @";Lorem ipsum dolor sit amet, consectetur adipiscing";
        public char c = ' ';

        [Benchmark(Baseline = true)]
        public string String_Add()
        {
            return c + s;
        }

        [Benchmark]
        public string StringBuilder()
        {
            return new StringBuilder(s.Length + 1).Append(c).Append(s).ToString();
        }

        [Benchmark]
        public string SimpleStringBuilder()
        {
            using (var builder = new SimpleStringBuilder(s.Length + 1))
            {
                builder.Append(c);
                builder.Append(s);
                return builder.ToString();
            }
        }

        [Benchmark]
        public string FixedStringBuilder()
        {
            using (var builder = new FixedStringBuilder(stackalloc char[s.Length + 8]))
            {
                builder.TryAppend(c);
                builder.TryAppend(s);
                return builder.ToString();
            }
        }
    }
}

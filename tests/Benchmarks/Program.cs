using BenchmarkDotNet.Running;

namespace Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //BenchmarkRunner.Run<StringConcatBenchmarks>();
            BenchmarkRunner.Run<StringBuilderBenchmarks>();
        }
    }
}

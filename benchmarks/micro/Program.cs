using BenchmarkDotNet.Running;
using System;

namespace micro
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CreationBenchmark>();
            BenchmarkRunner.Run<PredictionBenchmark>();
        }
    }
}

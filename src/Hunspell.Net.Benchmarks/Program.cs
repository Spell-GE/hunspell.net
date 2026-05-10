using BenchmarkDotNet.Running;
using Hunspell.Net.Benchmarks;

BenchmarkSwitcher.FromTypes([typeof(HunspellBenchmarks)]).Run(args);

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace Signals.Tests.Core.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfig config = DefaultConfig.Instance;

            if (Debugger.IsAttached) config = new DebugBuildConfig();

            BenchmarkRunner.Run(typeof(Program).Assembly, config
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddExporter(HtmlExporter.Default)
                );
        }
    }
}

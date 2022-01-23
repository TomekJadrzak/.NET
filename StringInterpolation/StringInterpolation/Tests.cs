using System.Text;
using BenchmarkDotNet.Attributes;

namespace StringInterpolation
{
    [MemoryDiagnoser]
    public class Tests
    {
        private int a = 6;
        private int b = 12;

        [Benchmark]
        public string StringConcat()
        {
            return string.Concat(
                "Zażółć ",
                a.ToString(),
                " gęślą ",
                b.ToString(),
                " jaźń");
        }

        [Benchmark]
        public string StringPlus()
        {
            return "Zażółć " + a + " gęślą " + b + " jaźń";
        }

        [Benchmark]
        public string StringFormat()
        {
            return string.Format("Zażółć {0} gęślą {1} jaźń", a, b);
        }

        [Benchmark]
        public string StringInterpolated()
        {
            return $"Zażółć {a} gęślą {b} jaźń";
        }

        [Benchmark]
        public string StringBuild()
        {
            StringBuilder builder = new(32);
            builder
                .Append("Zażółć ")
                .Append(a)
                .Append(" gęślą ")
                .Append(b)
                .Append(" jaźń");

            return builder.ToString();
        }
    }
}
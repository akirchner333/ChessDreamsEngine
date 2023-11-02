using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace EngineTest
{
    public class BenchmarkTest
    {
        private Board NolotOne = new Board("r3qb1k/1b4p1/p2pr2p/3n4/Pnp1N1N1/6RP/1B3PP1/1B1QR1K1 w - - 0 1");
        private Board NolotTwo = new Board("r4rk1/pp1n1p1p/1nqP2p1/2b1P1B1/4NQ2/1B3P2/PP2K2P/2R5 w - - 0 1");
        private Board NolotThree = new Board("r2qk2r/ppp1b1pp/2n1p3/3pP1n1/3P2b1/2PB1NN1/PP4PP/R1BQK2R w KQkq - 0 1");

        [GlobalSetup]
        public void Setup()
        {

        }

        [Benchmark]
        public void TestMovesOne() => NolotOne.GenerateMoves();
        [Benchmark]
        public void TestMovesTwo() => NolotTwo.GenerateMoves();
        [Benchmark]
        public void TestMovesThree() => NolotThree.GenerateMoves();
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>();
        }
    }
}

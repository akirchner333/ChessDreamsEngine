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
        private Move MoveOne;
        private Move MoveTwo;
        private Move MoveThree;

        [GlobalSetup]
        public void Setup()
        {
            MoveThree = new CastleMove(BitUtil.AlgebraicToBit("e1"), BitUtil.AlgebraicToBit("g1"), true)
            {
                RookStart = BitUtil.AlgebraicToBit("h1"),
                RookEnd = BitUtil.AlgebraicToBit("f1")
            };
            MoveTwo = new Move(BitUtil.AlgebraicToBit("b3"), BitUtil.AlgebraicToBit("f7"), true) { Capture = true };
            MoveOne = new Move(BitUtil.AlgebraicToBit("d1"), BitUtil.AlgebraicToBit("d4"), true) { Capture = true };
        }

        [Benchmark]
        public void TestMovesOne() => NolotOne.GenerateMoves();
        [Benchmark]
        public void TestMovesTwo() => NolotTwo.GenerateMoves();
        [Benchmark]
        public void TestMovesThree() => NolotThree.GenerateMoves();
        [Benchmark]
        public Move TestApplyOne() => NolotOne.ApplyMove(MoveOne);
        [Benchmark]
        public Move TestApplyTwo() => NolotTwo.ApplyMove(MoveTwo);
        [Benchmark]
        public Move TestApplyThree() => NolotThree.ApplyMove(MoveThree);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>();
        }
    }
}

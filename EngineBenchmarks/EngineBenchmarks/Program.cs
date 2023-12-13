using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Engine;

namespace EngineTest
{
    public class BenchmarkTest
    {
        private readonly Board NolotOne = new("r3qb1k/1b4p1/p2pr2p/3n4/Pnp1N1N1/6RP/1B3PP1/1B1QR1K1 w - - 0 1");
        private readonly Board NolotTwo = new("r4rk1/pp1n1p1p/1nqP2p1/2b1P1B1/4NQ2/1B3P2/PP2K2P/2R5 w - - 0 1");
        private readonly Board NolotThree = new("r2qk2r/ppp1b1pp/2n1p3/3pP1n1/3P2b1/2PB1NN1/PP4PP/R1BQK2R w KQkq - 0 1");

        [GlobalSetup]
        public void Setup()
        {

        }

        [Benchmark]
        public void TestMovesOne() => NolotOne.MoveArray();
        [Benchmark]
        public void TestMovesTwo() => NolotTwo.MoveArray();
        [Benchmark]
        public void TestMovesThree() => NolotThree.MoveArray();
        [Benchmark]
        public void TestEntire()
        {
            Span<Move> moves = new Move[218];
            NolotOne.Moves(ref moves);
            var fullMove = NolotOne.ApplyMove(moves[0]);
            NolotOne.ReverseMove(fullMove);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkTest>();
            //Board NolotOne = new("r3qb1k/1b4p1/p2pr2p/3n4/Pnp1N1N1/6RP/1B3PP1/1B1QR1K1 w - - 0 1");
            //var moves = NolotOne.Moves();
            //for (var i = 0; i < 1000; i++)
            //{
            //    foreach(var move in moves)
            //    {
            //        var fullMove = NolotOne.ApplyMove(move);
            //        NolotOne.ReverseMove(fullMove);
            //    }
            //}
            
            //Console.WriteLine("Yeah!");
        }
    }
}

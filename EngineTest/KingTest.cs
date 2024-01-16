using Engine;

namespace EngineTest
{
    [TestClass]
    public class KingTest : PieceTest
    {
        [TestMethod]
        public void TestMoves()
        {
            var b = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
            var king = new King("d3", Sides.White);
            var targets = king.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(8, targets.Count());

            string[] expected = new string[] { "d3c2", "d3c3", "d3c4", "d3d2", "d3d4", "d3e2", "d3e3", "d3e4" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var topCornerKing = new King("a1", Sides.White);
            targets = topCornerKing.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "a1a2", "a1b1", "a1b2" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var bottomCornerKing = new King("h8", Sides.White);
            targets = bottomCornerKing.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "h8h7", "h8g7", "h8g8" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }
        }



        [TestMethod]
        public void TestCastles()
        {
            var b = new Board("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            var king = b.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            var moves = Algebraic(king, b);
            Assert.IsTrue(moves.Contains("e1g1"));
            Assert.IsTrue(moves.Contains("e1c1"));

            var noRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1");
            king = noRights.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = Algebraic(king, noRights);
            Assert.IsFalse(moves.Contains("e1c1"));
            Assert.IsFalse(moves.Contains("e1g1"));

            var noKingRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w Qkq - 0 1");
            king = noKingRights.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = Algebraic(king, noKingRights);
            Assert.IsTrue(moves.Contains("e1c1"));
            Assert.IsFalse(moves.Contains("e1g1"));

            var blockers = new Board("7k/8/8/8/8/8/8/4KB1R w K - 0 1");
            king = blockers.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = Algebraic(king, blockers);
            Assert.IsFalse(moves.Contains("e1h1"));

            var attackedSquare = new Board("7k/8/2r5/8/8/8/8/R3K3 w Q - 0 1");
            king = attackedSquare.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = Algebraic(king, attackedSquare);
            Assert.IsFalse(moves.Contains("e1g1"));
        }

        [TestMethod]
        public void CheckMoves()
        {
            // King shouldn't generate moves that put it in check
            var corridor = new Board("k2r1r2/8/8/8/4K3/8/8/8 w - - 0 1");
            var king = corridor.GetKing(true);
            var moves = Algebraic(king, corridor);
            Assert.AreEqual(2, moves.Count());
            Assert.IsFalse(moves.Contains("e4d5"));
            Assert.IsFalse(moves.Contains("e4d4"));
            Assert.IsFalse(moves.Contains("e4d3"));

            Assert.IsFalse(moves.Contains("e4f5"));
            Assert.IsFalse(moves.Contains("e4f4"));
            Assert.IsFalse(moves.Contains("e4f3"));

            Assert.IsTrue(moves.Contains("e4e5"));
            Assert.IsTrue(moves.Contains("e4e3"));
        }

        [TestMethod]
        public void CaptureOnlyTest()
        {
            var board = new Board("k7/8/8/8/6p1/r4pK1/5P1p/8 w - - 0 1");
            var king = board.GetKing(true);
            var moves = king.Moves(board, true);
            foreach (var move in moves)
                Console.WriteLine(move);
            Assert.AreEqual(2, moves.Count());
            Assert.IsTrue(moves.Any(move => move.LongAlgebraic() == "g3h2"));
            Assert.IsTrue(moves.Any(move => move.LongAlgebraic() == "g3g4"));
        }
    }
}

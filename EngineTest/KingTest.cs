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
            var targets = king.Moves(b).Where(m => m != null).Select(m => m.EndAlgebraic());
            Assert.AreEqual(8, targets.Count());

            string[] expected = new string[] { "c2", "c3", "c4", "d2", "d4", "e2", "e3", "e4" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var topCornerKing = new King("a1", Sides.White);
            targets = topCornerKing.Moves(b).Where(m => m != null).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "a2", "b1", "b2" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var bottomCornerKing = new King("h8", Sides.White);
            targets = bottomCornerKing.Moves(b).Where(m => m != null).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "h7", "g7", "g8" };
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
            var moves = StartEnd(king, b);
            Assert.IsTrue(moves.Contains("e1c1"));
            Assert.IsTrue(moves.Contains("e1g1"));

            var noRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1");
            king = noRights.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, noRights);
            Assert.IsFalse(moves.Contains("e1c1"));
            Assert.IsFalse(moves.Contains("e1g1"));

            var noKingRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w Qkq - 0 1");
            king = noKingRights.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, noKingRights);
            Assert.IsTrue(moves.Contains("e1c1"));
            Assert.IsFalse(moves.Contains("e1g1"));

            var blockers = new Board("7k/8/8/8/8/8/8/4KB1R w K - 0 1");
            king = blockers.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, blockers);
            Assert.IsFalse(moves.Contains("e1h1"));

            var attackedSquare = new Board("7k/8/2r5/8/8/8/8/R3K3 w Q - 0 1");
            king = attackedSquare.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, attackedSquare);
            Assert.IsFalse(moves.Contains("e1g1"));
        }

        [TestMethod]
        public void CheckMoves()
        {
            // King shouldn't generate moves that put it in check
            var corridor = new Board("k2r1r2/8/8/8/4K3/8/8/8 w - - 0 1");
            Console.WriteLine(corridor.LegalMoves.WhiteAttacks);
            var king = corridor.GetKing(true);
            var moves = StartEnd(king, corridor);
            //Assert.AreEqual(2, moves.Count());
            Assert.IsFalse(moves.Contains("e4d5"));
            Assert.IsFalse(moves.Contains("e4d4"));
            Assert.IsFalse(moves.Contains("e4d3"));

            Assert.IsFalse(moves.Contains("e4f5"));
            Assert.IsFalse(moves.Contains("e4f4"));
            Assert.IsFalse(moves.Contains("e4f3"));

            Assert.IsTrue(moves.Contains("e4e5"));
            Assert.IsTrue(moves.Contains("e4e3"));
        }
    }
}

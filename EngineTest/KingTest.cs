using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class KingTest : PieceTest
    {
        [TestMethod]
        public void TestMoves()
        {
            var b = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
            var king = new King("D3", Sides.White);
            var targets = king.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(8, targets.Count());

            string[] expected = new string[] { "C2", "C3", "C4", "D2", "D4", "E2", "E3", "E4" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var topCornerKing = new King("A1", Sides.White);
            targets = topCornerKing.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "A2", "B1", "B2" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var bottomCornerKing = new King("H8", Sides.White);
            targets = bottomCornerKing.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "H7", "G7", "G8" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }
        }



        [TestMethod]
        public void TestCastles()
        {
            var b = new Board("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            var king = b.FindPiece(BitUtil.AlgebraicToBit("E1"));
            Assert.IsNotNull(king);
            var moves = StartEnd(king, b);
            Assert.IsTrue(moves.Contains("E1C1"));
            Assert.IsTrue(moves.Contains("E1G1"));

            var noRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1");
            king = noRights.FindPiece(BitUtil.AlgebraicToBit("E1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, noRights);
            Assert.IsFalse(moves.Contains("E1C1"));
            Assert.IsFalse(moves.Contains("E1G1"));

            var noKingRights = new Board("r3k2r/8/8/8/8/8/8/R3K2R w Qkq - 0 1");
            king = noKingRights.FindPiece(BitUtil.AlgebraicToBit("E1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, noKingRights);
            Assert.IsTrue(moves.Contains("E1C1"));
            Assert.IsFalse(moves.Contains("E1G1"));

            var blockers = new Board("7k/8/8/8/8/8/8/4KB1R w K - 0 1");
            king = blockers.FindPiece(BitUtil.AlgebraicToBit("E1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, blockers);
            Assert.IsFalse(moves.Contains("E1G1"));

            var attackedSquare = new Board("7k/8/2r5/8/8/8/8/R3K3 w Q - 0 1");
            king = attackedSquare.FindPiece(BitUtil.AlgebraicToBit("E1"));
            Assert.IsNotNull(king);
            moves = StartEnd(king, attackedSquare);
            Assert.IsFalse(moves.Contains("E1G1"));
        }
    }
}

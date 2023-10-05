using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class BishopTest
    {
        [TestMethod]
        public void TestPieceSprite()
        {
            var white = new Bishop(0, 0, Sides.White);
            Assert.AreEqual("WhiteBishop", actual: white.PieceSprite());
            var black = new Bishop(0, 0, Sides.Black);
            Assert.AreEqual("BlackBishop", actual: black.PieceSprite());
        }

        [TestMethod]
        public void TestMoveMask()
        {
            var b = new Board();
            var top = new Bishop(4, 2, Sides.White);
            Assert.AreEqual((ulong)424704217196612, actual: top.MoveMask(b));

            var right = new Bishop(2, 4, Sides.White);
            Assert.AreEqual((ulong)2310639079102947392, actual: right.MoveMask(b));

            var bottom = new Bishop(3, 6, Sides.White);
            Assert.AreEqual((ulong)1441174018118909952, actual: bottom.MoveMask(b));

            var left = new Bishop(4, 2, Sides.White);
            Assert.AreEqual((ulong)424704217196612, actual: left.MoveMask(b));
        }

        [TestMethod]
        public void TestMoves()
        {
            var b = new Board();
            var bishop = new Bishop(3, 3, Sides.White);
            var moves = bishop.Moves(b);
            var targets = moves.Select(m => m.EndAlgebraic());
            Assert.AreEqual(13, targets.Count());
            Assert.IsTrue(targets.Contains("A1"));
            Assert.IsTrue(targets.Contains("A7"));
            Assert.IsTrue(targets.Contains("B2"));
            Assert.IsTrue(targets.Contains("B6"));
            Assert.IsTrue(targets.Contains("C3"));
            Assert.IsTrue(targets.Contains("C5"));
            Assert.IsTrue(targets.Contains("E3"));
            Assert.IsTrue(targets.Contains("E5"));
            Assert.IsTrue(targets.Contains("F2"));
            Assert.IsTrue(targets.Contains("F6"));
            Assert.IsTrue(targets.Contains("G1"));
            Assert.IsTrue(targets.Contains("G7"));
            Assert.IsTrue(targets.Contains("H8"));
        }
    }
}

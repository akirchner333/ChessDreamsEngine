using Engine;

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
            var bishop = new Bishop(BitUtil.AlgebraicToBit("d4"), Sides.White);
            var moves = bishop.Moves(b);
            var targets = moves.Select(m => m.LongAlgebraic());
            Assert.AreEqual(13, targets.Count());
            Assert.IsTrue(targets.Contains("d4a1"));
            Assert.IsTrue(targets.Contains("d4a7"));
            Assert.IsTrue(targets.Contains("d4b2"));
            Assert.IsTrue(targets.Contains("d4b6"));
            Assert.IsTrue(targets.Contains("d4c3"));
            Assert.IsTrue(targets.Contains("d4c5"));
            Assert.IsTrue(targets.Contains("d4e3"));
            Assert.IsTrue(targets.Contains("d4e5"));
            Assert.IsTrue(targets.Contains("d4f2"));
            Assert.IsTrue(targets.Contains("d4f6"));
            Assert.IsTrue(targets.Contains("d4g1"));
            Assert.IsTrue(targets.Contains("d4g7"));
            Assert.IsTrue(targets.Contains("d4h8"));
        }
    }
}

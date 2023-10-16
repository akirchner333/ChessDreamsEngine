using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class RookTest
    {
        [TestMethod]
        public void TestPieceSprite()
        {
            var white = new Rook(0, 0, Sides.White);
            Assert.AreEqual("WhiteRook", actual: white.PieceSprite());
            var black = new Rook(0, 0, Sides.Black);
            Assert.AreEqual("BlackRook", actual: black.PieceSprite());
        }

        [TestMethod]
        public void TestMoveMask()
        {
            var b = new Board();
            var middle = new Rook(2, 2, Sides.White);
            Assert.AreEqual((ulong)289360691368494084, actual: middle.MoveMask(b));

            var topLeft = new Rook(0, 0, Sides.White);
            Assert.AreEqual((ulong)72340172838076926, actual: topLeft.MoveMask(b));

            var bottomRight = new Rook(7, 7, Sides.White);
            Assert.AreEqual((ulong)9187484529235886208, actual: bottomRight.MoveMask(b));
        }

        [TestMethod]
        public void TestCollisions()
        {
            var friendlies = new Board();
            friendlies.AddPiece(0, 2, PieceTypes.ROOK, Sides.White);
            friendlies.AddPiece(2, 0, PieceTypes.ROOK, Sides.White);
            var topLeft = new Rook(0, 0, Sides.White);
            Assert.AreEqual((ulong)258, actual: topLeft.MoveMask(friendlies));

            var enemies = new Board();
            enemies.AddPiece(0, 2, PieceTypes.ROOK, Sides.Black);
            enemies.AddPiece(2, 0, PieceTypes.ROOK, Sides.Black);
            Assert.AreEqual((ulong)65798, actual: topLeft.MoveMask(enemies));
        }
    }
}
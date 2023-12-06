using Engine;
using Engine.Rules;

namespace EngineTest
{
    [TestClass]
    public class RookTest
    {
        [TestMethod]
        public void TestPieceSprite()
        {
            var white = new Rook("a1", Sides.White);
            Assert.AreEqual("WhiteRook", actual: white.PieceSprite());
            var black = new Rook("a1", Sides.Black);
            Assert.AreEqual("BlackRook", actual: black.PieceSprite());
        }

        [TestMethod]
        public void TestMoveMask()
        {
            var b = new Board();
            var middle = new Rook("c3", Sides.White);
            Assert.AreEqual((ulong)289360691368494084, actual: middle.MoveMask(b));

            var topLeft = new Rook("a1", Sides.White);
            Assert.AreEqual((ulong)72340172838076926, actual: topLeft.MoveMask(b));

            var bottomRight = new Rook("h8", Sides.White);
            Assert.AreEqual((ulong)9187484529235886208, actual: bottomRight.MoveMask(b));
        }

        [TestMethod]
        public void TestCollisions()
        {
            var friendlies = new Board();
            friendlies.AddPiece("a3", PieceTypes.ROOK, Sides.White);
            friendlies.AddPiece("c1", PieceTypes.ROOK, Sides.White);
            var topLeft = new Rook("a1", Sides.White);
            Assert.AreEqual(258ul, actual: topLeft.MoveMask(friendlies));

            var enemies = new Board();
            enemies.AddPiece("a3", PieceTypes.ROOK, Sides.Black);
            enemies.AddPiece("c1", PieceTypes.ROOK, Sides.Black);
            Console.WriteLine(enemies.BlackPieces);
            Assert.AreEqual(65798ul, actual: topLeft.MoveMask(enemies));

            var middle = new Rook("e4", Sides.White);
            var middleEnemies = new Board("k7/4r3/8/8/2r3r1/4r3/8/7K w - - 0 1");
            Assert.AreEqual(4521262345879552ul, middle.MoveMask(middleEnemies));

            var middleFriends = new Board("k7/4R3/8/8/2R3R1/4R3/8/7K w - - 0 1");
            Assert.AreEqual(17661576609792ul, middle.MoveMask(middleFriends));
        }

        [TestMethod]
        public void TestCastleRights()
        {
            var a8 = new Rook("a8", false);
            Assert.AreEqual((int)Castles.BlackQueenside, a8.CastleRights);

            var h8 = new Rook("h8", false);
            Assert.AreEqual((int)Castles.BlackKingside, h8.CastleRights);

            var a1 = new Rook("a1", true);
            Assert.AreEqual((int)Castles.WhiteQueenside, a1.CastleRights);

            var h1 = new Rook("h1", true);
            Assert.AreEqual((int)Castles.WhiteKingside, h1.CastleRights);
        }
    }
}
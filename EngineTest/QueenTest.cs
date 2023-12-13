using Engine;

namespace EngineTest
{
    [TestClass]
    public class QueenTest : PieceTest
    {
        [TestMethod]
        public void MoveTest()
        {
            Queen topLeft = new Queen("a1", true);
            Board b = new Board();

            var moves = Algebraic(topLeft, b);
            Assert.AreEqual(21, moves.Count());
            foreach (var square in new string[] {
                "a1a2", "a1a3", "a1a4", "a1a5", "a1a6", "a1a7", "a1a8",
                "a1b1", "a1c1", "a1d1", "a1e1", "a1f1", "a1g1", "a1h1",
                "a1b2", "a1c3", "a1d4", "a1e5", "a1f6", "a1g7", "a1h8"
            })
            {
                Assert.IsTrue(moves.Any(m => m == square));
            }

            Queen bottomRight = new Queen("h8", true);
            moves = Algebraic(bottomRight, b);
            Assert.AreEqual(21, moves.Count());
            foreach (var square in new string[] {
                "h8h1", "h8h2", "h8h3", "h8h4", "h8h5", "h8h6", "h8h7",
                "h8a8", "h8b8", "h8c8", "h8d8", "h8e8", "h8f8", "h8g8",
                "h8a1", "h8b2", "h8c3", "h8d4", "h8e5", "h8f6", "h8g7"
            })
            {
                Assert.IsTrue(moves.Any(m => m == square));
            }
        }

        [TestMethod]
        public void PathBetweenTest()
        {
            var board = new Board("k7/4r3/8/8/8/8/r1r1Q3/7K b - - 0 1");
            var queen = board.FindPiece(BitUtil.AlgebraicToBit("e2"));
            Assert.AreEqual(17661175005184ul, queen.PathBetween(board, BitUtil.AlgebraicToIndex("e7")));
            Assert.AreEqual(1075838976ul, queen.PathBetween(board, BitUtil.AlgebraicToIndex("h5")));

            Assert.AreEqual(1024ul, queen.PathBetween(board, BitUtil.AlgebraicToIndex("a2")));
        }
    }
}

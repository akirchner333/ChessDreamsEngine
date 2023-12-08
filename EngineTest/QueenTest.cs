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

            var moves = EndPoints(topLeft, b);
            Assert.AreEqual(21, moves.Count());
            foreach (var square in new string[] {
                "a2", "a3", "a4", "a5", "a6", "a7", "a8",
                "b1", "c1", "d1", "e1", "f1", "g1", "h1",
                "b2", "c3", "d4", "e5", "f6", "g7", "h8"
            })
            {
                Assert.IsTrue(moves.Any(m => m == square));
            }

            Queen bottomRight = new Queen("h8", true);
            moves = EndPoints(bottomRight, b);
            Assert.AreEqual(21, moves.Count());
            foreach (var square in new string[] {
                "h1", "h2", "h3", "h4", "h5", "h6", "h7",
                "a8", "b8", "c8", "d8", "e8", "f8", "g8",
                "a1", "b2", "c3", "d4", "e5", "f6", "g7"
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

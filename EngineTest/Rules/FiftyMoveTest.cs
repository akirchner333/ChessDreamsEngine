using Engine;

namespace EngineTest.Rules
{
    [TestClass]
    public class FiftyMoveTest
    {
        [TestMethod]
        public void FiftyMoveDrawTest()
        {
            var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 99 49");
            board.ApplyMove(new Move(BitUtil.AlgebraicToBit("b1"), BitUtil.AlgebraicToBit("c3"), true));
            Assert.AreEqual(100, board.Clock.Clock);
            Assert.IsTrue(board.DrawAvailable());
        }

        [TestMethod]
        public void PawnResetTest()
        {
            var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 99 49");
            board.ApplyMove(new Move(BitUtil.AlgebraicToBit("a2"), BitUtil.AlgebraicToBit("a3"), true));
            Assert.AreEqual(0, board.Clock.Clock);
            Assert.IsFalse(board.DrawAvailable());
        }

        [TestMethod]
        public void CaptureResetTest()
        {
            var board = new Board("rnbqkb1r/pppppppp/8/8/8/6n1/PPPPPPPP/RNBQKBNR w KQkq - 99 49");
            board.ApplyMove(new Move(BitUtil.AlgebraicToBit("f2"), BitUtil.AlgebraicToBit("g3"), true) { Capture = true });
            Assert.AreEqual(0, board.Clock.Clock);
            Assert.IsFalse(board.DrawAvailable());
        }

        [TestMethod]
        public void SeventyFiveMoveDrawTest()
        {
            var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 149 74");
            board.ApplyMove(new Move(BitUtil.AlgebraicToBit("b1"), BitUtil.AlgebraicToBit("c3"), true));
            Assert.AreEqual(150, board.Clock.Clock);
            Assert.AreEqual(GameState.DRAW, board.State);
        }
    }
}

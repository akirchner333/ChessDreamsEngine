using Engine;

namespace EngineTest
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void CastleRightsTest()
        {
            Board all = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w KQkq - 0 1");
            Assert.IsTrue(all.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(all.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(all.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(all.HasCastleRights(Castles.BlackQueenside));

            Board none = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w - - 0 1");
            Assert.IsFalse(none.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(none.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(none.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(none.HasCastleRights(Castles.BlackQueenside));

            Board whiteOnly = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w KQ - 0 1");
            Assert.IsTrue(whiteOnly.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(whiteOnly.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(whiteOnly.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(whiteOnly.HasCastleRights(Castles.BlackQueenside));

            Board blackOnly = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w kq - 0 1");
            Assert.IsFalse(blackOnly.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(blackOnly.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(blackOnly.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(blackOnly.HasCastleRights(Castles.BlackQueenside));

            Board noKing = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w Qq - 0 1");
            Assert.IsFalse(noKing.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(noKing.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(noKing.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(noKing.HasCastleRights(Castles.BlackQueenside));

            Board noQueen = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w Kk - 0 1");
            Assert.IsTrue(noQueen.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(noQueen.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(noQueen.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(noQueen.HasCastleRights(Castles.BlackQueenside));
        }

        [TestMethod]
        public void CheckTest()
        {
            Board rookCheck = new Board("k6r/8/8/8/8/N7/8/R6K b - - 0 1");
            Assert.IsTrue(rookCheck.Attacked(Sides.White, BitUtil.AlgebraicToBit("H1")));
            Assert.IsFalse(rookCheck.Attacked(Sides.Black, BitUtil.AlgebraicToBit("A8")));

            Board pawnCheck = new Board("8/8/8/1k3p2/2P2K2/8/8/8 b - - 0 1");
            Assert.IsTrue(pawnCheck.Attacked(Sides.Black, BitUtil.AlgebraicToBit("B5")));
            Assert.IsFalse(pawnCheck.Attacked(Sides.White, BitUtil.AlgebraicToBit("F4")));
        }

        [TestMethod]
        public void RemoveCheckMovesTest()
        {
            //The knight is pinned, so it has no legal moves
            Board pinned = new Board("k3r3/8/8/8/4N3/8/8/4K3 w - - 0 1");
            var moves = pinned.Moves();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("E4")));

            //To move into the F column would put the king in check, so it has no legal moves
            Board noF = new Board("3nk3/3pp3/8/8/8/8/8/K4R2 b - - 0 1");
            moves = noF.Moves();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("E8")));

            Board mandatoryCapture = new Board("4k3/4Q3/8/8/8/8/8/K7 b - - 0 1");
            moves = mandatoryCapture.Moves();
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual("E8E7", moves[0].StartEnd());

            Board mandatoryCapture2 = new Board("4k3/4Q2R/8/8/7q/8/8/K7 b - - 0 1");
            moves = mandatoryCapture2.Moves();
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual("H4E7", moves[0].StartEnd());
        }
    }
}

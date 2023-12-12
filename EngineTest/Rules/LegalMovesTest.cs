using Engine;

namespace EngineTest.Rules
{
    [TestClass]
    public class LegalMovesTest
    {
        [TestMethod]
        public void RemoveCheckMovesTest()
        {
            //The knight is pinned, so it has no legal moves
            Board pinned = new("k3r3/8/8/8/4N3/8/8/4K3 w - - 0 1");
            var moves = pinned.MoveArray();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("e4")));

            //To move into the F column would put the king in check, so it has no legal moves
            Board noF = new("3nk3/3pp3/8/8/8/8/8/K4R2 b - - 0 1");
            moves = noF.MoveArray();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("e8")));
        }

        [TestMethod]
        public void MandatoryCaptureTest()
        {
            // The king has to capture the queen
            Board mandatoryCapture = new("4k3/4Q3/8/8/8/8/8/K7 b - - 0 1");
            var moves = mandatoryCapture.MoveArray();
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual("e8e7", moves[0].StartEnd());

            // The black queen has to capture the white queen
            Board mandatoryCapture2 = new("4k3/4Q2R/8/8/7q/8/8/K7 b - - 0 1");
            moves = mandatoryCapture2.MoveArray();
            Assert.AreEqual(1, moves.Length);
            Assert.AreEqual("h4e7", moves[0].StartEnd());
        }

        [TestMethod]
        public void KingCaptureCheckTest()
        {
            Board board = new("4r3/8/2p5/3p4/2rKn3/n7/8/7k w - - 0 1");
            var moves = board.MoveArray();
            Assert.AreEqual(2, moves.Length);
            // Cannot capture the rook, cause of the knight
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "d4c4"));
            // Cannot capture the knight, cause of the rook
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "d4e4"));
            // Cannot capture the pawn, cause of the other pawn
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "d4d5"));
        }

        [TestMethod]
        public void SlideCheckTest()
        {
            Board board = new("7k/1b2r2q/8/8/4K3/8/8/8 w - - 0 1");
            var moves = board.MoveArray();
            Assert.AreEqual(2, moves.Length);
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "e4d3"));
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "e4e3"));
            Assert.IsFalse(moves.Any(m => m.LongAlgebraic() == "e4f3"));
        }

        [TestMethod]
        public void PinTest()
        {
            var board = new Board("8/1r1p2k1/5pp1/4B3/3b4/1PP5/1K1P2R1/8 b - - 0 1");
            Assert.AreEqual(393216ul, board.LegalMoves.WhitePins);
            Assert.AreEqual(105553116266496ul, board.LegalMoves.BlackPins);
        }

        [TestMethod]
        public void ReverseMove()
        {
            using StreamReader r = new("../../../board_moves.csv");

            string line;
            while ((line = r.ReadLine()) != null)
            {
                var fen = line.Split(',')[0];
                var board = new Board(fen);

                board.LegalMoves.SetAttackMasks();
                var whitePins = board.LegalMoves.WhitePins;
                var blackPins = board.LegalMoves.BlackPins;
                var whiteAttacks = board.LegalMoves.WhiteAttacks;
                var blackAttacks = board.LegalMoves.BlackAttacks;
                foreach (var move in board.MoveArray())
                {
                    var fullMove = board.ApplyMove(move);
                    board.ReverseMove(fullMove);
                    Assert.AreEqual(whitePins, board.LegalMoves.WhitePins, $"White pins mismatch {move} {fen}");
                    Assert.AreEqual(blackPins, board.LegalMoves.BlackPins, $"White pins mismatch {move} {fen}");
                    Assert.AreEqual(whiteAttacks, board.LegalMoves.WhiteAttacks, $"White attack mismatch {move} {fen}");
                    Assert.AreEqual(blackAttacks, board.LegalMoves.BlackAttacks, $"Black attack mismatch {move} {fen}");
                }

            }
        }
    }
}

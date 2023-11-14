using Engine;
using Engine.Rules;
using System.ComponentModel;
using System.Linq;

namespace EngineTest
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void CastleRightsTest()
        {
            Board all = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w KQkq - 0 1");
            Assert.IsTrue(all.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(all.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(all.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(all.Castles.HasCastleRights(Castles.BlackQueenside));

            Board none = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w - - 0 1");
            Assert.IsFalse(none.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(none.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(none.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(none.Castles.HasCastleRights(Castles.BlackQueenside));

            Board whiteOnly = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w KQ - 0 1");
            Assert.IsTrue(whiteOnly.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(whiteOnly.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(whiteOnly.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(whiteOnly.Castles.HasCastleRights(Castles.BlackQueenside));

            Board blackOnly = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w kq - 0 1");
            Assert.IsFalse(blackOnly.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(blackOnly.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(blackOnly.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(blackOnly.Castles.HasCastleRights(Castles.BlackQueenside));

            Board noKing = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w Qq - 0 1");
            Assert.IsFalse(noKing.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsTrue(noKing.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsFalse(noKing.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsTrue(noKing.Castles.HasCastleRights(Castles.BlackQueenside));

            Board noQueen = new Board("rnbqkbnr/8/8/8/8/8/8/RNBQKBNR w Kk - 0 1");
            Assert.IsTrue(noQueen.Castles.HasCastleRights(Castles.WhiteKingside));
            Assert.IsFalse(noQueen.Castles.HasCastleRights(Castles.WhiteQueenside));
            Assert.IsTrue(noQueen.Castles.HasCastleRights(Castles.BlackKingside));
            Assert.IsFalse(noQueen.Castles.HasCastleRights(Castles.BlackQueenside));
        }

        [TestMethod]
        public void CheckTest()
        {
            Board rookCheck = new Board("k6r/8/8/8/8/N7/8/R6K b - - 0 1");
            Assert.IsTrue(rookCheck.Attacked(Sides.White, BitUtil.AlgebraicToBit("h1")));
            Assert.IsFalse(rookCheck.Attacked(Sides.Black, BitUtil.AlgebraicToBit("a8")));

            Board pawnCheck = new Board("8/8/8/1k3p2/2P2K2/8/8/8 b - - 0 1");
            Assert.IsTrue(pawnCheck.Attacked(Sides.Black, BitUtil.AlgebraicToBit("b5")));
            Assert.IsFalse(pawnCheck.Attacked(Sides.White, BitUtil.AlgebraicToBit("f4")));
        }

        [TestMethod]
        public void RemoveCheckMovesTest()
        {
            //The knight is pinned, so it has no legal moves
            Board pinned = new Board("k3r3/8/8/8/4N3/8/8/4K3 w - - 0 1");
            var moves = pinned.Moves();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("e4")));

            //To move into the F column would put the king in check, so it has no legal moves
            Board noF = new Board("3nk3/3pp3/8/8/8/8/8/K4R2 b - - 0 1");
            moves = noF.Moves();
            Assert.IsFalse(moves.Any(m => m.Start == BitUtil.AlgebraicToBit("e8")));

            Board mandatoryCapture = new Board("4k3/4Q3/8/8/8/8/8/K7 b - - 0 1");
            moves = mandatoryCapture.Moves();
            Assert.AreEqual(1, moves.Count());
            Assert.AreEqual("e8e7", moves[0].StartEnd());

            Board mandatoryCapture2 = new Board("4k3/4Q2R/8/8/7q/8/8/K7 b - - 0 1");
            moves = mandatoryCapture2.Moves();
            Assert.AreEqual(1, moves.Count());
            Assert.AreEqual("h4e7", moves[0].StartEnd());
        }

        [TestMethod]
        public void BoardMoveTests()
        {
            //board_moves.csv contains a bunch of FENs and all the possible moves from those positions
            //with move generation taken from Stockfish. Goes through each of them and checks to see if the generated moves
            //match the expected moves
            using (StreamReader r = new StreamReader("../../../board_moves.csv"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var board = new Board(parts[0]);
                    var officialMoves = parts[1].Split(" ").ToList();
                    var moves = board.Moves();
                    
                    foreach(var move in board.Moves())
                    {
                        Assert.IsTrue(
                            officialMoves.Contains(move.LongAlgebraic()),
                            $"Found incorrect {move.LongAlgebraic()} in {parts[0]}"
                        );
                    }

                    foreach (var move in officialMoves)
                    {
                        Assert.IsTrue(
                            moves.Select(m => m.LongAlgebraic()).Contains(move),
                            $"Expected to find {move} in {parts[0]} but did not"
                        );
                    }
                    Assert.AreEqual(officialMoves.Count, moves.Count(), $"Move count mismatch on {parts[0]}");
                }
            }
        }

        [TestMethod]
        public void FenTest()
        {
            using (StreamReader r = new StreamReader("../../../board_moves.csv"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    var fen = line.Split(',')[0];
                    var board = new Board(fen);
                    Assert.AreEqual(fen, board.Fen());

                }
            }
        }

        //b4b3 f6f5 a7a6 c7c6 d7d6 g7g6 h7h6 a7a5 c7c5 d7d5 h7h5 b4c3 f6g5 b4a3 b8a6 b8c6 g8h6 g8e7
        //c8a6 c8b7 f8c5 f8d6 f8e7 d8e7 e8e7 e8f7
        [TestMethod]
        public void MoveSequenceTest()
        {
            using (StreamReader r = new StreamReader("../../../move_sequence.csv"))
            {
                var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                string line;
                int halfmove = 0;
                while ((line = r.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var officialMoves = parts[1].Split(" ").ToList();
                    var moves = board.Moves();
                    //var missing = String.Join(" ", moves.Select(m => m.LongAlgebraic()));
                    
                    foreach (var move in board.Moves())
                    {
                        Assert.IsTrue(
                            officialMoves.Contains(move.LongAlgebraic()),
                            $"Found incorrect move {move.LongAlgebraic()} in halfmove {halfmove}, before {parts[0]}"
                        );
                    }

                    foreach (var move in officialMoves)
                    {
                        Assert.IsTrue(
                            moves.Select(m => m.LongAlgebraic()).Contains(move),
                            $"Expected to find {move} at halfmove {halfmove} (before {parts[0]} but did not"
                        );
                    }
                    Assert.AreEqual(officialMoves.Count, moves.Count(), $"Move count mismatch on halfmove {halfmove}, before {parts[0]}");

                    var nextMove = Array.Find(moves, m => m.LongAlgebraic() == parts[0]);
                    board.ApplyMove(nextMove!);
                    halfmove++;
                }
            }
        }

        [TestMethod]
        public void ReverseMove()
        {
            using (StreamReader r = new StreamReader("../../../board_moves.csv"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var fen = parts[0];
                    var officialMoves = parts[1].Split(" ").ToList();
                    var board = new Board(fen);

                    var hash = board.Hash;
                    var moves = board.Moves();
                    var all = board.AllPieces;
                    var white = board.WhitePieces;
                    var black = board.BlackPieces;
                    foreach (var move in board.Moves())
                    {
                        var fullMove = board.ApplyMove(move);
                        board.ReverseMove(fullMove);
                        Assert.AreEqual(fen, board.Fen(), $"Expected: {fen}, Actual: {board.Fen()}, After {move.StartEnd()}");
                        Assert.AreEqual(hash, board.Hash, $"Hash mismatch {move.ToString()}");
                        Assert.AreEqual(all, board.AllPieces, $"All Pieces mismatch {move.ToString()}");
                        Assert.AreEqual(white, board.WhitePieces, $"White pieces mismatch {move.ToString()}");
                        Assert.AreEqual(black, board.BlackPieces, $"Black pieces mismatch {move.ToString()}");

                        var moveList = board.Moves();

                        Assert.AreEqual(moves.Count(), officialMoves.Count, $"Move count mismatch on {parts[0]}");
                        foreach (var m in moveList)
                        {
                            Assert.IsTrue(
                                officialMoves.Contains(m.LongAlgebraic()),
                                $"Expected to find {m.LongAlgebraic()} in {parts[0]} but did not"
                            );
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void DepthTest()
        {
            var board = new Board("k7/8/8/8/8/3B1b2/8/4K2R b K - 0 1");
            TestReverse(board, 3, "");
        }

        public void TestReverse(Board board, int depth, string sequence)
        {
            if (depth == 0)
                return;

            var hash = board.Hash;
            var all = board.AllPieces;
            var white = board.WhitePieces;
            var black = board.BlackPieces;

            foreach (var move in board.Moves())
            {
                var newSequence = sequence + " " + move.LongAlgebraic();
                var fullMove = board.ApplyMove(move);
                TestReverse(board, depth - 1, newSequence);
                board.ReverseMove(fullMove);
                Assert.AreEqual(all, board.AllPieces, $"All Pieces mismatch on depth {depth} after {newSequence}");
                Assert.AreEqual(white, board.WhitePieces, $"White pieces mismatch on depth {depth} after {newSequence}");
                Assert.AreEqual(black, board.BlackPieces, $"Black pieces mismatch on depth {depth} after {newSequence}");
                Assert.AreEqual(hash, board.Hash, $"Hash mismatch on depth {depth} after {newSequence}");
            }
        }
    }
}

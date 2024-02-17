using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace EngineTest
{
    [TestClass]
    public class PGNTest
    {
        [TestMethod]
        public void AttributeTest()
        {
            var pgn = new PGN("[Event \"London m\"]\r\n[Site \"London\"]\r\n[Date \"1840.??.??\"]\r\n[Round \"?\"]\r\n[White \"Staunton, Howard\"]\r\n[Black \"Popert, HW.\"]\r\n[Result \"0-1\"]\r\n[WhiteElo \"\"]\r\n[BlackElo \"\"]\r\n[ECO \"C02\"]\r\n\r\n1.e4 e6 2.d4 d5 3.e5 c5 4.c3 Nc6 5.Nf3 Bd7 6.a3 Rc8 7.b4 cxd4 8.cxd4 Be7\r\n9.Bd3 f6 10.Bd2 f5 11.Qe2 Rc7 12.O-O g5 13.b5 Nb8 14.Nxg5 h5 15.Nh3 h4 16.Nc3 Nh6\r\n17.a4 Kf7 18.Rac1 Qg8 19.f3 Ke8 20.Rfe1 Rc8 21.Bf4 Bd8 22.Nxd5 Rxc1 23.Rxc1 exd5\r\n24.e6 Qxe6 25.Qxe6+ Bxe6 26.Bxb8 Bb6 27.Be5 Rg8 28.Nf4 Nf7 29.Nxe6 Nxe5 30.Bxf5 Nxf3+\r\n31.Kf2 Nxd4 32.Nxd4 Bxd4+ 33.Kf3 Kf7 34.Rd1 Be5 35.Rxd5 Bxh2 36.Rd7+ Kf6\r\n37.Bg4 Rg7 38.Rd6+ Bxd6  0-1");

            Assert.AreEqual("0-1", pgn.Attributes["Result"]);
            Assert.AreEqual("London m", pgn.Attributes["Event"]);
            Assert.AreEqual("", pgn.Attributes["BlackElo"]);
        }

        [TestMethod]
        public void MoveTest()
        {
            var pgn = new PGN("[Event \"London m\"]\r\n[Site \"London\"]\r\n[Date \"1840.??.??\"]\r\n[Round \"?\"]\r\n[White \"Staunton, Howard\"]\r\n[Black \"Popert, HW.\"]\r\n[Result \"0-1\"]\r\n[WhiteElo \"\"]\r\n[BlackElo \"\"]\r\n[ECO \"C02\"]\r\n\r\n1.e4 e6 2.d4 d5 3.e5 c5 4.c3 Nc6 5.Nf3 Bd7 6.a3 Rc8 7.b4 cxd4 8.cxd4 Be7\r\n9.Bd3 f6 10.Bd2 f5 11.Qe2 Rc7 12.O-O g5 13.b5 Nb8 14.Nxg5 h5 15.Nh3 h4 16.Nc3 Nh6\r\n17.a4 Kf7 18.Rac1 Qg8 19.f3 Ke8 20.Rfe1 Rc8 21.Bf4 Bd8 22.Nxd5 Rxc1 23.Rxc1 exd5\r\n24.e6 Qxe6 25.Qxe6+ Bxe6 26.Bxb8 Bb6 27.Be5 Rg8 28.Nf4 Nf7 29.Nxe6 Nxe5 30.Bxf5 Nxf3+\r\n31.Kf2 Nxd4 32.Nxd4 Bxd4+ 33.Kf3 Kf7 34.Rd1 Be5 35.Rxd5 Bxh2 36.Rd7+ Kf6\r\n37.Bg4 Rg7 38.Rd6+ Bxd6  0-1");
            Assert.AreEqual("e4", pgn.Moves[0]);
            Assert.AreEqual("a3", pgn.Moves[10]);
            Assert.AreEqual("Rc8", pgn.Moves[11]);
        }

        [TestMethod]
        public void StepTest()
        {
            var pgn = new PGN("[Event \"London m\"]\r\n[Site \"London\"]\r\n[Date \"1840.??.??\"]\r\n[Round \"?\"]\r\n[White \"Staunton, Howard\"]\r\n[Black \"Popert, HW.\"]\r\n[Result \"0-1\"]\r\n[WhiteElo \"\"]\r\n[BlackElo \"\"]\r\n[ECO \"C02\"]\r\n\r\n1.e4 e6 2.d4 d5 3.e5 c5 4.c3 Nc6 5.Nf3 Bd7 6.a3 Rc8 7.b4 cxd4 8.cxd4 Be7\r\n9.Bd3 f6 10.Bd2 f5 11.Qe2 Rc7 12.O-O g5 13.b5 Nb8 14.Nxg5 h5 15.Nh3 h4 16.Nc3 Nh6\r\n17.a4 Kf7 18.Rac1 Qg8 19.f3 Ke8 20.Rfe1 Rc8 21.Bf4 Bd8 22.Nxd5 Rxc1 23.Rxc1 exd5\r\n24.e6 Qxe6 25.Qxe6+ Bxe6 26.Bxb8 Bb6 27.Be5 Rg8 28.Nf4 Nf7 29.Nxe6 Nxe5 30.Bxf5 Nxf3+\r\n31.Kf2 Nxd4 32.Nxd4 Bxd4+ 33.Kf3 Kf7 34.Rd1 Be5 35.Rxd5 Bxh2 36.Rd7+ Kf6\r\n37.Bg4 Rg7 38.Rd6+ Bxd6  0-1");
            pgn.StepGame();
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", pgn.Board.Fen());
            pgn.StepGame();
            pgn.StepGame();
            pgn.StepGame();
            Assert.AreEqual("rnbqkbnr/ppp2ppp/4p3/3p4/3PP3/8/PPP2PPP/RNBQKBNR w KQkq d6 0 3", pgn.Board.Fen());

        }

        [TestMethod]
        public void CastlingTest()
        {
            var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");
            Span<Move> moves = new Move[board.MAXMOVES];
            board.Moves(ref moves);
            var pgn = new PGN("");

            var kingside = pgn.FindMove("O-O+", moves);
            Assert.AreEqual("e1g1", kingside.LongAlgebraic());
            Console.WriteLine("-");
            var queenside = pgn.FindMove("O-O-O+", moves);
            Assert.AreEqual("e1c1", queenside.LongAlgebraic());
        }

        [TestMethod]
        public void PositionTest()
        {
            var pgn = new PGN("");

            var rowBoard = new Board("6k1/8/R7/8/7R/8/8/RR1B3K w - - 0 1");
            pgn.Board = rowBoard;
            Span<Move> moves = new Move[rowBoard.MAXMOVES];
            rowBoard.Moves(ref moves);
            var rookMove = pgn.FindMove("R1a4", moves);
            Assert.AreEqual("a1a4", rookMove.LongAlgebraic());

            var colBoard = new Board("k6r/8/5n2/1b5b/8/8/8/7K b - - 0 1");
            pgn.Board = colBoard;
            moves = new Move[colBoard.MAXMOVES];
            colBoard.Moves(ref moves);
            var rookMove2 = pgn.FindMove("Bhe8", moves);
            Assert.AreEqual("h5e8", rookMove2.LongAlgebraic());
        }

        [TestMethod]
        public void PromotionTest()
        {
            var pgn = new PGN("");

            var board = new Board("3q3R/2P1P3/8/7b/8/8/8/k6K w - - 0 1");
            Span<Move> moves = new Move[board.MAXMOVES];
            board.Moves(ref moves);
            var promo = pgn.FindMove("exd8=Q", moves);
            Assert.AreEqual("e7d8q", promo.LongAlgebraic());
        }
    }
}

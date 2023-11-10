using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest
{
    [TestClass]
    public class PawnTest : PieceTest
    {
        [TestMethod]
        public void TwoStepTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("a2", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("a3"));
            Assert.IsTrue(targets.Contains("a4"));

            Pawn blackPawn = new Pawn("a7", Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("a6"));
            Assert.IsTrue(targets.Contains("a5"));
        }

        [TestMethod]
        public void OneStepTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("a4", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("a5"), $"Expected to contain a5 actually continas {targets.First()}");

            Pawn blackPawn = new Pawn("a5", Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("a4"), $"Expected to contain a4 actually contains {targets.First()}");
        }

        [TestMethod]
        public void BlockedTest()
        {
            Board b = new Board();
            b.AddPiece(0, 2, PieceTypes.PAWN, Sides.Black);
            Pawn whitePawn = new Pawn("a2", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(0, targets.Count());

            Pawn blackPawn = new Pawn("a7", Sides.Black);
            b.AddPiece(0, 5, PieceTypes.PAWN, Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(0, targets.Count());
        }

        [TestMethod]
        public void CaptureTest()
        {
            Board b = new Board("k7/8/PPPPPPPP/8/8/pppppppp/8/7K b - - 0 1");

            Pawn whitePawn = new Pawn("c2", Sides.White);
            var targets = EndPoints(whitePawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("b3"));
            Assert.IsTrue(targets.Contains("d3"));
            var moves = whitePawn.Moves(b);
            Assert.IsTrue(moves.All(m => m.Capture));

            Pawn leftPawn = new Pawn("a2", Sides.White);
            targets = EndPoints(leftPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("b3"));

            Pawn rightPawn = new Pawn("h2", Sides.White);
            targets = EndPoints(rightPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("g3"));

            Pawn blackPawn = new Pawn("c7", Sides.Black);
            targets = EndPoints(blackPawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("b6"));
            Assert.IsTrue(targets.Contains("d6"));

            var blackBoard = new Board("k7/7P/7P/7P/7P/7P/7P/7K w - - 0 1");
            var pawn = new Pawn("a4", Sides.Black);
            targets = EndPoints(pawn, blackBoard);
            Assert.AreEqual(1, targets.Count());
        }

        [TestMethod]
        public void PromotionTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("b7", Sides.White);
            var moves = whitePawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m is PromotionMove));
            Assert.IsTrue(moves.All(m => BitUtil.BitToAlgebraic(m.End) == "b8"));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.ROOK));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.QUEEN));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.BISHOP));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.KNIGHT));

            Pawn blackPawn = new Pawn("b2", Sides.Black);
            moves = blackPawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m is PromotionMove));
        }

        [TestMethod]
        public void PromotionCaptureTest()
        {
            Board b = new Board();
            b.AddPiece("a8", PieceTypes.ROOK, Sides.Black);
            b.AddPiece("b8", PieceTypes.ROOK, Sides.Black);
            b.AddPiece("c8", PieceTypes.ROOK, Sides.Black);

            Pawn whitePawn = new Pawn("b7", Sides.White);
            var moves = whitePawn.Moves(b);
            Assert.AreEqual(8, moves.Count());
            Assert.IsTrue(moves.All(m => m.Capture));
            Assert.IsTrue(moves.Any(m => m.End == BitUtil.AlgebraicToBit("a8")));
            Assert.IsTrue(moves.Any(m => m.End == BitUtil.AlgebraicToBit("c8")));
        }

        [TestMethod]
        public void EnPassantTest()
        {
            Board b = new Board("7k/8/8/8/PpP5/8/8/7K b - a3 0 1");

            var blackPawn = b.FindPiece(BitUtil.AlgebraicToBit("b4"));
            Assert.IsNotNull(blackPawn);

            var moves = blackPawn!.Moves(b);
            Assert.IsTrue(moves.Any(m => m is PassantMove));
            var passant = Array.Find(moves, m => m is PassantMove);
            if(passant != null)
            {
                Assert.AreEqual(BitUtil.AlgebraicToBit("b4"), passant.Start);
                Assert.AreEqual(BitUtil.AlgebraicToBit("a3"), passant.End);
                Assert.AreEqual(BitUtil.AlgebraicToBit("a4"), passant.TargetSquare());
            }
        }
    }
}

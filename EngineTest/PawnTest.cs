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
            Pawn whitePawn = new Pawn("A2", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("A3"));
            Assert.IsTrue(targets.Contains("A4"));

            Pawn blackPawn = new Pawn("A7", Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("A6"));
            Assert.IsTrue(targets.Contains("A5"));
        }

        [TestMethod]
        public void OneStepTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("A2", Sides.White);
            whitePawn.ApplyMove(new Move(BitUtilities.AlgebraicToBit("A2"), BitUtilities.AlgebraicToBit("A4"), Sides.White));
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("A5"));

            Pawn blackPawn = new Pawn("A7", Sides.Black);
            blackPawn.ApplyMove(new Move(BitUtilities.AlgebraicToBit("A7"), BitUtilities.AlgebraicToBit("A5"), Sides.Black));
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("A4"));
        }

        [TestMethod]
        public void BlockedTest()
        {
            Board b = new Board();
            b.AddPiece(0, 2, PieceTypes.PAWN, Sides.Black);
            Pawn whitePawn = new Pawn("A2", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(0, targets.Count());

            Pawn blackPawn = new Pawn("A7", Sides.Black);
            b.AddPiece(0, 5, PieceTypes.PAWN, Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(0, targets.Count());
        }

        [TestMethod]
        public void CaptureTest()
        {
            Board b = new Board();
            for(var i = 0; i < 8; i++)
            {
                b.AddPiece(i, 2, PieceTypes.PAWN, Sides.Black);
                b.AddPiece(i, 3, PieceTypes.PAWN, Sides.Black);
                b.AddPiece(i, 5, PieceTypes.PAWN, Sides.White);
            }
            b.AddPiece("A1", PieceTypes.ROOK, Sides.Black);

            Pawn whitePawn = new Pawn("C2", Sides.White);
            var targets = EndPoints(whitePawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("B3"));
            Assert.IsTrue(targets.Contains("D3"));

            Pawn leftPawn = new Pawn("A2", Sides.White);
            targets = EndPoints(leftPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("B3"));

            Pawn rightPawn = new Pawn("H2", Sides.White);
            targets = EndPoints(rightPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("G3"));

            Pawn blackPawn = new Pawn("C7", Sides.Black);
            targets = EndPoints(blackPawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("B6"));
            Assert.IsTrue(targets.Contains("D6"));
        }

        [TestMethod]
        public void PromotionTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("B7", Sides.White);
            var moves = whitePawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m is PromotionMove));
            Assert.IsTrue(moves.All(m => BitUtilities.BitToAlgebraic(m.End) == "B8"));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.ROOK));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.QUEEN));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.BISHOP));
            Assert.IsTrue(moves.Any(m => ((PromotionMove)m).Promotion == PieceTypes.KNIGHT));

            Pawn blackPawn = new Pawn("B2", Sides.Black);
            moves = blackPawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m is PromotionMove));
        }

        [TestMethod]
        public void PromotionCaptureTest()
        {
            Board b = new Board();
            b.AddPiece("A8", PieceTypes.ROOK, Sides.Black);
            b.AddPiece("B8", PieceTypes.ROOK, Sides.Black);
            b.AddPiece("C8", PieceTypes.ROOK, Sides.Black);

            Pawn whitePawn = new Pawn("B7", Sides.White);
            var moves = whitePawn.Moves(b);
            Assert.AreEqual(8, moves.Count());
            Assert.IsTrue(moves.All(m => m is CapturePromotionMove));
            Assert.IsTrue(moves.Any(m => m.End == BitUtilities.AlgebraicToBit("A8")));
            Assert.IsTrue(moves.Any(m => m.End == BitUtilities.AlgebraicToBit("C8")));
        }

        [TestMethod]
        public void EnPassantTest()
        {
            Board b = new Board();
            b.AddPiece("A2", PieceTypes.PAWN, Sides.White);
            b.AddPiece("C4", PieceTypes.PAWN, Sides.White);
            b.ApplyMove(new Move(BitUtilities.AlgebraicToBit("A2"), BitUtilities.AlgebraicToBit("A4"), Sides.White));

            Pawn blackPawn = new Pawn("B4", Sides.Black);
            var moves = blackPawn.Moves(b);
            Assert.IsTrue(moves.Any(m => m is PassantMove));

            var passant = moves.Find(m => m is PassantMove);
            if(passant != null)
            {
                Assert.AreEqual(BitUtilities.AlgebraicToBit("B4"), passant.Start);
                Assert.AreEqual(BitUtilities.AlgebraicToBit("A3"), passant.End);
                Assert.AreEqual(BitUtilities.AlgebraicToBit("A4"), ((PassantMove)passant).TargetPosition);
            }
        }
    }
}

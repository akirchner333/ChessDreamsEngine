﻿using Engine;

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
            var targets = whitePawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("a2a3"));
            Assert.IsTrue(targets.Contains("a2a4"));

            Pawn blackPawn = new Pawn("a7", Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("a7a6"));
            Assert.IsTrue(targets.Contains("a7a5"));
        }

        [TestMethod]
        public void OneStepTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("a4", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("a4a5"), $"Expected to contain a4a5 actually contains {targets.First()}");

            Pawn blackPawn = new Pawn("a5", Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("a5a4"), $"Expected to contain a5a4 actually contains {targets.First()}");
        }

        [TestMethod]
        public void BlockedTest()
        {
            Board b = new Board();
            b.AddPiece(0, 2, PieceTypes.PAWN, Sides.Black);
            Pawn whitePawn = new Pawn("a2", Sides.White);
            var targets = whitePawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(0, targets.Count());

            Pawn blackPawn = new Pawn("a7", Sides.Black);
            b.AddPiece(0, 5, PieceTypes.PAWN, Sides.Black);
            targets = blackPawn.Moves(b).Select(m => m.LongAlgebraic());
            Assert.AreEqual(0, targets.Count());
        }

        [TestMethod]
        public void CaptureTest()
        {
            Board b = new Board("k7/8/PPPPPPPP/8/8/pppppppp/8/7K b - - 0 1");

            Pawn whitePawn = new Pawn("c2", Sides.White);
            var targets = Algebraic(whitePawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("c2b3"));
            Assert.IsTrue(targets.Contains("c2d3"));
            var moves = whitePawn.Moves(b);
            Assert.IsTrue(moves.All(m => m.Capture()));

            Pawn leftPawn = new Pawn("a2", Sides.White);
            targets = Algebraic(leftPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("a2b3"));

            Pawn rightPawn = new Pawn("h2", Sides.White);
            targets = Algebraic(rightPawn, b);
            Assert.AreEqual(1, targets.Count());
            Assert.IsTrue(targets.Contains("h2g3"));

            Pawn blackPawn = new Pawn("c7", Sides.Black);
            targets = Algebraic(blackPawn, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Contains("c7b6"));
            Assert.IsTrue(targets.Contains("c7d6"));

            var blackBoard = new Board("k7/7P/7P/7P/7P/7P/7P/7K w - - 0 1");
            var pawn = new Pawn("a4", Sides.Black);
            targets = Algebraic(pawn, blackBoard);
            Assert.AreEqual(1, targets.Count());
        }

        [TestMethod]
        public void PromotionTest()
        {
            Board b = new Board();
            Pawn whitePawn = new Pawn("b7", Sides.White);
            var moves = whitePawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m.Promoting()));
            Assert.IsTrue(moves.All(m => BitUtil.BitToAlgebraic(m.End) == "b8"));
            Assert.IsTrue(moves.Any(m => m.Promotion == PieceTypes.ROOK));
            Assert.IsTrue(moves.Any(m => m.Promotion == PieceTypes.QUEEN));
            Assert.IsTrue(moves.Any(m => m.Promotion == PieceTypes.BISHOP));
            Assert.IsTrue(moves.Any(m => m.Promotion == PieceTypes.KNIGHT));

            Pawn blackPawn = new Pawn("b2", Sides.Black);
            moves = blackPawn.Moves(b);
            Assert.AreEqual(4, moves.Count());
            Assert.IsTrue(moves.All(m => m.Promoting()));
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
            Assert.IsTrue(moves.All(m => m.Capture()));
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
            Assert.IsTrue(moves.Any(m => m.Passant()));
            var passant = Array.Find(moves, m => m.Passant());
            Assert.AreEqual(BitUtil.AlgebraicToBit("b4"), passant.Start);
            Assert.AreEqual(BitUtil.AlgebraicToBit("a3"), passant.End);
            Assert.AreEqual(BitUtil.AlgebraicToBit("a4"), passant.TargetSquare());
        }

        [TestMethod]
        public void CaptureOnlyTest()
        {
            var board = new Board("k5n1/5P2/8/1pP5/8/4r3/5P2/7K w - b6 0 1");
            var moves = board.MoveArray(true);

            Assert.AreEqual(6, moves.Length);
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "f2e3"));
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "c5b6"));
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "f7g8r"));
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "f7g8b"));
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "f7g8n"));
            Assert.IsTrue(moves.Any(m => m.LongAlgebraic() == "f7g8q"));
        }
    }
}

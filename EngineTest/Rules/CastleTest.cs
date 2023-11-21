﻿using Engine;
using Engine.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest.Rules
{
    [TestClass]
    public class CastleTest
    {
        [TestMethod]
        public void RightsUpdateTest()
        {
            var board = new Board("r3k2r/8/6N1/8/8/8/8/R3K2R w KQkq - 0 1");
            var castle = new Castling(board);
            Assert.AreEqual(0b1111, castle.CastleRights);
            var king = board.FindPiece(BitUtil.AlgebraicToBit("e1"));
            Move castleMove = new CastleMove(BitUtil.AlgebraicToBit("e1"), BitUtil.AlgebraicToBit("c1"), true)
            {
                RookStart = BitUtil.AlgebraicToBit("a1"),
                RookEnd = BitUtil.AlgebraicToBit("d1")
            };
            castleMove = castle.ApplyMove(castleMove, king!);
            Assert.AreEqual(0b1100, castle.CastleRights);
            castle.ReverseMove(castleMove);

            Move captureMove = Move.FromAlgebraic("g6", "h7", true, true);
            var knight = board.FindPiece(BitUtil.AlgebraicToBit("g6"));
            captureMove = castle.ApplyMove(captureMove, knight!);
            Assert.AreEqual(0b1011, castle.CastleRights);
            castle.ReverseMove(captureMove);
        }
    }
}

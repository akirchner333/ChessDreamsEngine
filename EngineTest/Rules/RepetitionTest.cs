﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace EngineTest.Rules
{
    [TestClass]
    public class RepetitionTest
    {
        [TestMethod]
        public void ThreeFoldTest()
        {
            var board = new Board("r1b4k/pppp4/8/8/8/8/4PPPP/K4B1R w - - 0 1");
            for(var i = 0; i < 3; i++)
            {
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("h1"), BitUtil.AlgebraicToBit("g1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("a8"), BitUtil.AlgebraicToBit("b8"), false));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("g1"), BitUtil.AlgebraicToBit("h1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("b8"), BitUtil.AlgebraicToBit("a8"), false));
            }

            Assert.IsTrue(board.DrawAvailable());
        }

        [TestMethod]
        public void NewBoardTest()
        {
            var board = new Board("r1b4k/pppp4/8/8/8/8/4PPPP/K4B1R w - - 0 1");
            for (var i = 0; i < 3; i++)
            {
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("h1"), BitUtil.AlgebraicToBit("g1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("a8"), BitUtil.AlgebraicToBit("b8"), false));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("g1"), BitUtil.AlgebraicToBit("h1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("b8"), BitUtil.AlgebraicToBit("a8"), false));
            }
            board.ApplyMove(new Move(BitUtil.AlgebraicToBit("a1"), BitUtil.AlgebraicToBit("b2"), true));

            Assert.IsFalse(board.DrawAvailable());
        }

        [TestMethod]
        public void FiveFoldTest()
        {
            var board = new Board("r1b4k/pppp4/8/8/8/8/4PPPP/K4B1R w - - 0 1");
            for (var i = 0; i < 6; i++)
            {
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("h1"), BitUtil.AlgebraicToBit("g1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("a8"), BitUtil.AlgebraicToBit("b8"), false));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("g1"), BitUtil.AlgebraicToBit("h1"), true));
                board.ApplyMove(new Move(BitUtil.AlgebraicToBit("b8"), BitUtil.AlgebraicToBit("a8"), false));
            }

            Assert.AreEqual(GameState.DRAW, board.State);
        }
    }
}

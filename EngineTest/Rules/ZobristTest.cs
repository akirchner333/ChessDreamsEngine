using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace EngineTest.Rules
{
    [TestClass]
    public class ZobristTest
    {
        [TestMethod]
        public void HashTest()
        {
            var board = Board.StartPosition();
            // It doesn't actually matter what this number is, just that it's the same everytime we run it
            Assert.AreEqual(6985536709956772916ul, board.Hash);

            var emptyBoard = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
            Assert.AreEqual(0ul, emptyBoard.Hash);

            var blackToMove = new Board("8/8/8/8/8/8/8/8 b - - 0 1");
            Assert.AreEqual(Board.TurnValue, blackToMove.Hash);
        }

        [TestMethod]
        public void MoveSequenceTest()
        {
            using (StreamReader r = new StreamReader("../../../move_sequence.csv"))
            {
                var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                string line;
                int halfmove = 0;
                List<ulong> hashes = new List<ulong>();

                while ((line = r.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var moves = board.Moves();
                    hashes.Add(board.Hash);
                    //Assert.IsTrue(hashes.Distinct().Count() == hashes.Count(), $"Hash collision at halfmove {halfmove}, before {parts[0]}");

                    var sameBoard = new Board(board.Fen());
                    Assert.AreEqual(sameBoard.Hash, board.Hash, $"Hash from fen doesn't match hash from moves at halfmove {halfmove},  before {parts[0]}  {board.Fen()}");

                    var nextMove = Array.Find(moves, m => m.LongAlgebraic() == parts[0]);
                    board.ApplyMove(nextMove!);
                    halfmove++;
                }
            }
        }
    }
}

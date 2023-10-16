using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace EngineTest
{
    [TestClass]
    public class QueenTest : PieceTest
    {
        [TestMethod]
        public void MoveTest()
        {
            Queen topLeft = new Queen("A1", true);
            Board b = new Board();

            var moves = EndPoints(topLeft, b);
            Assert.AreEqual(21, moves.Count());
            foreach(var square in new string[] {
                "A2", "A3", "A4", "A5", "A6", "A7", "A8", 
                "B1", "C1", "D1", "E1", "F1", "G1", "H1",
                "B2", "C3", "D4", "E5", "F6", "G7", "H8"
            })
            {
                Assert.IsTrue(moves.Any(m => m == square));
            }

            Queen bottomRight = new Queen("H8", true);
            moves = EndPoints(bottomRight, b);
            Assert.AreEqual(21, moves.Count());
            foreach (var square in new string[] {
                "H1", "H2", "H3", "H4", "H5", "H6", "H7",
                "A8", "B8", "C8", "D8", "E8", "F8", "G8",
                "A1", "B2", "C3", "D4", "E5", "F6", "G7"
            })
            {
                Assert.IsTrue(moves.Any(m => m == square));
            }

        }
    }
}

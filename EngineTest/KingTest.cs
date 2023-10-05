using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class KingTest
    {
        [TestMethod]
        public void TestMoves()
        {
            var b = new Board();
            var king = new King("D3", Sides.White);
            var targets = king.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(8, targets.Count());

            string[] expected = new string[] { "C2", "C3", "C4", "D2", "D4", "E2", "E3", "E4" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var topCornerKing = new King("A1", Sides.White);
            targets = topCornerKing.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "A2", "B1", "B2" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }

            var bottomCornerKing = new King("H8", Sides.White);
            targets = bottomCornerKing.Moves(b).Select(m => m.EndAlgebraic());
            Assert.AreEqual(3, targets.Count());

            expected = new string[] { "H7", "G7", "G8" };
            foreach (var target in expected)
            {
                Assert.IsTrue(targets.Contains(target));
            }
        }
    }
}

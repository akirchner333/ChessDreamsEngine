using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class KnightTest : PieceTest
    {
        [TestMethod]
        public void MoveTest()
        {
            Board b = new Board();
            Knight middle = new Knight("D4", Sides.White);
            var targets = EndPoints(middle, b);
            Assert.AreEqual(8, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "C6"));
            Assert.IsTrue(targets.Any(m => m == "E6"));
            Assert.IsTrue(targets.Any(m => m == "B5"));
            Assert.IsTrue(targets.Any(m => m == "F5"));
            Assert.IsTrue(targets.Any(m => m == "B3"));
            Assert.IsTrue(targets.Any(m => m == "F3"));
            Assert.IsTrue(targets.Any(m => m == "C2"));
            Assert.IsTrue(targets.Any(m => m == "E2"));

            Knight topLeft = new Knight("A8", Sides.White);
            targets = EndPoints(topLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "B6"));
            Assert.IsTrue(targets.Any(m => m == "C7"));

            Knight bottomLeft = new Knight("H1", Sides.White);
            targets = EndPoints(bottomLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "F2"));
            Assert.IsTrue(targets.Any(m => m == "G3"));
        }
        
    }
}
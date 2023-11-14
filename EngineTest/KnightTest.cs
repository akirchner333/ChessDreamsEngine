using Engine;

namespace EngineTest
{
    [TestClass]
    public class KnightTest : PieceTest
    {
        [TestMethod]
        public void MoveTest()
        {
            Board b = new Board();
            Knight middle = new Knight("d4", Sides.White);
            var targets = EndPoints(middle, b);
            foreach (var target in targets)
            {
                Console.WriteLine(target);
            }
            Assert.AreEqual(8, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "c6"));
            Assert.IsTrue(targets.Any(m => m == "e6"));
            Assert.IsTrue(targets.Any(m => m == "b5"));
            Assert.IsTrue(targets.Any(m => m == "f5"));
            Assert.IsTrue(targets.Any(m => m == "b3"));
            Assert.IsTrue(targets.Any(m => m == "f3"));
            Assert.IsTrue(targets.Any(m => m == "c2"));
            Assert.IsTrue(targets.Any(m => m == "e2"));

            Knight topLeft = new Knight("a8", Sides.White);
            targets = EndPoints(topLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "b6"));
            Assert.IsTrue(targets.Any(m => m == "c7"));

            Knight bottomLeft = new Knight("h1", Sides.White);
            targets = EndPoints(bottomLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "f2"));
            Assert.IsTrue(targets.Any(m => m == "g3"));
        }

    }
}
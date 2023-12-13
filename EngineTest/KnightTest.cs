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
            var targets = Algebraic(middle, b);
            foreach (var target in targets)
            {
                Console.WriteLine(target);
            }
            Assert.AreEqual(8, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "d4c6"));
            Assert.IsTrue(targets.Any(m => m == "d4e6"));
            Assert.IsTrue(targets.Any(m => m == "d4b5"));
            Assert.IsTrue(targets.Any(m => m == "d4f5"));
            Assert.IsTrue(targets.Any(m => m == "d4b3"));
            Assert.IsTrue(targets.Any(m => m == "d4f3"));
            Assert.IsTrue(targets.Any(m => m == "d4c2"));
            Assert.IsTrue(targets.Any(m => m == "d4e2"));

            Knight topLeft = new Knight("a8", Sides.White);
            targets = Algebraic(topLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "a8b6"));
            Assert.IsTrue(targets.Any(m => m == "a8c7"));

            Knight bottomLeft = new Knight("h1", Sides.White);
            targets = Algebraic(bottomLeft, b);
            Assert.AreEqual(2, targets.Count());
            Assert.IsTrue(targets.Any(m => m == "h1f2"));
            Assert.IsTrue(targets.Any(m => m == "h1g3"));
        }

    }
}
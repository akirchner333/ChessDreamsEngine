using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class BitUtilitiesTest
    {
        [TestMethod]
        public void TestBitToAlgebraic()
        {
            Assert.AreEqual("A1", BitUtilities.BitToAlgebraic(1));
            Assert.AreEqual("B1", BitUtilities.BitToAlgebraic(2));
            Assert.AreEqual("H1", BitUtilities.BitToAlgebraic(1 << 7));
            Assert.AreEqual("A2", BitUtilities.BitToAlgebraic(1 << 8));
            Assert.AreEqual("A8", BitUtilities.BitToAlgebraic((ulong)1 << 56));
            Assert.AreEqual("H8", BitUtilities.BitToAlgebraic((ulong)1 << 63));
        }

        [TestMethod]
        public void TestAlgebraicToBit()
        {
            Assert.AreEqual((ulong)1, BitUtilities.AlgebraicToBit("A1"));
            Assert.AreEqual((ulong)2, BitUtilities.AlgebraicToBit("B1"));
            Assert.AreEqual((ulong)1 << 7, BitUtilities.AlgebraicToBit("H1"));
            Assert.AreEqual((ulong)1 << 8, BitUtilities.AlgebraicToBit("A2"));
            Assert.AreEqual((ulong)1 << 56, BitUtilities.AlgebraicToBit("A8"));
            Assert.AreEqual((ulong)1 << 63, BitUtilities.AlgebraicToBit("H8"));
        }
    }
}
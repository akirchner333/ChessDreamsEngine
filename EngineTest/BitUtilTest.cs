using Engine;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace EngineTest
{
    [TestClass]
    public class BitUtilTest
    {
        [TestMethod]
        public void TestBitToAlgebraic()
        {
            Assert.AreEqual("A1", BitUtil.BitToAlgebraic(1));
            Assert.AreEqual("B1", BitUtil.BitToAlgebraic(2));
            Assert.AreEqual("H1", BitUtil.BitToAlgebraic(1 << 7));
            Assert.AreEqual("A2", BitUtil.BitToAlgebraic(1 << 8));
            Assert.AreEqual("A8", BitUtil.BitToAlgebraic(1ul << 56));
            Assert.AreEqual("H8", BitUtil.BitToAlgebraic(1ul << 63));
        }

        [TestMethod]
        public void TestAlgebraicToBit()
        {
            Assert.AreEqual(1ul, BitUtil.AlgebraicToBit("A1"));
            Assert.AreEqual(2ul, BitUtil.AlgebraicToBit("B1"));
            Assert.AreEqual(1ul << 7, BitUtil.AlgebraicToBit("H1"));
            Assert.AreEqual(1ul << 8, BitUtil.AlgebraicToBit("A2"));
            Assert.AreEqual(1ul << 56, BitUtil.AlgebraicToBit("A8"));
            Assert.AreEqual(1ul << 63, BitUtil.AlgebraicToBit("H8"));
        }

        [TestMethod]
        public void OverlapTest()
        {
            Assert.IsTrue(BitUtil.Overlap(
                0b101010001010ul,
                0b011101110101ul)
            );

            Assert.IsFalse(BitUtil.Overlap(
                0b101010001010ul,
                0b010101110101ul)
            );
        }

        [TestMethod]
        public void RemoveTest()
        {
            ulong a = 0b11111111;
            ulong b = 0b00101000;

            Assert.AreEqual(0b11010111ul, BitUtil.Remove(a, b));
            Assert.AreEqual(0ul, BitUtil.Remove(0, b));
            Assert.AreEqual(a, BitUtil.Remove(a, 0));
        }

        [TestMethod]
        public void SplitBitsTest()
        {
            var bits = BitUtil.SplitBits(0b10101101010);
            Assert.AreEqual(6, bits.Count());
            Assert.AreEqual(         0b10ul, bits[0]);
            Assert.AreEqual(       0b1000ul, bits[1]);
            Assert.AreEqual(     0b100000ul, bits[2]);
            Assert.AreEqual(    0b1000000ul, bits[3]);
            Assert.AreEqual(  0b100000000ul, bits[4]);
            Assert.AreEqual(0b10000000000ul, bits[5]);

            Assert.AreEqual(64, BitUtil.SplitBits(ulong.MaxValue).Count());
            Assert.AreEqual(0, BitUtil.SplitBits(0ul).Count());
        }
    }
}
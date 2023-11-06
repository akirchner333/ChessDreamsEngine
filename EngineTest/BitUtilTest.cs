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
            Assert.AreEqual("a1", BitUtil.BitToAlgebraic(1));
            Assert.AreEqual("b1", BitUtil.BitToAlgebraic(2));
            Assert.AreEqual("h1", BitUtil.BitToAlgebraic(1 << 7));
            Assert.AreEqual("a2", BitUtil.BitToAlgebraic(1 << 8));
            Assert.AreEqual("a8", BitUtil.BitToAlgebraic(1ul << 56));
            Assert.AreEqual("h8", BitUtil.BitToAlgebraic(1ul << 63));
        }

        [TestMethod]
        public void TestAlgebraicToBit()
        {
            Assert.AreEqual(1ul, BitUtil.AlgebraicToBit("a1"));
            Assert.AreEqual(2ul, BitUtil.AlgebraicToBit("b1"));
            Assert.AreEqual(1ul << 7, BitUtil.AlgebraicToBit("h1"));
            Assert.AreEqual(1ul << 8, BitUtil.AlgebraicToBit("a2"));
            Assert.AreEqual(1ul << 56, BitUtil.AlgebraicToBit("a8"));
            Assert.AreEqual(1ul << 63, BitUtil.AlgebraicToBit("h8"));
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

        [TestMethod]
        public void BitToIndexTest()
        {
            Assert.AreEqual(5, BitUtil.BitToIndex(1 << 5));
        }

        [TestMethod]
        public void FillTest()
        {
            Assert.AreEqual(16711680ul, BitUtil.Fill(16, 23));
            Assert.AreEqual(1048574ul, BitUtil.Fill(1, 19));
        }

        [TestMethod]
        public void SouthFillTest()
        {
            Assert.AreEqual(18085043209519168ul, BitUtil.SouthFill(18014398509481984));
            Assert.AreEqual(514ul, BitUtil.SouthFill(512));
        }

        [TestMethod]
        public void NorthFillTest()
        {
            Assert.AreEqual(4629700416936869888ul, BitUtil.NorthFill(18014398509481984));
            Assert.AreEqual(144680345676153344ul, BitUtil.NorthFill(512));
        }
    }
}
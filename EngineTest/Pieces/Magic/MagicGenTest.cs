using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Pieces.Magic;

namespace EngineTest.Pieces.Movers
{
    [TestClass]
    public class MagicGenTest
    {
        [TestMethod]
        public void AllVariantsTest()
        {
            var variants = TestMagic.AllVariants(0b10);
            Assert.AreEqual(0ul, variants[0]);
            Assert.AreEqual(2ul, variants[1]);

            variants = TestMagic.AllVariants(0b1_00000_1_0000_1_0000_1ul);
            Assert.AreEqual(0ul, variants[0]);
            Assert.AreEqual(0b0_00000_0_0000_0_0000_1ul, variants[1]);
            Assert.AreEqual(0b0_00000_0_0000_1_0000_0ul, variants[2]);
            Assert.AreEqual(0b0_00000_0_0000_1_0000_1ul, variants[3]);
            Assert.AreEqual(0b0_00000_1_0000_0_0000_0ul, variants[4]);
            Assert.AreEqual(0b0_00000_1_0000_0_0000_1ul, variants[5]);
            Assert.AreEqual(0b0_00000_1_0000_1_0000_0ul, variants[6]);
            Assert.AreEqual(0b0_00000_1_0000_1_0000_1ul, variants[7]);
            Assert.AreEqual(0b1_00000_0_0000_0_0000_0ul, variants[8]);
            Assert.AreEqual(0b1_00000_0_0000_0_0000_1ul, variants[9]);
            Assert.AreEqual(0b1_00000_0_0000_1_0000_0ul, variants[10]);
            Assert.AreEqual(0b1_00000_0_0000_1_0000_1ul, variants[11]);
            Assert.AreEqual(0b1_00000_1_0000_0_0000_0ul, variants[12]);
            Assert.AreEqual(0b1_00000_1_0000_0_0000_1ul, variants[13]);
            Assert.AreEqual(0b1_00000_1_0000_1_0000_0ul, variants[14]);
            Assert.AreEqual(0b1_00000_1_0000_1_0000_1ul, variants[15]);
        }

        [TestMethod]
        public void AllUniqueTest()
        {
            Assert.IsTrue(TestMagic.AllUnique(new ulong[5] { 1, 2, 3, 4, 5 }));
            Assert.IsTrue(TestMagic.AllUnique(new ulong[1] { 0 }));
            Assert.IsFalse(TestMagic.AllUnique(new ulong[6] { 1, 1, 2, 3, 4, 5 }));
            Assert.IsFalse(TestMagic.AllUnique(new ulong[2] { 0, 0 }));

        }
    }
}

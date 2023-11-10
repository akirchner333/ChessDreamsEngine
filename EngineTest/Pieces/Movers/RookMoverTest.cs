using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Pieces.Movers;

namespace EngineTest.Pieces.Movers
{
    [TestClass]
    public class RookCalcTest
    {
        [TestMethod]
        public void EmptyMaskTest()
        {
            var calc = new RookCalc();
            Assert.AreEqual(282578800148862ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("a1")));
            Assert.AreEqual(282580897300736ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("a4")));
            Assert.AreEqual(9079539427579068672ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("a8")));
            Assert.AreEqual(7930856604974452736ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("e8")));
            Assert.AreEqual(9115426935197958144ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("h8")));
            Assert.AreEqual(36170077829103616ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("h5")));
            Assert.AreEqual(36170086419038334ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("h1")));
            Assert.AreEqual(4521260802379886ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("e1")));
            Assert.AreEqual(4521262379438080ul, calc.EmptyMask(BitUtil.AlgebraicToIndex("e4")));
        }
    }
}

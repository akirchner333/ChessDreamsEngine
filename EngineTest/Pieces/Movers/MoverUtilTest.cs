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
    public class MoverUtilTest
    {
        [TestMethod]
        public void TestBlockers()
        {
            Assert.AreEqual(Board.Columns["A"], MoverUtil.Blocker(1));
            Assert.AreEqual(Board.Columns["A"], MoverUtil.Blocker(9));
            Assert.AreEqual(Board.Columns["A"], MoverUtil.Blocker(-7));
            Assert.AreEqual(Board.Columns["H"], MoverUtil.Blocker(-1));
            Assert.AreEqual(Board.Columns["H"], MoverUtil.Blocker(-9));
            Assert.AreEqual(Board.Columns["H"], MoverUtil.Blocker(7));
        }
    }
}

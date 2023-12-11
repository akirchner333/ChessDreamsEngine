using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest
{
    [TestClass]
    public class FastStackTest
    {
        [TestMethod]
        public void StackTest()
        {
            var stack = new FastStack<int>(16);
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            Assert.AreEqual(3, stack.Peek());
            Assert.AreEqual(3, stack.Pop());
            stack.Push(4);
            Assert.AreEqual(4, stack.Peek());
            Assert.AreEqual(4, stack.Pop());
            Assert.AreEqual(2, stack.Peek());
            Assert.AreEqual(2, stack.Pop());
            Assert.AreEqual(1, stack.Peek());
        }
    }
}

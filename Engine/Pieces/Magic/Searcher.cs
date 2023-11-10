using Engine.Pieces.Movers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engine.Pieces.Magic
{
    public class Searcher
    {
        public Random rand { get; private set; } = new Random();

        public TestMagic RookTester { get; private set; }
        public TestMagic BishopTester { get; private set; }

        public Searcher()
        {
            MagicDatabase.FillNewPiece("Bishop");
            MagicDatabase.FillNewPiece("Rook");

            RookTester = new TestMagic(new RookMover(), "Rook");
            BishopTester = new TestMagic(new BishopMover(), "Bishop");
        }

        public void Search(int tryCount)
        {
            for (int tries = 0; tries < tryCount; tries++)
            {
                var key = RandomKey();

                RookTester.TestKey(key);
                BishopTester.TestKey(key);
                if(tries % 500 == 0)
                {
                    Console.WriteLine(tries);
                    Save();
                }
            }
            Save();
        }

        public void Save()
        {
            MagicDatabase.Save("Rook", RookTester.XMLMagic());
            MagicDatabase.Save("Bishop", BishopTester.XMLMagic());
        }

        //For trying specific values. Good for injecting other people's magics
        public void Try(ulong[] magics)
        {
            foreach(var m in magics)
            {
                RookTester.TestKey(m);
                BishopTester.TestKey(m);
            }
        }

        public void Print()
        {
            Console.WriteLine("~~~~~ROOKS~~~~~~~");
            RookTester.PrintMagics();
            Console.WriteLine("~~~~~~BISHOPS~~~~~~");
            BishopTester.PrintMagics();
        }

        public void Stats()
        {
            RookTester.Stats();
            BishopTester.Stats();
        }

        public void ClearAll()
        {
            RookTester.Clear();
            BishopTester.Clear();
            Save();
        }

        public ulong RandomKey()
        {
            return (ulong)((rand.NextInt64() << 1) ^ rand.Next());
        }
    }
}

using Engine.Pieces.Movers;

namespace Engine.Pieces.Magic
{
    public class Searcher
    {
        private readonly Random _searchRand = new();

        public TestMagic RookTester { get; private set; }
        public TestMagic BishopTester { get; private set; }

        public Searcher()
        {
            MagicDatabase.FillNewPiece("Bishop");
            MagicDatabase.FillNewPiece("Rook");

            RookTester = new TestMagic(new RookCalc(), "Rook");
            BishopTester = new TestMagic(new BishopCalc(), "Bishop");
        }

        public void Search(int tryCount)
        {
            for (int tries = 0; tries < tryCount; tries++)
            {
                var key = Rand.RandULong(_searchRand);

                RookTester.TestKey(key);
                BishopTester.TestKey(key);
                if (tries % 500 == 0)
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
            foreach (var m in magics)
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
    }
}

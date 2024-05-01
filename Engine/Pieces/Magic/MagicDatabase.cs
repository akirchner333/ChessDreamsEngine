using System.Xml.Linq;

namespace Engine.Pieces.Magic
{
    public static class MagicDatabase
    {
        // I have not yet figured out how to load this XML file in a manner
        // that would work on other people's computers. It hasn't been a priority
        public static string XMLPath { get; private set; } = @"C:\Users\akirc\OneDrive\Desktop\Projects\Engine\Engine\Pieces\Magic\KnownMagic.xml";
        public static XDocument KnownMagics { get; private set; }

        static MagicDatabase()
        {
            KnownMagics = XDocument.Load(XMLPath);
        }

        public static ulong[] LoadMagics(string piece)
        {
            return KnownMagics.Root!.Element(piece)!.Element("magics")!.Elements("value").Select(x => (ulong)x).ToArray();
        }

        public static int[] LoadOffsets(string piece)
        {
            return KnownMagics.Root!.Element(piece)!.Element("offsets")!.Elements("value").Select(x => (int)x).ToArray();
        }

        public static void FillNewPiece(string piece)
        {
            if (KnownMagics.Root!.Element(piece) == null)
            {
                var emptyMagics = new ulong[64];
                Array.Fill<ulong>(emptyMagics, 1);
                var emptyOffsets = new int[64];
                Array.Fill<int>(emptyOffsets, 0);
                KnownMagics.Root.Add(new XElement(
                    piece,
                    new XElement(
                        "magics",
                        from x in emptyMagics
                        select new XElement("value", x)
                    ),
                    new XElement(
                        "offsets",
                        from x in emptyOffsets
                        select new XElement("value", x)
                    )
                ));
                KnownMagics.Save(XMLPath);
            }
        }

        public static void Save(string piece, XElement input)
        {
            KnownMagics.Root.Element(piece).ReplaceWith(input);
            KnownMagics.Save(XMLPath);
        }
    }
}

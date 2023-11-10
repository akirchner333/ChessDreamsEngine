using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine.Pieces.Movers;

namespace Engine.Pieces.Magic
{
    public class TestMagic
    {
        public string Name { get; set; }
        private IMagicRider _mover;
        public ulong[] Magics { get; private set; }
        public int[] Offsets { get; private set; }
        public ulong[][] Blockers { get; private set;  } = new ulong[64][];
        public TestMagic(IMagicRider mover, string name)
        {
            _mover = mover;
            Magics = MagicDatabase.LoadMagics(name);
            Offsets = MagicDatabase.LoadOffsets(name);
            Name = name;

            GenerateBlockers();
        }

        public void GenerateBlockers()
        {
            for(var i = 0; i < 64; i++)
            {
                var mask = _mover.EmptyMask(i);
                Blockers[i] = AllVariants(mask);
            }
        }

        public void TestKey(ulong key)
        {
            for (var i = 0; i < 64; i++)
            {
                var keyOffset = FindOffset(key, Blockers[i], Offsets[i]);
                if (keyOffset > Offsets[i])
                {
                    Console.WriteLine($"New {Name} magic {i}, {Offsets[i]} -> {keyOffset}");
                    Offsets[i] = keyOffset;
                    Magics[i] = key;
                }
            }
        }

        public int FindOffset(ulong key, ulong[] blockers, int currentOffset)
        {
            var highest = 0;
            for (var i = currentOffset + 1; i < 64; i++)
            {
                var keys = MapBlockers(blockers, key, i);
                if (AllUnique(keys))
                {
                    highest = i;
                }
            }
            return highest;
        }

        public bool CheckOffset(int i)
        {
            var keys = MapBlockers(Blockers[i], Magics[i], Offsets[i]);
            return AllUnique(keys);
        }

        public ulong[] MapBlockers(ulong[] blockers, ulong magic, int offset)
        {
            return blockers.Select(b => MakeKey(b, magic, offset)).ToArray();
        }

        public ulong MakeKey(ulong blocker, ulong magic, int offset)
        {
            return (blocker * magic) >> offset;
        }

        public void Clear()
        {
            for(var i = 0; i < 64; i++)
            {
                Magics[i] = 1;
                Offsets[i] = 0;
            }
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ OUTPUT ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void PrintMagics()
        {
            for (var i = 0; i < 8; i++)
            {
                var line = String.Join(", ", Magics.Skip(i * 8).Take(8));
                Console.WriteLine(line + ",");
            }

            for (var i = 0; i < 8; i++)
            {
                var line = String.Join(", ", Offsets.Skip(i * 8).Take(8));
                Console.WriteLine(line + ",");
            }
        }

        public void Stats()
        {
            var max = Enumerable.Max(Offsets);
            var min = Enumerable.Min(Offsets);
            var avg = Enumerable.Average(Offsets);
            Console.WriteLine($"Largest {Name} offset {max}, smallest {min}, average {avg}");
        }

        public XElement XMLMagic()
        {
            var magicElement = new XElement(
                "magics",
                from x in Magics
                select new XElement("value", x)
            );
            var offsetElement = new XElement(
                "offsets",
                from x in Offsets
                select new XElement("value", x)
            );
            return new XElement(Name, magicElement, offsetElement);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ UTILITIES ~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static ulong[] AllVariants(ulong mask)
        {
            var length = (ulong)Math.Pow(2, BitOperations.PopCount(mask));
            var variants = new ulong[length];
            for (var i = 0ul; i < length; i++)
            {
                BitUtil.SplitBitsNew(mask, (ulong bit, int bitIndex) =>
                {
                    variants[i] |= BitUtil.Overlap(i, 1ul << bitIndex) ? bit : 0;
                });
            }
            return variants;
        }

        // Are all the values in this array unique?
        public static bool AllUnique(ulong[] values)
        {
            return values.Count() == values.Distinct().Count();
        }
    }
}

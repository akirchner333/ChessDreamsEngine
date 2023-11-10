using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public interface IMagicRider
    {
        ulong EmptyMask(int i);
        ulong CalculateMask(int i, ulong pieces);
    }

    public class Rider
    {
        public string Key { get; private set; }
        public bool Side { get; set; }
        private static Dictionary<string, ulong[]> _maskDatabase = new Dictionary<string, ulong[]>();

        public Rider(int[] directions, string key)
        {
            Key = key;
            if (!_maskDatabase.ContainsKey(key))
                _maskDatabase.Add(Key, FillMaskDatabase(directions));
        }

        public ulong[] FillMaskDatabase(int[] directions)
        {
            var masks = new ulong[64];
            for(var i = 0; i < 64; i++)
            {
                masks[i] = 11;
            }
            return masks;
        }
    }
}

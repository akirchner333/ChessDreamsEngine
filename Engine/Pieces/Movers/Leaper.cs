namespace Engine.Pieces.Movers
{
    public class Leaper : IMover
    {
        public string Key { get; private set; }
        public bool Side { get; set; }
        private static Dictionary<string, ulong[]> _moveDatabase = new Dictionary<string, ulong[]>();
        public Leaper(int[] directions, string key)
        {
            Key = key;
            if (!_moveDatabase.ContainsKey(key))
                _moveDatabase.Add(Key, FillMoveDatabase(directions));
        }

        public ulong RawMask(int index)
        {
            return GetMoveDatabase(index);
        }

        public virtual ulong MoveMask(int index, Board board)
        {
            return BitUtil.Remove(GetMoveDatabase(index), board.SidePieces(Side));
        }

        public ulong[] FillMoveDatabase(int[] directions)
        {
            var moves = new ulong[64];
            for (var i = 0; i < 64; i++)
            {
                var index = 1ul << i;
                moves[i] = MovesAtIndex(index, directions);
            }
            return moves;
        }

        public ulong MovesAtIndex(ulong index, int[] directions)
        {
            return directions
                .Select(d => LeaperMask(d, MoverUtil.Blocker(d), index))
                .Aggregate(0ul, (acc, x) => acc | x);
        }

        public static ulong LeaperMask(int steps, ulong blocker, ulong position)
        {
            ulong move = MoverUtil.Shifter(steps, position)(1);

            return BitUtil.Remove(move, blocker);
        }
        public ulong GetMoveDatabase(int index)
        {
            return _moveDatabase[Key][index];
        }
    }
}

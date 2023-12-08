using Engine.Pieces.Magic;

namespace Engine.Pieces.Movers
{
    public class RiderData
    {
        public ulong[] EmptyMasks = new ulong[64];
        public ulong[][] MoveDB = new ulong[64][];
        public ulong[] Magics { get; set; } = new ulong[64];
        public int[] Offset { get; set; } = new int[64];

        public RiderData(string pieceName, IRiderCalc calc)
        {
            Magics = MagicDatabase.LoadMagics(pieceName);
            Offset = MagicDatabase.LoadOffsets(pieceName);

            for (var i = 0; i < 64; i++)
            {
                EmptyMasks[i] = calc.EmptyMask(i);

                var magicNum = Magics[i];
                var offset = Offset[i];
                MoveDB[i] = new ulong[(ulong)Math.Pow(2, 64 - offset)];

                foreach (var m in TestMagic.AllVariants(EmptyMasks[i]))
                {
                    var key = (m * magicNum) >> offset;
                    MoveDB[i][key] = calc.CalculateMask(i, m);
                }
            }
        }

        public ulong MoveMask(int i, Board board, bool side)
        {
            var key = Key(i, board.AllPieces);
            return BitUtil.Remove(MoveDB[i][key], board.SidePieces(side));
        }

        public ulong RawMask(int i, ulong occ)
        {
            var key = Key(i, occ);
            return MoveDB[i][key];
        }

        public ulong XRayAttacks(int i, Board board, bool side)
        {
            var moves = MoveMask(i, board, side);
            var attacks = moves & board.SidePieces(!side);
            var removedPieces = BitUtil.Remove(board.AllPieces, attacks);

            var key = Key(i, removedPieces);
            return BitUtil.Remove(MoveDB[i][key], attacks) & board.SidePieces(!side);
        }

        // Kind of returns two different things, depending on the situation
        // If there are no pieces between the two squares, it returns the squares between them
        // If there's one piece, it returns that piece's bit
        // All other cases, it returns 0
        // Can't think of a good name to cover both those cases
        public ulong PathBetween(int i, int targetIndex, Board board, bool side)
        {
            if (!BitUtil.Overlap(RawMask(i, 0), BitUtil.IndexToBit(targetIndex)))
                return 0;

            var moves = MoveMask(i, board, side);
            var targetMoves = MoveMask(targetIndex, board, side);
            return moves & targetMoves;
        }

        private ulong Key(int i, ulong occ)
        {
            var mask = EmptyMasks[i];
            return ((mask & occ) * Magics[i]) >> Offset[i];
        }
    }
}

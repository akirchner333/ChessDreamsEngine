using Engine.Rules;

namespace Engine.Pieces.Movers
{
    public class CastleMover
    {
        public bool Side { get; set; }
        public CastleMover() { }

        public const ulong a1 = 1;
        public const ulong c1 = 4ul;
        public const ulong g1 = 64ul;
        public const ulong h1 = 128ul;
        public const ulong a8 = 72057594037927936;
        public const ulong c8 = 288230376151711744;
        public const ulong g8 = 4611686018427387904;
        public const ulong h8 = 9223372036854775808;

        public const ulong whiteKingside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01100000;
        public const ulong whiteQueenside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001110;
        public const ulong blackKingside = 0b01100000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        public const ulong blackQueenside = 0b00001110_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

        public const ulong whiteQueenMove = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001100;
        public const ulong blackQueenMove = 0b00001100_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

        // I am hardcoding a lot of castling stuff for now since castling is a weird move
        // We are assuming the king starts at E1/E8, and the Rooks are in the corners
        // Kingside castles move the King from E to G, Rook from H to F
        // Queenside castles move the King from E to C, Rook from A to D
        public List<Move> Moves(Board board, ulong Position)
        {
            List<Move> list = new List<Move>();

            if (board.LegalMoves.Attacked(Side, Position))
                return list;

            if (Side)
            {
                if (CastleLegal(Castles.WhiteKingside, board))
                    list.Add(Move.CastleMove(Position, g1, h1, Side));
                if (CastleLegal(Castles.WhiteQueenside, board))
                    list.Add(Move.CastleMove(Position, c1, a1, Side));
            }
            else
            {
                if (CastleLegal(Castles.BlackKingside, board))
                    list.Add(Move.CastleMove(Position, g8, h8, Side));
                if (CastleLegal(Castles.BlackQueenside, board))
                    list.Add(Move.CastleMove(Position, c8, a8, Side));
            }
            return list;
        }

        public bool CastleLegal(Castles castle, Board b)
        {
            // The squares that the kill will move through - none of them can be attacked.
            // The same as empty squares for kingside but different for queenside
            var moveThrough = castle switch
            {
                Castles.BlackKingside => blackKingside,
                Castles.BlackQueenside => blackQueenMove,
                Castles.WhiteQueenside => whiteQueenMove,
                Castles.WhiteKingside => whiteKingside,
                _ => 0ul
            };
            var attacked = BitUtil.SplitBits(moveThrough).Any(x => b.LegalMoves.Attacked(Side, x));

            //The squares which must be empty
            var emptyMask = castle switch
            {
                Castles.BlackKingside => blackKingside,
                Castles.BlackQueenside => blackQueenside,
                Castles.WhiteQueenside => whiteQueenside,
                Castles.WhiteKingside => whiteKingside,
                _ => 0ul
            };

            return b.Castles.HasCastleRights(castle) && !BitUtil.Overlap(b.AllPieces, emptyMask) && !attacked;
        }
    }
}

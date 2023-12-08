using Engine.Pieces.MoveFactories;

namespace Engine
{

    public interface IPiece
    {
        string Name { get; }
        PieceTypes Type { get; }
        char Short { get; }
    }
    public abstract class Piece : IPiece
    {
        public abstract string Name { get; }
        public abstract PieceTypes Type { get; }
        public abstract char Short { get; }

        public virtual int Rank { get { return 999; } }

        public ulong Position { get; set; }
        public int Index { get; set; }
        public bool Side;
        public bool Captured { get; set; } = false;
        public int CastleRights { get; set; } = 0;
        protected MoveFactory _convert { get; set; } = new MoveFactory();

        public Piece(ulong bit, bool side)
        {
            Side = side;
            Position = bit;
            Index = BitUtil.BitToIndex(bit);
            _convert.Side = side;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INFO ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public string PieceSprite()
        {
            return (Side ? "White" : "Black") + Name;
        }

        protected ulong allyPieces(Board b)
        {
            return Side ? b.WhitePieces : b.BlackPieces;
        }

        protected ulong enemyPieces(Board b)
        {
            return Side ? b.BlackPieces : b.WhitePieces;
        }

        public bool Active(bool side)
        {
            return side == this.Side && !Captured;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Move Generation ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public abstract ulong MoveMask(Board b);

        public virtual ulong AttackMask(Board b)
        {
            return MoveMask(b);
        }

        public virtual ulong XRayAttacks(Board b)
        {
            return 0;
        }

        public virtual ulong PathBetween(Board b, int index)
        {
            return 0;
        }

        // In many cases just breaking up the mask (am I using that term correctly?) will be enough
        // But for stuff like en passant or castling, we'll need to extend this method to add in some extra stuff
        public virtual Move[] Moves(Board b)
        {
            var mask = MoveMask(b);
            return _convert.ConvertMask(b, Position, mask);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ MOVE APPLICATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public virtual Move ApplyMove(Move m)
        {
            Position = m.End;
            Index = BitUtil.BitToIndex(m.End);
            return m;
        }

        public virtual Move ReverseMove(Move m)
        {
            Position = m.Start;
            Index = BitUtil.BitToIndex(m.Start);
            return m;
        }
    }
}
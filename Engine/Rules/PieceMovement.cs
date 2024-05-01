namespace Engine.Rules
{
    // Handles movement
    public class PieceMovement
    {
        private Board _board;

        // Putting piece count in a const here to make it easier in the future, when piece count might be variable
        const int pieceCount = 6;
        private static ulong[] _values;
        private static PieceTypes[] _pieces = new PieceTypes[pieceCount]
        {
            PieceTypes.PAWN,
            PieceTypes.ROOK,
            PieceTypes.KNIGHT,
            PieceTypes.BISHOP,
            PieceTypes.QUEEN,
            PieceTypes.KING
        };

        static PieceMovement()
        {
            _values = Rand.RandULongArray(pieceCount * 2 * 64);
        }
        public PieceMovement(Board board)
        {
            _board = board;
            foreach (var piece in _board.Pieces)
            {
                TogglePiece(piece);
            }
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            return MovePiece(_board.Pieces[pieceIndex], move.Start, move.End, move);
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            MovePiece(_board.Pieces[pieceIndex], move.End, move.Start, move);
        }

        public Move MovePiece(Piece p, ulong start, ulong end, Move m)
        {
            TogglePiece(p);
            p.Move(end);
            TogglePiece(p);

            if (p.Side == Sides.Black)
            {
                _board.BlackPieces = BitUtil.Remove(_board.BlackPieces, start);
                _board.BlackPieces |= end;
            }
            else
            {
                _board.WhitePieces = BitUtil.Remove(_board.WhitePieces, start);
                _board.WhitePieces |= end;
            }

            _board.AllPieces = _board.BlackPieces | _board.WhitePieces;

            return m;
        }

        public void TogglePiece(Piece p)
        {
            _board.Hash ^= _values[GetPieceIndex(p)];
        }

        public int GetPieceIndex(Piece p)
        {
            var startIndex = Array.FindIndex(_pieces, type => type == p.Type) * 64;
            startIndex += p.Side ? 0 : 64 * pieceCount;

            return startIndex + p.Index;
        }
    }
}

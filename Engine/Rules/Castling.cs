namespace Engine.Rules
{
    public enum Castles
    {
        WhiteKingside = 0b0001,
        WhiteQueenside = 0b0010,
        BlackKingside = 0b0100,
        BlackQueenside = 0b1000
    }

    public class Castling
    {
        public int CastleRights { get; set; } = 0b1111;
        private FastStack<int> _castleStack = new FastStack<int>(16);
        private Board _board;
        private static ulong[] _values;

        static Castling()
        {
            _values = Rand.RandULongArray(16);
            _values[0] = 0;
        }

        public Castling(Board board)
        {
            _board = board;
            _board.Hash ^= _values[0];
        }

        public Castling(string[] fenParts, Board board)
        {
            _board = board;
            CastleRights = 0;
            if (fenParts[2] != "-")
            {
                if (fenParts[2].Contains('K'))
                    CastleRights |= (int)Castles.WhiteKingside;
                if (fenParts[2].Contains('Q'))
                    CastleRights |= (int)Castles.WhiteQueenside;
                if (fenParts[2].Contains('k'))
                    CastleRights |= (int)Castles.BlackKingside;
                if (fenParts[2].Contains('q'))
                    CastleRights |= (int)Castles.BlackQueenside;
            }

            _board.Hash ^= _values[CastleRights];
        }
        public string Fen()
        {
            if (CastleRights == 0)
                return "-";
            else
            {
                var fen = "";
                if ((CastleRights & (int)Castles.WhiteKingside) > 0)
                    fen += "K";
                if ((CastleRights & (int)Castles.WhiteQueenside) > 0)
                    fen += "Q";
                if ((CastleRights & (int)Castles.BlackKingside) > 0)
                    fen += "k";
                if ((CastleRights & (int)Castles.BlackQueenside) > 0)
                    fen += "q";
                return fen;
            }
        }

        public Boolean HasCastleRights(Castles c)
        {
            return (CastleRights & (int)c) != 0;
        }

        public Move ApplyMove(Move m, Piece p)
        {
            if (m.Castling())
            {
                var RookIndex = _board.FindPieceIndex(m.CastleStart);
                if (RookIndex != -1)
                {
                    var piece = _board.Pieces[RookIndex];
                    m = _board.Move.MovePiece(piece, m.CastleStart, m.CastleEnd, m);
                }
            }

            int effectedCastles = p.CastleRights;
            if (m.Capture)
            {
                var target = _board.Capture.LastCapture();
                effectedCastles |= target.CastleRights;
            }

            if (BitUtil.Overlap(CastleRights, effectedCastles))
            {
                _board.Hash ^= _values[CastleRights];
                _castleStack.Push(CastleRights);
                CastleRights = BitUtil.Remove(CastleRights, effectedCastles);
                _board.Hash ^= _values[CastleRights];
                m.CastleImpact = true;
            }
            
            return m;
        }

        public void ReverseMove(Move m)
        {
            if (m.Castling())
            {
                var RookIndex = _board.FindPieceIndex(m.CastleEnd);
                _board.Move.MovePiece(_board.Pieces[RookIndex], m.CastleEnd, m.CastleStart, m, true);
            }

            if(m.CastleImpact)
            {
                _board.Hash ^= _values[CastleRights];
                CastleRights = _castleStack.Pop();
                _board.Hash ^= _values[CastleRights];
            }
        }

        public void Save()
        {
            _castleStack.Clear();
        }
    }
}

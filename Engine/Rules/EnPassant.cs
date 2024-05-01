namespace Engine.Rules
{
    public class EnPassant : IRule
    {
        public ulong PassantSquare { get; private set; } = 0;
        public ulong TargetSquare { get; private set; } = 0;
        private Board _board;
        private FastStack<ulong> _passant = new FastStack<ulong>(16);
        private FastStack<ulong> _targets = new FastStack<ulong>(16);

        private static ulong[] _values;

        static EnPassant()
        {
            _values = Rand.RandULongArray(8);
        }

        public EnPassant(Board board)
        {
            _board = board;
        }

        public EnPassant(string[] fenParts, Board board)
        {
            _board = board;

            var passant = fenParts[3];
            if (passant != "-")
            {
                PassantSquare = BitUtil.AlgebraicToBit(passant);
                TargetSquare = _board.Turn ? PassantSquare >> 8 : PassantSquare << 8;
                _board.Hash ^= _values[HashIndex()];
            }
        }

        public string Fen()
        {
            if (PassantSquare == 0)
                return "-";

            return BitUtil.BitToAlgebraic((ulong)PassantSquare);
        }

        public Move ApplyMove(Move m, int pieceIndex)
        {
            if (_board.Pieces[pieceIndex].Type == PieceTypes.PAWN && (m.Start >> 16 == m.End || m.Start << 16 == m.End))
            {
                UpdateHash();
                _passant.Push(PassantSquare);
                _targets.Push(TargetSquare);

                TargetSquare = m.End;
                PassantSquare = _board.Turn ? m.Start << 8 : m.Start >> 8;
                UpdateHash();
                m.PassantImpact = true;
            }
            else if(TargetSquare != 0)
            {
                _passant.Push(PassantSquare);
                _targets.Push(TargetSquare);
                UpdateHash();

                TargetSquare = 0;
                PassantSquare = 0;
                m.PassantImpact = true;
            }

            return m;
        }

        public void ReverseMove(Move m, int _pieceIndex)
        {
            if (m.PassantImpact)
            {
                UpdateHash();
                PassantSquare = _passant.Pop();
                TargetSquare = _targets.Pop();
                UpdateHash();
            }
        }

        public void Save()
        {
            _passant.Clear();
            _targets.Clear();
        }

        // Provide a mask of squares this piece can attack
        // Returns true if that mask overlaps with the passant square
        public bool Attack(ulong mask)
        {
            return BitUtil.Overlap(PassantSquare, mask);
        }

        public int HashIndex()
        {
            return BitUtil.BitToX(PassantSquare);
        }

        public void UpdateHash()
        {
            if(PassantSquare != 0)
            {
                _board.Hash ^= _values[HashIndex()];
            }
        }
    }
}

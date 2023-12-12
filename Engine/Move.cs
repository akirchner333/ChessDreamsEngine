namespace Engine
{
    public interface IMove<T>
    {
        bool EqualTo(T move);
        bool Quiet();
    }
    enum MoveType
    {
        Quiet,
        Capture,
        Castle,
        Passant,
        Upgrade
    }
    public class Move : IMove<Move>
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public bool Side { get; private set; }
        public bool Capture { get; set; } = false;

        //These are things we reference to help with reversing. They're set as the move is applied
        public bool CastleImpact { get; set; } = false;
        public bool PassantImpact { get; set; } = false;
        public bool ClockReset { get; set; } = false;
        

        public Move(ulong start, ulong end, bool side)
        {
            Start = start;
            End = end;
            Side = side;
        }

        public static Move FromAlgebraic(string start, string end, bool side, bool capture = false)
        {
            return new Move(BitUtil.AlgebraicToBit(start), BitUtil.AlgebraicToBit(end), side) { Capture = capture };
        }

        public virtual ulong TargetSquare()
        {
            return End;
        }
        public string EndAlgebraic()
        {
            return BitUtil.BitToAlgebraic(End);
        }

        public string StartAlgebraic()
        {
            return BitUtil.BitToAlgebraic(Start);
        }

        public string StartEnd()
        {
            return StartAlgebraic() + EndAlgebraic();
        }

        public virtual string LongAlgebraic()
        {
            return StartEnd();
        }

        public override string ToString()
        {
            if (Capture)
            {
                return $"Capturing {(Side ? "White" : "Black")} move " + LongAlgebraic();
            }
            else
            {
                return $"{(Side ? "White" : "Black")} Move " + LongAlgebraic();
            }
        }

        // This is not sufficient to determine if two moves from different board positions are the same
        // But if we're comparing the same board position, we should be fine
        public bool EqualTo(Move m)
        {
            return Side == m.Side && Start == m.Start && End == m.End && TargetSquare() == m.TargetSquare();
        }

        public bool Quiet()
        {
            //Not strictly true - we should also be checking if this move puts the opponent into check. 
            return !Capture;
        }
    }

    //Claiming a draw
    public class DrawMove : Move
    {
        public DrawMove(bool side) : base(0, 0, side) { }

        public override string LongAlgebraic()
        {
            return "½–½";
        }
    }

    public class PassantMove : Move
    {
        private ulong _targetSquare;
        public PassantMove(ulong start, ulong end, bool side, ulong targetSquare) : base(start, end, side)
        {
            _targetSquare = targetSquare;
            Capture = true;
        }
        public override ulong TargetSquare()
        {
            return _targetSquare;
        }

        public override string ToString()
        {
            return "En Passant " + LongAlgebraic();
        }
    }

    public class PromotionMove : Move
    {
        public PieceTypes Promotion { get; protected set; }
        public PromotionMove(ulong start, ulong end, bool side, PieceTypes promotion) : base(start, end, side)
        {
            Promotion = promotion;
        }

        public override string LongAlgebraic()
        {
            var promotionChar = Promotion switch
            {
                PieceTypes.ROOK => 'r',
                PieceTypes.KNIGHT => 'n',
                PieceTypes.BISHOP => 'b',
                PieceTypes.QUEEN => 'q',
                _ => 'x'
            };
            return StartEnd() + promotionChar;
        }

        public override string ToString()
        {
            return (Capture ? "Capture Promote Move " : "Promotion Move ") + LongAlgebraic();

        }
    }

    //Castling is traditionally a king move, so start and end refer to the king's starting and ending position
    public class CastleMove : Move
    {
        public ulong RookStart { get; init; }
        public ulong RookEnd { get; init; }
        public CastleMove(ulong start, ulong end, bool side) : base(start, end, side) { }

        public int RookIndex { get; set; }

        public override string ToString()
        {
            return "Castle Move " + LongAlgebraic();
        }
    }
}


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
        Promotion,
        CapturePromotion
    }

    public struct Move : IMove<Move>
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public ulong CastleStart { get; set; } = 0;
        public ulong CastleEnd { get; set; } = 0;
        public ulong PassantTargetSquare { get; set; } = 0;
        public PieceTypes? Promotion { get; set; } = null;
        public bool Side { get; set; }
        public bool Capture { get; set; } = false;

        //These are things we reference to help with reversing. They're set as the move is applied
        public bool CastleImpact { get; set; } = false;
        public bool PassantImpact { get; set; } = false;
        public bool ClockReset { get; set; } = false;
        public bool Draw { get; set; } = false;

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

        public ulong TargetSquare()
        {
            if(PassantTargetSquare != 0)
                return PassantTargetSquare;
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

        public string LongAlgebraic()
        {
            if (Draw)
            {
                return "½–½";
            }
            return StartEnd() + PromotionChar();
        }

        public string PromotionChar()
        {
            return Promotion switch
            {
                PieceTypes.ROOK => "r",
                PieceTypes.KNIGHT => "n",
                PieceTypes.BISHOP => "b",
                PieceTypes.QUEEN => "q",
                _ => ""
            };
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

        public bool Castling()
        {
            return CastleStart != 0 && CastleEnd != 0;
        }

        public bool Promoting()
        {
            return Promotion != null;
        }

        public bool Passant()
        {
            return PassantTargetSquare != 0;
        }
    }
}


namespace Engine
{
    public interface IMove<T>
    {
        bool EqualTo(T move);
        bool Quiet();
    }
    public enum MoveType
    {
        Quiet,
        Capture,
        Castle,
        Passant,
        Promotion,
        CapturePromotion
    }

    // Moves are a struct so can be `stackalloc`ed
    public struct Move : IMove<Move>
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public ulong OtherPosition { get; set; } = 0;
        public MoveType Type { get; private set; } = MoveType.Quiet;
        public PieceTypes Promotion { get; set; } = PieceTypes.PAWN;
        public bool Side { get; set; }

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

        public Move(ulong start, ulong end, bool side, bool capture)
        {
            Start = start;
            End = end;
            Side = side;
            Type = capture ? MoveType.Capture : MoveType.Quiet;
        }

        public static Move FromAlgebraic(string start, string end, bool side, MoveType type)
        {
            return new Move(BitUtil.AlgebraicToBit(start), BitUtil.AlgebraicToBit(end), side) { Type = type };
        }

        public static Move CastleMove(ulong start, ulong end, ulong rookStart, bool side)
        {
            return new Move(start, end, side)
            {
                Type = MoveType.Castle,
                OtherPosition = rookStart
            };
        }

        public static Move PromotionMove(ulong start, ulong end, bool side, PieceTypes promotion, bool capture)
        {
            return new Move(start, end, side)
            {
                Type = capture ? MoveType.CapturePromotion : MoveType.Promotion,
                Promotion = promotion
            };
        }

        public static Move PassantMove(ulong start, ulong end, ulong target, bool side)
        {
            return new Move(start, end, side)
            {
                Type = MoveType.Passant,
                OtherPosition = target
            };
        }

        public ulong TargetSquare()
        {
            if(Type == MoveType.Passant)
                return OtherPosition;
            return End;
        }


        public string LongAlgebraic()
        {
            if (Draw)
            {
                return "½–½";
            }
            return $"{BitUtil.BitToAlgebraic(Start)}{BitUtil.BitToAlgebraic(End)}{PromotionChar()}";
        }
        public string StartString()
        {
            return BitUtil.BitToAlgebraic(Start);
        }

        public string EndString()
        {
            return BitUtil.BitToAlgebraic(End);
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
            if (Capture())
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

        public bool Castling()
        {
            return Type == MoveType.Castle;
        }

        public bool Promoting()
        {
            return Type == MoveType.Promotion || Type == MoveType.CapturePromotion;
        }

        public bool Passant()
        {
            return Type == MoveType.Passant;
        }

        public bool Capture()
        {
            return Type == MoveType.Capture || Type == MoveType.Passant || Type == MoveType.CapturePromotion;
        }

        public bool Quiet()
        {
            return Type == MoveType.Quiet || Type == MoveType.Castle;
        }
    }
}


using System;
using System.Numerics;

namespace Engine
{
    enum MoveType
    {
        Quiet,
        Capture,
        Castle,
        Passant,
        Upgrade
    }
    public class Move
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public ulong TargetSquare { get; set; } = 0;
        public bool Side { get; private set; }
        public bool Capture { get; protected set; } = false;
        public bool Promoting { get; protected set; } = false;

        public Move(ulong start, ulong end, bool side)
        {
            Start = start;
            End = end;
            Side = side;
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
    }

    public class CaptureMove : Move
    {
        public PieceTypes Target {  get; protected set; }
        public CaptureMove(ulong start, ulong end, bool side, PieceTypes target) : base(start, end, side)
        {
            Target = target;
            TargetSquare = end;
            Capture = true;
        }
    }

    public class PassantMove : CaptureMove
    {
        public PassantMove(ulong start, ulong end, bool side, ulong targetSquare) : base(start, end, side, PieceTypes.PAWN)
        {
            TargetSquare = targetSquare;
            Capture = true;
        }
    }

    public class PromotionMove : Move
    {
        public PieceTypes Promotion { get; protected set; }
        public PromotionMove(ulong start, ulong end, bool side, PieceTypes promotion) : base(start, end, side)
        {
            Promotion = promotion;
            Promoting = true;
        }
    }

    public class CapturePromotionMove : Move
    {
        public PieceTypes Target { get; protected set; }
        public PieceTypes Promotion { get; protected set; }
        
        public CapturePromotionMove(ulong start, ulong end, bool side, PieceTypes target, PieceTypes promotion) : base(start, end, side)
        {
            Target = target;
            TargetSquare = end;
            Promotion = promotion;
            Capture = true;
            Promoting = true;
        }
    }

    //Castling is traditionally a king move, so start and end refer to the king's starting and ending position
    public class CastleMove : Move
    {
        public ulong RookStart { get; init; }
        public ulong RookEnd { get; init; }
        public CastleMove(ulong start, ulong end, bool side) : base(start, end, side) { }
    }
}


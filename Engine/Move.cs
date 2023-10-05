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
        public bool Side { get; private set; }
        public bool Capture { get; protected set; } = false;

        public Move(ulong start, ulong end, bool side)
        {
            Start = start;
            End = end;
            Side = side;
        }

        public string EndAlgebraic()
        {
            return BitUtilities.BitToAlgebraic(End);
        }
    }

    public class CaptureMove : Move
    {
        public PieceTypes Target {  get; protected set; }
        public CaptureMove(ulong start, ulong end, bool side, PieceTypes target) : base(start, end, side)
        {
            Target = target;
            Capture = true;
        }
    }

    public class PassantMove : CaptureMove
    {
        public ulong TargetPosition { get; private set; }
        public PassantMove(ulong start, ulong end, bool side, ulong targetPosition) : base(start, end, side, PieceTypes.PAWN)
        {
            TargetPosition = targetPosition;
            Capture = true;
        }
    }

    public class PromotionMove : Move
    {
        public PieceTypes Promotion { get; protected set; }
        public PromotionMove(ulong start, ulong end, bool side, PieceTypes promotion) : base(start, end, side)
        {
            Promotion = promotion;
        }
    }

    public class CapturePromotionMove : Move
    {
        public PieceTypes Target { get; protected set; }
        public PieceTypes Promotion { get; protected set; }
        
        public CapturePromotionMove(ulong start, ulong end, bool side, PieceTypes target, PieceTypes promotion) : base(start, end, side)
        {
            Target = target;
            Promotion = promotion;
            Capture = true;
        }
    }

    //Castling is traditionally a king move, so start and end refer to the king's starting and ending position
    public class CastleMove : Move
    {
        public ulong KnightStart { get; protected set; }
        public ulong KnightEnd { get; protected set; }
        public CastleMove(ulong start, ulong end, ulong knightStart, ulong knightEnd, bool side) : base(start, end, side)
        {
            KnightStart = knightStart;
            KnightEnd = knightEnd;
        }
    }
}


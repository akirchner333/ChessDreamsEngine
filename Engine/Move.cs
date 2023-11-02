using System;
using System.Drawing;
using System.Numerics;

namespace Engine
{
    public interface IMove { }
    enum MoveType
    {
        Quiet,
        Capture,
        Castle,
        Passant,
        Upgrade
    }
    public class Move : IMove
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public bool Side { get; private set; }
        public ulong TargetSquare { get; set; } = 0;
        public bool Capture { get; protected set; } = false;
        public bool Promoting { get; protected set; } = false;

        //These are things we reference to help with reversing. They're set as the move is applied
        //Is this the pieces first move?
        public bool FirstMove { get; set; } = false;
        public int EffectedCastles { get; set; } = 0;
        public Pawn? EnPassantTarget { get; set; }
        public int HalfMoves { get; set; }
        public PieceTypes Target { get; set; }

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

        public virtual string LongAlgebraic()
        {
            return StartEnd();
        }
    }

    public class CaptureMove : Move
    {
        public CaptureMove(ulong start, ulong end, bool side) : base(start, end, side)
        {
            TargetSquare = end;
            Capture = true;
        }
    }

    public class PassantMove : CaptureMove
    {
        public PassantMove(ulong start, ulong end, bool side, ulong targetSquare) : base(start, end, side)
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
    }

    public class CapturePromotionMove : PromotionMove
    {        
        public CapturePromotionMove(ulong start, ulong end, bool side, PieceTypes promotion) : base(start, end, side, promotion)
        {
            TargetSquare = end;
            Capture = true;
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


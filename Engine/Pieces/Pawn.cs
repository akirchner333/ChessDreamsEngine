using System;

namespace Engine
{
    public class Pawn : Piece
    {
        //Keeps track if this pawn has ever move
        //This way if a pawn got teleported back to the first row it couldn't make a 2 move again (I dunno, it could happen)
        public Boolean NeverMoved { get; private set; } = true;
        private ulong _sevenBlocker;
        private ulong _nineBlocker;
        private Func<int, ulong> _shifter;
        //The penultimate row for each side. pen(ultimate)Row
        private ulong _penRow;

        public Pawn(int x, int y, bool side) : base(x, y, side)
        {
            setFixed();
        }

        public Pawn(String algebraic, bool side) : base(algebraic, side)
        {
            setFixed();
        }

        private void setFixed()
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
            _sevenBlocker = ~(Side ? Board.Columns["H"] : Board.Columns["A"]);
            _nineBlocker = ~(Side ? Board.Columns["A"] : Board.Columns["H"]);
            _shifter = ShiftPosition(Side ? 1 : -1);
            _penRow = Side ? Board.Rows[6] : Board.Rows[1];
            //Pawns that don't start on the home row are assumed to have moved
            NeverMoved = BitUtil.Overlap(Position, Side ? Board.Rows[1] : Board.Rows[6]);
        }

        public override ulong MoveMask(Board board)
        {
            if (BitUtil.Overlap(Position, _penRow))
                return 0;

            ulong moves = 0;

            var oneStep = _shifter(8);
            if ((oneStep & board.AllPieces) == 0)
            {
                moves |= oneStep;
            }
            if (NeverMoved && moves > 0)
            {
                var twoSteps = _shifter(16);
                if ((twoSteps & board.AllPieces) == 0)
                    moves |= twoSteps;
            }

            return moves | CaptureMoveMask(board);
        }

        public override ulong AttackMask(Board b)
        {
            return CaptureMoveMask(b);
        }

        //To Do: Add the correct blockers here
        public ulong CaptureMoveMask(Board board)
        {
            ulong attacks = (
                (_shifter(7) & _sevenBlocker) |
                (_shifter(9) & _nineBlocker)
            ) & enemyPieces(board);

            return attacks;
        }

        public List<Move> EnPassant(Board board)
        {
            var moves = new List<Move>();
            if (board.EnPassantTarget == null)
                return moves;

            if ((Position << 1) == board.EnPassantTarget.Position || (Position >> 1) == board.EnPassantTarget.Position)
            {
                ulong end = (Position << 1) == board.EnPassantTarget.Position ? _shifter(7) : _shifter(9);
                moves.Add(new PassantMove(Position, end, Side, board.EnPassantTarget.Position));
            }
            return moves;
        }

        // Promotions! Detecting when they happen and making the moves for them
        public List<Move> Promotions(Board board)
        {
            var list = new List<Move>();
            if ((Position & _penRow) != 0)
            {
                if ((_shifter(8) & board.AllPieces) == 0)
                    list.AddRange(AllPromotions(_shifter(8)));
                if ((_shifter(7) & board.AllPieces) != 0)
                {
                    var targetPiece = board.FindPiece(_shifter(7));
                    if (targetPiece != null)
                        list.AddRange(AllCapturePromotions(_shifter(7), targetPiece.Type));
                }
                if ((_shifter(9) & board.AllPieces) != 0)
                {
                    var targetPiece = board.FindPiece(_shifter(9));
                    if (targetPiece != null)
                        list.AddRange(AllCapturePromotions(_shifter(9), targetPiece.Type));
                }
            }
            return list;
        }

        private List<Move> AllPromotions(ulong end)
        {
            return new List<Move>()
            {
                new PromotionMove(Position, end, Side, PieceTypes.ROOK),
                new PromotionMove(Position, end, Side, PieceTypes.KNIGHT),
                new PromotionMove(Position, end, Side, PieceTypes.BISHOP),
                new PromotionMove(Position, end, Side, PieceTypes.QUEEN)
            };
        }

        private List<Move> AllCapturePromotions(ulong end, PieceTypes target)
        {
            return new List<Move>()
            {
                new CapturePromotionMove(Position, end, Side, target, PieceTypes.ROOK),
                new CapturePromotionMove(Position, end, Side, target, PieceTypes.KNIGHT),
                new CapturePromotionMove(Position, end, Side, target, PieceTypes.BISHOP),
                new CapturePromotionMove(Position, end, Side, target, PieceTypes.QUEEN)
            };
        }

        public override List<Move> Moves(Board b)
        {
            var result = new List<Move>();

            result.AddRange(base.Moves(b));
            result.AddRange(Promotions(b));
            result.AddRange(EnPassant(b));

            return result;
        }

        public override void MoveTo(ulong end)
        {
            base.MoveTo(end);
            _shifter = ShiftPosition(Side ? 1 : -1);
            //Does having a conditional check here every time we do a move faster than just setting _neverMoved to false every time?
            //And does it make enough of a difference to matter?
            if (NeverMoved)
                NeverMoved = false;
        }

        public override void ReverseMove(Move m)
        {
            //How do I know if this was the move that sets it back to 0?
            //Uuuuugh
            base.ReverseMove(m);
        }
    }
}

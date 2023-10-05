using System;

namespace Engine
{
    public class Pawn : Piece
    {
        //Keeps track if this pawn has ever move
        //This way if a pawn got teleported back to the first row it couldn't make a 2 move again (I dunno, it could happen)
        private Boolean _neverMoved = true;
        public Pawn(int x, int y, bool side) : base(x, y, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public Pawn(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public override ulong MoveMask(Board board)
        {
            var shifter = pawnShifter();
            ulong moves = 0;
            var blocker = Side == Sides.White ? Board.Rows[7] : Board.Rows[0];

            var oneStep = shifter(8);
            if ((oneStep & (board.AllPieces | blocker)) == 0)
            {
                moves |= oneStep;
            }
            if (_neverMoved && moves > 0)
            {
                var twoSteps = shifter(16);
                if ((twoSteps & board.AllPieces) == 0)
                    moves |= twoSteps;
            }

            return moves | CaptureMoveMask(board);
        }

        public ulong CaptureMoveMask(Board board)
        {
            var blocker = Side == Sides.White ? Board.Rows[7] : Board.Rows[0];
            var shifter = pawnShifter();

            ulong attacks = (
                (shifter(7) & (long.MaxValue ^ Right)) |
                (shifter(9) & (long.MaxValue ^ Left))
            ) & enemyPieces(board);

            if ((attacks & blocker) == 0)
            {
                return attacks;
            }
            return 0;
        }

        public List<Move> EnPassant(Board board)
        {
            var moves = new List<Move>();
            if (board.EnPassantTarget == null)
                return moves;

            if ((Position << 1) == board.EnPassantTarget.Position || (Position >> 1) == board.EnPassantTarget.Position)
            {
                var shifter = pawnShifter();
                ulong end = (Position << 1) == board.EnPassantTarget.Position ? shifter(7) : shifter(9);
                moves.Add(new PassantMove(Position, end, Side, board.EnPassantTarget.Position));
            }
            return moves;
        }

        // Promotions! Detecting when they happen and making the moves for them
        public List<Move> Promotions(Board board)
        {
            var list = new List<Move>();
            var penRow = Side == Sides.White ? Board.Rows[6] : Board.Rows[1];
            var shifter = pawnShifter();
            if ((Position & penRow) != 0)
            {
                if ((shifter(8) & board.AllPieces) == 0)
                    list.AddRange(AllPromotions(shifter(8)));
                if ((shifter(7) & board.AllPieces) != 0)
                {
                    var targetPiece = board.FindPiece(shifter(7));
                    if (targetPiece != null)
                        list.AddRange(AllCapturePromotions(shifter(7), targetPiece.Type));
                }
                if ((shifter(9) & board.AllPieces) != 0)
                {
                    var targetPiece = board.FindPiece(shifter(9));
                    if (targetPiece != null)
                        list.AddRange(AllCapturePromotions(shifter(9), targetPiece.Type));
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

        public Func<int, ulong> pawnShifter()
        {
            return ShiftDirection(Side ? 1 : -1);
        }

        public override void ApplyMove(Move m)
        {
            base.ApplyMove(m);
            //Does having a conditional check here every time we do a move faster than just setting _neverMoved to false every time?
            //And does it make enough of a difference to matter?
            if (_neverMoved)
                _neverMoved = false;
        }
    }
}

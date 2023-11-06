using Engine.Pieces.Movers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Engine
{
    public class Pawn : Piece
    {
        public override string Name { get; } = "Pawn";
        public override PieceTypes Type { get; } = PieceTypes.PAWN;
        public override char Short { get; } = 'p';

        private Func<int, ulong> _shifter;
        //The penultimate row for each side. pen(ultimate)Row
        private ulong _endRow;

        private PeacefulLeaper _moveLeaper;
        private KillerLeaper _attackLeaper;
        private InitialMover _twoSteps;

        public Pawn(int x, int y, bool side) : base(x, y, side)
        {
            setFixed();
        }

        public Pawn(String algebraic, bool side) : base(algebraic, side)
        {
            setFixed();
        }

        public Pawn(ulong bit, bool side) : base(bit, side)
        {
            setFixed();
        }

        private void setFixed()
        {
            _shifter = ShiftPosition(Side ? 1 : -1);
            _endRow = Side ? Board.Rows[7] : Board.Rows[0];
            _moveLeaper = new PeacefulLeaper(Side ? new int[1] { 8 } : new int[1] { -8 }, Side ? "PawnMoveWhite" : "PawnMoveBlack");
            _attackLeaper = new KillerLeaper(Side ? new int[2] {  7, 9 } : new int[2] { -7, -9 }, Side ? "PawnAttackWhite" : "PawnAttackBlack")
            {
                Side = Side
            };
            _twoSteps = new InitialMover(Side ? Board.Rows[1] : Board.Rows[6], Side ? 8 : -8);
        }

        public override ulong MoveMask(Board board)
        {
            return _moveLeaper.MoveMask(Index, board) | _attackLeaper.MoveMask(Index, board) | _twoSteps.MoveMask(Index, board);
        }

        public override ulong AttackMask(Board b)
        {
            if (PassantAttacker(b))
                return _attackLeaper.MoveMask(Index, b) | b.EnPassantTarget.Position;

            return _attackLeaper.MoveMask(Index, b);
        }

        public List<Move> EnPassant(Board board)
        {
            var moves = new List<Move>();
            if (!PassantAttacker(board))
                return moves;

            ulong end = (Position << 1) == board.EnPassantTarget.Position ? _shifter(7) : _shifter(9);
            moves.Add(new PassantMove(Position, end, Side, board.EnPassantTarget.Position));
            return moves;
        }

        public bool PassantAttacker(Board board)
        {
            return board.EnPassantTarget != null && (
                (Position << 1) == board.EnPassantTarget.Position || (Position >> 1) == board.EnPassantTarget.Position
            );
        }
        public override List<Move> ConvertMask(Board b)
        {
            var moves = new List<Move>();
            var mask = MoveMask(b);
            var bits = BitUtil.SplitBits(mask);
            foreach(var bit in bits)
            {
                if (BitUtil.Overlap(bit, _endRow))
                    moves.AddRange(AllPromotions(bit, BitUtil.Overlap(bit, b.AllPieces)));
                else
                {
                    moves.Add(new Move(Position, bit, Side) {  Capture = BitUtil.Overlap(bit, b.AllPieces) });
                }
            }

            return moves;
        }

        private List<Move> AllPromotions(ulong end, bool capture)
        {
            return new List<Move>()
            {
                new PromotionMove(Position, end, Side, PieceTypes.ROOK) { Capture = capture },
                new PromotionMove(Position, end, Side, PieceTypes.KNIGHT) { Capture = capture },
                new PromotionMove(Position, end, Side, PieceTypes.BISHOP) { Capture = capture },
                new PromotionMove(Position, end, Side, PieceTypes.QUEEN) { Capture = capture }
            };
        }

        public override List<Move> Moves(Board b)
        {
            var result = new List<Move>();

            result.AddRange(base.Moves(b));
            //result.AddRange(Promotions(b));
            result.AddRange(EnPassant(b));

            return result;
        }

        public override Move ApplyMove(Move m)
        {
            m = base.ApplyMove(m);
            _shifter = ShiftPosition(Side ? 1 : -1);

            return m;
        }

        public override Move ReverseMove(Move m)
        {
            m = base.ReverseMove(m);
            _shifter = ShiftPosition(Side ? 1 : -1);
            return m;
        }
    }
}

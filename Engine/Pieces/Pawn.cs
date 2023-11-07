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

        private ulong _endRow;

        private PeacefulLeaper _moveLeaper;
        private KillerLeaper _attackLeaper;
        private InitialMover _twoSteps;

        private Promotions _promotion;

        // Everything gets set in SetFixed, so I'm ignoring the warnings here
#pragma warning disable CS8618
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
#pragma warning restore CS8618

        private void setFixed()
        {
            _endRow = Side ? Board.Rows[7] : Board.Rows[0];
            _moveLeaper = new PeacefulLeaper(Side ? new int[1] { 8 } : new int[1] { -8 }, Side ? "PawnMoveWhite" : "PawnMoveBlack");
            _attackLeaper = new KillerLeaper(Side ? new int[2] {  7, 9 } : new int[2] { -7, -9 }, Side ? "PawnAttackWhite" : "PawnAttackBlack")
            {
                Side = Side
            };
            _twoSteps = new InitialMover(Side ? Board.Rows[1] : Board.Rows[6], Side ? 8 : -8);
            _promotion = new Promotions(Side ? Board.Rows[7] : Board.Rows[0]) { Side = Side };
        }

        public override ulong MoveMask(Board board)
        {
            return _moveLeaper.MoveMask(Index, board) | _attackLeaper.MoveMask(Index, board) | _twoSteps.MoveMask(Index, board);
        }

        public override ulong AttackMask(Board b)
        {
            if (b.Passant.Attack(_attackLeaper.RawMask(Index)))
                return _attackLeaper.MoveMask(Index, b) | b.Passant.TargetSquare;

            return _attackLeaper.MoveMask(Index, b);
        }

        // This calls the Passant rule in the board directly
        // So if I tried to put a Pawn into a game that didn't allow En Passant (i.e. didn't have that rule), it would fail
        // I think the solution to this is Events and messages - something to look into later
        public Move[] EnPassant(Board board)
        {
            // Might be worth replacing this conditional with something like Promotion
            if (board.Passant.Attack(_attackLeaper.RawMask(Index)))
            {
                return new Move[1]
                {
                    new PassantMove(Position, board.Passant.PassantSquare, Side, board.Passant.TargetSquare)
                };
            }

            return new Move[0] {};
        }

        // There's something here about, like....
        // Generating the move mask, breaking the number based on certain sections
        // Then building something about what's left
        public override List<Move> ConvertMask(Board b)
        {
            var moves = new List<Move>();
            var mask = MoveMask(b);
            // Need an elegant way to gather all the squares otherwise claimed and then just put the regular squares back in here
            var bits = BitUtil.SplitBits(mask & ~_promotion.PromoteSquares);
            foreach(var bit in bits)
                moves.Add(new Move(Position, bit, Side) { Capture = BitUtil.Overlap(bit, b.AllPieces) });

            moves.AddRange(_promotion.ConvertMask(b, Position, mask));

            return moves;
        }

        public override List<Move> Moves(Board b)
        {
            var result = new List<Move>();

            result.AddRange(base.Moves(b));
            result.AddRange(EnPassant(b));

            return result;
        }

        public override Move ApplyMove(Move m)
        {
            m = base.ApplyMove(m);

            return m;
        }

        public override Move ReverseMove(Move m)
        {
            m = base.ReverseMove(m);
            return m;
        }
    }
}

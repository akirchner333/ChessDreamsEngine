using Engine.Pieces.MoveFactories;
using Engine.Pieces.Movers;

namespace Engine
{
    public class Pawn : Piece
    {
        public override string Name { get; } = "Pawn";

        public override PieceTypes Type { get; } = PieceTypes.PAWN;
        public override char Short { get; } = 'p';
        public override int Rank { get { return 6; } }

        private PeacefulLeaper _moveLeaper;
        private KillerLeaper _attackLeaper;
        private InitialMover _twoSteps;
        private PassantMover _passant;

        private Promotions _promotion;

        public Pawn(String algebraic, bool side) : this(BitUtil.AlgebraicToBit(algebraic), side) { }

        public Pawn(ulong bit, bool side) : base(bit, side)
        {
            _moveLeaper = new PeacefulLeaper(Side ? new int[1] { 8 } : new int[1] { -8 }, Side ? "PawnMoveWhite" : "PawnMoveBlack");
            _attackLeaper = new KillerLeaper(Side ? new int[2] { 7, 9 } : new int[2] { -7, -9 }, Side ? "PawnAttackWhite" : "PawnAttackBlack")
            {
                Side = Side
            };
            _twoSteps = new InitialMover(Side ? Board.Rows[1] : Board.Rows[6], Side ? 8 : -8);

            _promotion = new Promotions(Side ? Board.Rows[7] : Board.Rows[0]) { Side = Side };
            _convert.RemoveSquares(_promotion.ApplicableSquares);
            _passant = new PassantMover() { Side = Side };
        }

        // So I'd like to include the passant move in the move mask, cause it might matter for checks and such
        // But I'd need to prevent it from being converted into a normal move
        // With promotions it's easy, cause it's always the same squares
        // Two possible solutions:
        // 1. Add and remove the squares from applicable squares every turn
        // 2. Move around when the attack filter goes through, so it gets filtered out
        public override ulong MoveMask(Board board)
        {
            return _moveLeaper.MoveMask(Index, board) | _twoSteps.MoveMask(Index, board) | _attackLeaper.MoveMask(Index, board);
        }

        public override ulong Mask(ulong _occ)
        {
            return _attackLeaper.RawMask(Index);
        }

        public override ulong AttackMask(Board board)
        {
            return _attackLeaper.RawMask(Index) | _passant.MoveMask(Index, board, _attackLeaper);
        }

        // 4 spots for basic moves
        // 12 spots for promotions
        // 1 spot for en passant
        // But they can kind of overlap, right?
        public override Move[] Moves(Board board)
        {
            var moves = new Move[12];
            var moveCount = 0;

            var mask = MoveMask(board);
            var baseMoves = _convert.ConvertMask(board, Position, mask);
            baseMoves.CopyTo(moves, 0);
            moveCount += baseMoves.Length;

            var passant = _passant.ConvertMask(
                board, Position,
                _passant.MoveMask(Index, board, _attackLeaper)
            );
            passant.CopyTo(moves, moveCount);
            moveCount += passant.Length;

            var promotions = _promotion.ConvertMask(board, Position, mask);
            promotions.CopyTo(moves, moveCount);
            moveCount += promotions.Length;

            Array.Resize(ref moves, moveCount);

            return moves;
        }
    }
}

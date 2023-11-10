using Engine.Pieces.Movers;
using System;

namespace Engine
{
    public class Queen : Piece
    {
        public override string Name { get; } = "Queen";
        public override PieceTypes Type { get; } = PieceTypes.QUEEN;
        public override char Short { get; } = 'q';

        private readonly RookMover _rookMover = new();
        private readonly BishopMover _bishopMover = new();
        public Queen(int x, int y, bool side) : base(x, y, side)
        {
            _rookMover.Side = side;
            _bishopMover.Side = side;
        }

        public Queen(String algebraic, bool side) : base(algebraic, side)
        {
            _rookMover.Side = side;
            _bishopMover.Side = side;
        }

        public Queen(ulong bit, bool side) : base(bit, side)
        {
            _rookMover.Side = side;
            _bishopMover.Side = side;
        }

        public override ulong MoveMask(Board board)
        {
            return _rookMover.MoveMask(Index, board) | _bishopMover.MoveMask(Index, board); 
        }
    } 
}
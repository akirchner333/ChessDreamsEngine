﻿using Engine.Pieces.Movers;
using Engine.Rules;

namespace Engine
{
    public class King : Piece
    {
        public override string Name { get; } = "King";
        public override PieceTypes Type { get; } = PieceTypes.KING;
        public override char Short { get; } = 'k';
        public override int Rank { get { return 0; } }

        private Leaper _leaper = new Leaper(new int[8] { 7, -7, 1, -1, 8, -8, 9, -9 }, "King");
        private CastleMover _castler = new CastleMover();

        public King(String algebraic, bool side) : this(BitUtil.AlgebraicToBit(algebraic), side) { }
        public King(ulong bit, bool side) : base(bit, side)
        {
            _leaper.Side = side;
            _castler.Side = side;
            CastleRights = (int)Castles.WhiteQueenside | (int)Castles.WhiteKingside;
            if (!side)
            {
                CastleRights <<= 2;
            }
        }

        public override ulong MoveMask(Board b)
        {
            return BitUtil.Remove(_leaper.MoveMask(Index, b), Side ? b.LegalMoves.BlackAttacks : b.LegalMoves.WhiteAttacks);
        }

        public override ulong Mask(ulong _occ)
        {
            return _leaper.RawMask(Index);
        }

        public override Move[] Moves(Board b, bool captureOnly = false)
        {
            if (captureOnly)
            {
                var mask = MoveMask(b) & enemyPieces(b);
                return _convert.ConvertMask(b, Position, mask);
            }

            var moves = new Move[8];
            var moveCount = 0;
            
            var baseMoves = base.Moves(b);
            baseMoves.CopyTo(moves, 0);
            moveCount += baseMoves.Length;

            var castleMoves = _castler.Moves(b, Position);
            castleMoves.CopyTo(moves, moveCount);
            moveCount += castleMoves.Count();

            Array.Resize(ref moves, moveCount);

            return moves;
        }


    }
}


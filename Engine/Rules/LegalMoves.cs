﻿using Engine.Pieces.Movers;

namespace Engine.Rules
{
    public class LegalMoves
    {
        public ulong WhiteAttacks { get; set; } = 0;
        public ulong BlackAttacks { get; set; } = 0;
        // WhitePins is a board of the white pieces which are pinned. BlackPins is the same for black pieces
        public ulong WhitePins { get; set; } = 0;
        public ulong BlackPins { get; set; } = 0;
        public bool Check { get; set; } = false;
        public Piece[] Attackers { get; set; }

        private Board _board;

        public LegalMoves(Board board)
        {
            _board = board;
            SetAttackMasks();
        }

        public bool LegalMove(Move m)
        {
            var king = _board.GetKing(m.Side);
            if (Check)
            {
                //There are only 3 valid moves in this case:
                // 1. Move the king out of the way. Any moves which'd put the king into check have already
                // been filtered out
                if (m.Start == king!.Position)
                    return true;

                // If two pieces are attacking the king, the only way out is to move the king. And we've already
                // Established that you're not doing that
                if (Attackers.Length > 1)
                    return false;
                var attacker = Attackers[0];

                // 2. Capture the attacking piece
                if (m.Capture && m.TargetSquare() == attacker.Position)
                    return true;

                // 3. Move into the path of the attacker
                var path = attacker.PathBetween(_board, king.Index);
                if (BitUtil.Overlap(path, m.End))
                    return true;

                return false;
            }

            // If the piece is pinned
            if (BitUtil.Overlap(m.Start, m.Side ? WhitePins : BlackPins))
            {
                foreach (var attacker in _board.Pieces)
                {
                    // Find the piece that's attacking
                    if (attacker.Active(!m.Side) && BitUtil.Overlap(attacker.XRayAttacks(_board), king.Position))
                    {
                        var path = attacker.PathBetween(_board, BitUtil.BitToIndex(m.Start));
                        return BitUtil.Overlap(path | attacker.Position, m.End);
                    }
                }
            }

            return true;
        }

        public bool InCheck(bool side)
        {
            var king = _board.GetKing(side);
            if (king == null) return false;
            var check = Attacked(side, king.Position);
            if(check)
                Attackers = Array.FindAll(_board.Pieces, p => p.Active(!side) && BitUtil.Overlap(p.AttackMask(_board), king.Position));

            return check;
        }

        public bool Attacked(bool side, ulong position)
        {
            return BitUtil.Overlap(position, side ? BlackAttacks : WhiteAttacks);
        }

        public void SetAttackMasks()
        {
            WhiteAttacks = 0;
            BlackAttacks = 0;
            WhitePins = 0;
            BlackPins = 0;
            var whiteKing = _board.GetKing(true);
            var blackKing = _board.GetKing(false);
            foreach (var piece in _board.Pieces)
            {
                if (piece.Active(true))
                {
                    WhiteAttacks |= piece.Mask(BitUtil.Remove(_board.AllPieces, blackKing.Position));
                    BlackPins |= piece.PathBetween(_board, blackKing.Index) & _board.AllPieces;
                }
                else if (!piece.Captured)
                {
                    BlackAttacks |= piece.Mask(BitUtil.Remove(_board.AllPieces, whiteKing.Position));
                    WhitePins |= piece.PathBetween(_board, whiteKing.Index) & _board.AllPieces;
                }
            }
            Check = InCheck(_board.Turn);
        }

        public Move ApplyMove(Move m, int _pieceIndex)
        {
            SetAttackMasks();
            return m;
        }

        public void ReverseMove(Move _m, int _pieceIndex)
        {
            SetAttackMasks();
        }
    }
}

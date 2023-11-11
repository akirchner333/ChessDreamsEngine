using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public class Zobrist : IRule
    {
        public ulong Hash { get; private set; } = 0;

        private Board _board;
        private Random _rand { get; private set; } = new Random(1932847123948);

        const var pieceCount = 6;
        const var passantFiles = 8;
        const var castleOptions = 16;

        private ulong[] _values;

        // I'm not sure about these values
        private int _passantIndex;
        private int _castleIndex;
        private int _turnIndex;

        public Zobrist(Board board)
        {
            _board = board;

            // ~~~~~~~~~~~~~ Set up values array ~~~~~~~~~~~~~~
            _passantIndex = _pieceCount * 2 * 64;
            _castleIndex = _passantIndex + passantFiles;
            _turnIndex = _castleIndex + castleOptions;
            _values = new ulong[_turnIndex + 1];
            for(var i = 0; i < _values.Count(); i++)
            {
                _values[i] = RandomKey();
            }

            // ~~~~~~~~~~~~~ Calculate initial hash ~~~~~~~~~~~~~~~~~~~~~
            for(var i = 0; i < _board.Pieces)
            {
                TogglePiece(_board.Pieces[i], _board.Pieces[i].Index);
            }

            if(!_board.Turn)
                ToggleTurn();

            if(_board.Passant.PassantSquare > 0)
                TogglePassant(_board.Passant.PassantSquare);

            //Is this handling the situation correctly when CastleRights = 0?
            ToggleCastles(_board.Castles.CastleRights);
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            TogglePiece(_board.Pieces[pieceIndex], BitUtil.BitToIndex(move.Start));
            TogglePiece(_board.Pieces[pieceIndex], BitUtil.BitToIndex(move.End));

            if(move.Capture)
            {
                TogglePiece(
                    _board.Pieces[move.TargetIndex],
                    BitUtil.BitToIndex(move.TargetSquare())
                );
            }

            // Could I somehow leverage that these 0 when empty to avoid using
            // conditionals here?
            // What happens if I put 0 into BitToIndex?
            if(m.PassantSquare > 0)
            {
                TogglePassant(m.PassantSquare));
            }

            if(_board.Passant.PassantSquare > 0)
            {
                TogglePassant(_board.Passant.PassantSquare));
            }

            ToggleCastles(m.Castles);
            ToggleCastles(_board.Castles.CastleRights);
            ToggleTurn();

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            ApplyMove(move, pieceIndex);
        }

        // ~~~~~~~~~~~~~~~~~~~~~ Toggles! ~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void TogglePiece(Piece p, int index)
        {
            Hash ^= _values[GetPieceIndex(p, index)];
        }

        public ulong GetPieceIndex(Piece p, int index)
        {
            var startIndex = type switch
            {
                PieceTypes.PAWN => 0,
                PieceTypes.ROOK => 64,
                PieceTypes.KNIGHT => 128,
                PieceTypes.BISHOP => 192,
                PieceTypes.QUEEN =>256,
                PieceTypes.KING => 320,
                _ => 0
            };

            startIndex += p.Side ? 0 : 384;

            return startIndex + index;
        }

        public void TogglePassant(ulong passantSquare)
        {
            Hash ^= values[_passantIndex + BitUtil.BitToX(passantSquare)];
        }

        public void ToggleCastles(int rights)
        {
            Hash ^= values[_castleIndex + rights];
        }

        public void ToggleTurn()
        {
            Hash ^= values[_turnIndex];
        }

        public ulong RandomKey()
        {
            return (ulong)((rand.NextInt64() << 1) ^ rand.Next());
        }
    }
}

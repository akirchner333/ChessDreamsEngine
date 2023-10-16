using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Engine
{
    public abstract class Piece
    {
        public ulong Position { get; set; } = 0;
        public int Index { get; set; } = 0;
        public PieceTypes Type { get; protected set; }
        public bool Side;
        public bool Captured { get; set; } = false;

        protected string _name = "whatever";

        public Piece(int x, int y, bool side)
        {
            Side = side;
            Index = y * 8 + x;
            Position = 1ul << Index;
        }

        public Piece(string square, bool side)
        {
            Side = side;
            Index = BitUtil.AlgebraicToIndex(square);
            Position = 1ul << Index;
        }

        public string PieceSprite()
        {
            return (Side ? "White" : "Black") + _name;
        }

        public abstract ulong MoveMask(Board b);

        public virtual ulong AttackMask(Board b)
        {
            return MoveMask(b);
        }

        // Moves in a specified direction until it hits a border or obstacle. Can move any number of squares. Bishops, Rooks, and Queens
        // Move this into a Rider class, maybe, later
        public ulong RiderMoves(int steps, ulong blocker, Board board)
        {
            ulong moves = 0;
            var shifter = ShiftPosition(steps);

            for (var i = 1; i < 8; i++)
            {
                var square = shifter(i);
                if (square == 0 || (square & blocker) > 0)
                    break;
                moves |= square;
                if ((board.AllPieces & square) > 0)
                    break;
            }

            //Removes all the possible moves that are on top of a piece of the same side
            return BitUtil.Remove(moves, allyPieces(board));
        }

        protected ulong allyPieces(Board b)
        {
            return Side ? b.WhitePieces : b.BlackPieces;
        }

        protected ulong enemyPieces(Board b)
        {
            return Side ? b.BlackPieces : b.WhitePieces;
        }

        //This is a bit of an over-complicated way to get around C#'s inability to shift by negative values
        protected Func<int, ulong> ShiftPosition(int steps)
        {
            return Shifter(steps, Position);
        }

        protected static Func<int, ulong> Shifter(int steps, ulong start)
        {
            return steps > 0 ?
                (x) => start << (x * steps) :
                (x) => start >> (x * Math.Abs(steps));
        }

        //Takes a ulong and breaks it up into a bunch of Move objects, one for each bit
        public List<Move> ConvertMask(Board b)
        {
            var moves = new List<Move>();
            var mask = MoveMask(b);
            var index = 0;
            while ((mask >> index) > 0 && index < 64)
            {
                index += BitOperations.TrailingZeroCount(mask >> index);
                var targetSquare = 1ul << index;

                // I don't like this bit - it's bad encapsulation to have so much Board in Piece
                // Maybe board should handle identifying captures?
                if ((b.AllPieces & targetSquare) == 0)
                    moves.Add(new Move(Position, targetSquare, Side));
                else
                {
                    var targetPiece = b.FindPiece(targetSquare);
                    if (targetPiece != null)
                        moves.Add(new CaptureMove(Position, targetSquare, Side, targetPiece.Type));
                }
                index += 1;
            }

            return moves;
        }

        // In many cases just breaking up the mask (am I using that term correctly?) will be enough
        // But for stuff like en passant or castling, we'll need to extend this method to add in some extra stuff
        public virtual List<Move> Moves(Board b)
        {
            return ConvertMask(b);
        }

        public virtual void MoveTo(ulong end)
        {
            Position = end;
        }

        public virtual void ReverseMove(Move m)
        {
            Position = m.Start;
        }
    }
}
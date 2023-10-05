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
        public PieceTypes Type { get; protected set; }
        protected string _name = "whatever";
        public bool Side;

        protected const ulong Bottom = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_11111111;
        protected const ulong Top =    0b11111111_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        protected const ulong Right =  0b10000000_10000000_10000000_10000000_10000000_10000000_10000000_10000000;
        protected const ulong Left =   0b00000001_00000001_00000001_00000001_00000001_00000001_00000001_00000001;

        public Piece(int x, int y, bool side)
        {
            Side = side;
            Position = (ulong)1 << (y * 8 + x);
        }

        public Piece(string square, bool side)
        {
            Side = side;
            Position = BitUtilities.AlgebraicToBit(square);
        }

        public string PieceSprite()
        {
            return (Side ? "White" : "Black") + _name;
        }

        public abstract ulong MoveMask(Board b);

        // Moves in a specified direction until it hits a border or obstacle. Can move any number of squares. Bishops, Rooks, and Queens
        public ulong RiderMoves(int steps, ulong blocker, Board board)
        {
            ulong moves = 0;
            var shifter = ShiftDirection(steps);

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
            return (moves & allyPieces(board)) ^ moves;
        }

        // Moves directly to a fixed distance square, once. Knights and (technically) kings
        public ulong LeaperMoves(int steps, ulong blocker, Board board)
        {
            var shifter = ShiftDirection(steps);
            ulong move = shifter(1);
            
            if ((move & allyPieces(board)) == 0 && (move & blocker) == 0)
                return move;
            return 0;
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
        protected Func<int, ulong> ShiftDirection(int steps)
        {
            return steps > 0 ?
                x => Position << x * steps :
                x => Position >> x * Math.Abs(steps);
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
                var targetSquare = (ulong)1 << index;

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

        public virtual void ApplyMove(Move m)
        {
            Position = m.End;
        }
    }
}
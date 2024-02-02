using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Engine
{
    public class PGN
    {
        public Dictionary<string, string> Attributes { get; private set; } = new Dictionary<string, string>();
        public List<string> Moves { get; private set; } = new List<string>();
        public int Turn { get; private set; } = 0;
        public Board Board { get; private set; } = Board.StartPosition();

        public PGN(string pgn)
        {
            AddAttributes(pgn);
            GetMoves(pgn);
        }

        public void AddAttributes(string pgn)
        {
            // This regex matches the attribute lines at the top, in the format of
            // [Event "London m"]
            Regex infoReg = new Regex(@"\[(.*)\s\""(.*)\""\]");
            var matches = infoReg.Matches(pgn);
            foreach(Match match in matches)
            {
                Attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        public void GetMoves(string pgn)
        {
            // This regex matches the turn lines in the pgn file, in the format of
            // 12.O-O Bxc3
            // The two capture groups - /([\w\d\-\+]+)/ - should catch the individual moves. Pretty sure I got all the characters that might be there
            // The second one has {0,1} in case the game ends on white's turn
            var turnReg = new Regex(@"\d+\.([\w\d\-\+\=]+)\s([\w\d\-\+\=]+){0,1}");
            var matches = turnReg.Matches(pgn);
            foreach (Match match in matches)
            {
                Moves.Add(match.Groups[1].Value);
                Moves.Add(match.Groups[2].Value);
            }
        }

        public void StepGame()
        {
            var move = Moves[Turn];
            Span<Move> moves = stackalloc Move[Board.MAXMOVES];
            Board.Moves(ref moves);
            if(move == "O-O" || move == "O-O-O")
            {
                // Find the castle move and apply it
                return;
            }

            foreach(var m in moves)
            {
                if (!MatchingMove(move, m))
                    continue;
                
                // Need to handle promotions

                Board.ApplyMoveReal(m);
                break;
            }
            // Sort through the moves and find all the moves with this ending position
            // If there's more than one, keep looking
            // There are three additional pieces of information that might be there: piece type, column, or row
            // If it has an = in it, it's a promotion move
        }

        public bool MatchingMove(string algebraic, Move move)
        {
            var moveParts = new Regex(@"(?<piece>[NRBQK])?(?<start_row>[a-h])?(?<start_col>[1-8])?x?(?<end>[a-h][1-8])\+?");
            GroupCollection parts = moveParts.Match(algebraic).Groups;

            if (move.EndString() != parts["end"].Value)
                return false;

            if (parts["start_row"].Success && !move.StartString().StartsWith(parts["start_row"].Value))
                return false;

            if (parts["start_col"].Success && !move.StartString().EndsWith(parts["start_col"].Value))
                return false;

            // Piece types are mentioned by default, with
            var type = parts["piece"].Success ? LetterToType(parts["piece"].Value) : PieceTypes.PAWN;
            var piece = Board.FindPiece(move.Start);
            if (piece != null)
            {
                if (piece.Type != type)
                    return false;
            }

            return true;
        }

        public PieceTypes LetterToType(string letter)
        {
            switch (letter)
            {
                case "K":
                    return PieceTypes.KING;
                case "Q":
                    return PieceTypes.QUEEN;
                case "R":
                    return PieceTypes.ROOK;
                case "B":
                    return PieceTypes.BISHOP;
                case "N":
                    return PieceTypes.KNIGHT;
                default:
                    return PieceTypes.PAWN;
            }
        }
    }
}

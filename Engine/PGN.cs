using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Engine
{
    // http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm
    public class PGN
    {
        public Dictionary<string, string> Attributes { get; private set; } = new Dictionary<string, string>();
        public List<string> Moves { get; private set; } = new List<string>();
        public int Turn { get; private set; } = 0;
        public Board Board { get; set; } = Board.StartPosition();

        public static bool WholePGN(string pgn)
        {
            var trimmed = pgn.Trim();
            return trimmed.StartsWith("[") && (trimmed.EndsWith("1-0") || trimmed.EndsWith("0-1") || trimmed.EndsWith("1/2-1/2") || trimmed.EndsWith("*"));
        }

        public PGN(string pgn)
        {
            var cleaned = Regex.Replace(pgn, @"\{.*?\}", "");
            cleaned = Regex.Replace(cleaned, @"\d+\.{3}", "");
            cleaned = Regex.Replace(cleaned, @"[\?\!]", "");
            AddAttributes(cleaned);
            GetMoves(cleaned);
        }

        public void AddAttributes(string pgn)
        {
            // This regex matches the attribute lines at the top, in the format of
            // [Event "London m"]
            Regex infoReg = new Regex(@"\[(.*)\s\""(.*)\""\]");
            var matches = infoReg.Matches(pgn);
            foreach(Match match in matches)
            {
                Attributes[match.Groups[1].Value] = match.Groups[2].Value;
            }
        }

        public void GetMoves(string pgn)
        {
            // This regex matches the turn lines in the pgn file, in the format of
            // 12.O-O Bxc3
            // The two capture groups - /([\w\d\-\+\=]+)/ - should catch the individual moves. Pretty sure I got all the characters that might be there
            // The second one has {0,1} in case the game ends on white's turn
            var turnReg = new Regex(@"\d+\.\s*([\w\d\-\+\=]+)\s+([\w\d\-\+\=]+){0,1}");
            var matches = turnReg.Matches(pgn);
            foreach (Match match in matches)
            {
                Moves.Add(match.Groups[1].Value);
                if (match.Groups[2].Success)
                {
                    var move = match.Groups[2].Value;
                    if(move != "1-0" && move != "0-1" && move != "1")
                        Moves.Add(match.Groups[2].Value);
                }
            }
        }

        public Move StepGame()
        {
            var algebraic = Moves[Turn];
            Span<Move> moves = stackalloc Move[Board.MAXMOVES];
            Board.Moves(ref moves);

            var move = FindMove(algebraic, moves);
            Board.ApplyMoveReal(move);
            Turn++;

            return move;
        }

        public bool GameRunning()
        {
            return Turn < Moves.Count();
        }

        public Move FindMove(string algebraic, Span<Move> moves)
        {
            foreach (var m in moves)
            {
                if (algebraic.StartsWith("O-O"))
                {
                    if (!m.Castling())
                        continue;

                    // Queenside
                    if (algebraic.StartsWith("O-O-O"))
                    {
                        if (BitUtil.BitToX(m.End) == 2)
                            return m;

                        continue;
                    }
                        
                    // Kingside
                    if (BitUtil.BitToX(m.End) == 6)
                        return m;

                    continue;
                }

                if (algebraic.Contains("=") && !PromotionMatch(algebraic, m))
                    continue;

                if (!PositionMatch(algebraic, m) || !TypeMatch(algebraic, m))
                    continue;

                return m;
            }

            throw new NotImplementedException($"Requested move {algebraic} not found among the moves for {Board.Fen()}, Turn {Turn}");
        }

        public bool PositionMatch(string algebraic, Move move)
        {
            var moveParts = new Regex(@"(?<start_row>[a-h])?(?<start_col>[1-8])?x?(?<end>[a-h][1-8])");
            GroupCollection parts = moveParts.Match(algebraic).Groups;

            if (move.EndString() != parts["end"].Value)
                return false;

            if (parts["start_row"].Success && !move.StartString().StartsWith(parts["start_row"].Value))
                return false;
                
            if (parts["start_col"].Success && !move.StartString().EndsWith(parts["start_col"].Value))
                return false;
                

            return true;
        }

        public bool TypeMatch(string algebraic, Move move)
        {
            var piece = Board.FindPiece(move.Start);
            if (piece == null)
                throw new ArgumentNullException($"Could not find piece at {BitUtil.BitToAlgebraic(move.Start)}");

            return piece.Type == LetterToType(algebraic[0]);
        }

        public bool PromotionMatch(string algebraic, Move move)
        {
            // The last letter might be +, if it puts the king into check
            // So how do I find the last non-plus sign character?
            // For real, I hate it here
            var promote = new Regex(@"([QRBN])\+?$");
            var last = promote.Match(algebraic).Groups[1].Value[0];
            return move.Promoting() && move.Promotion == LetterToType(last);
        }

        public PieceTypes LetterToType(char letter)
        {
            switch (letter)
            {
                case 'K':
                    return PieceTypes.KING;
                case 'Q':
                    return PieceTypes.QUEEN;
                case 'R':
                    return PieceTypes.ROOK;
                case 'B':
                    return PieceTypes.BISHOP;
                case 'N':
                    return PieceTypes.KNIGHT;
                default:
                    return PieceTypes.PAWN;
            }
        }
    }
}

using Engine.Rules;
using System;
using System.Collections;
using System.Drawing;

namespace Engine
{
    public interface IGameEngine<M> where M : class
    {
        M[] Moves();
        M ApplyMove(M move);
        void ReverseMove(M move);
        bool TerminalNode();
    }
    public enum GameState
    {
        PLAY,
        WHITE_WINS,
        BLACK_WINS,
        DRAW
    }
    public static class Sides
    {
        public const bool White = true;
        public const bool Black = false;
    }

    public enum PieceTypes
    {
        PAWN,
        ROOK,
        KNIGHT,
        BISHOP,
        QUEEN,
        KING
    }

    public class Board : IGameEngine<Move>
    {
        //TODO
        // - Build from FEN
        // - Print FEN

        public static ulong[] Rows = new ulong[8]
        {
            0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_11111111,
            0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_00000000,
            0b00000000_00000000_00000000_00000000_00000000_11111111_00000000_00000000,
            0b00000000_00000000_00000000_00000000_11111111_00000000_00000000_00000000,
            0b00000000_00000000_00000000_11111111_00000000_00000000_00000000_00000000,
            0b00000000_00000000_11111111_00000000_00000000_00000000_00000000_00000000,
            0b00000000_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
            0b11111111_00000000_00000000_00000000_00000000_00000000_00000000_00000000
        };

        public static Dictionary<string, ulong> Columns = new Dictionary<string, ulong>()
        {
            { "H", 0b10000000_10000000_10000000_10000000_10000000_10000000_10000000_10000000 },
            { "G", 0b01000000_01000000_01000000_01000000_01000000_01000000_01000000_01000000 },
            { "F", 0b00100000_00100000_00100000_00100000_00100000_00100000_00100000_00100000 },
            { "E", 0b00010000_00010000_00010000_00010000_00010000_00010000_00010000_00010000 },
            { "D", 0b00001000_00001000_00001000_00001000_00001000_00001000_00001000_00001000 },
            { "C", 0b00000100_00000100_00000100_00000100_00000100_00000100_00000100_00000100 },
            { "B", 0b00000010_00000010_00000010_00000010_00000010_00000010_00000010_00000010 },
            { "A", 0b00000001_00000001_00000001_00000001_00000001_00000001_00000001_00000001 }
        };

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ GAME STATE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Piece[] Pieces = new Piece[0];
        public bool Turn = Sides.White;
        public int TurnNumber { get; private set; } = 0;
        public static ulong TurnValue;
        public GameState State { get; set; } = GameState.PLAY;
        public bool drawAvailable = false;
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~ RULES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PieceMovement Move { get; private set; }
        public Capture Capture { get; private set; }
        public EnPassant Passant { get; private set; }
        public FiftyMove Clock { get; private set; }
        public Castling Castles { get; private set; }
        public Promotion Promote { get; private set; }
        public Repetition Repetition { get; private set; }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ RECORDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ulong AllPieces { get; set; } = 0;
        public ulong WhitePieces { get; set; } = 0;
        public ulong BlackPieces { get; set; } = 0;
        public Move[] MoveList { get; set; }
        public ulong Hash { get; set; } = 0;

        // ------------------------------------------------------------- BOARD CREATION ------------------------------------------------
        static Board()
        {
            TurnValue = Rand.RandULong();
        }
        
        public Board()
        {
            Move = new PieceMovement(this);
            Capture = new Capture(this);
            Passant = new EnPassant(this);
            Clock = new FiftyMove(this);
            Castles = new Castling(this);
            Promote = new Promotion(this);
            Repetition = new Repetition(this);

            SetGameState();
        }

        public Board(string fen)
        {
            var fields = fen.Split(' ');

            //Piece placement
            var y = 7; //why do FENs start with 8 and go down?
            foreach (var row in fields[0].Split('/'))
            {
                var x = 0;
                foreach (var c in row)
                {
                    switch (Char.ToLower(c))
                    {
                        case 'p':
                            AddPiece(x, y, PieceTypes.PAWN, Char.IsUpper(c));
                            break;
                        case 'r':
                            AddPiece(x, y, PieceTypes.ROOK, Char.IsUpper(c));
                            break;
                        case 'n':
                            AddPiece(x, y, PieceTypes.KNIGHT, Char.IsUpper(c));
                            break;
                        case 'b':
                            AddPiece(x, y, PieceTypes.BISHOP, Char.IsUpper(c));
                            break;
                        case 'q':
                            AddPiece(x, y, PieceTypes.QUEEN, Char.IsUpper(c));
                            break;
                        case 'k':
                            AddPiece(x, y, PieceTypes.KING, Char.IsUpper(c));
                            break;
                        default:
                            x += (int)Char.GetNumericValue(c) - 1;
                            break;
                    }
                    x++;
                }
                y--;
            }

            //Active color
            Turn = (fields[1] == "w");
            if (!Turn)
                Hash ^= TurnValue;
            TurnNumber = Int32.Parse(fields[5]);

            Move = new PieceMovement(this);
            Castles = new Castling(fields, this);
            Passant = new EnPassant(fields, this);
            Clock = new FiftyMove(fields, this);
            Promote = new Promotion(this);
            Capture = new Capture(this);
            Repetition = new Repetition(this);

            SetGameState();
        }

        public static Board StartPosition()
        {
            return new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void AddPiece(int x, int y, PieceTypes t, bool side)
        {
            AddPiece(BitUtil.CoordToBit(x, y), t, side);
        }

        public void AddPiece(string algebraic, PieceTypes t, bool side)
        {
            AddPiece(BitUtil.AlgebraicToBit(algebraic), t, side);
        }

        public void AddPiece(ulong bit, PieceTypes t, bool side)
        {
            var newPiece = NewPiece(bit, t, side);

            AllPieces |= newPiece.Position;

            if (side)
                WhitePieces |= newPiece.Position;
            else
                BlackPieces |= newPiece.Position;

            //This is probably not the fastest, but we'll only be adding pieces at the start of the game
            //So it's probably ok
            Array.Resize(ref Pieces, Pieces.Length + 1);
            Pieces[Pieces.Length - 1] = newPiece;

            Move?.TogglePiece(newPiece);
        }

        public Piece NewPiece(ulong bit, PieceTypes t, bool side)
        {
            return t switch
            {
                PieceTypes.PAWN => new Pawn(bit, side),
                PieceTypes.ROOK => new Rook(bit, side),
                PieceTypes.KNIGHT => new Knight(bit, side),
                PieceTypes.BISHOP => new Bishop(bit, side),
                PieceTypes.QUEEN => new Queen(bit, side),
                PieceTypes.KING => new King(bit, side),
                _ => new Rook(bit, side)
            };
        }

        // --------------------------------------------------------------- BOARD INFORMATION -------------------------------------------------
        public Piece? FindPiece(ulong position)
        {
            return Array.Find(Pieces, p => p.Position == position & !p.Captured);
        }

        public int FindPieceIndex(ulong position)
        {
            return Array.FindIndex(Pieces, p => p.Position == position & !p.Captured);
        }

        //Is the given square under attack by the other side?
        public bool Attacked(bool side, ulong position)
        {
            var captures = Pieces
                .Where(p => p.Side != side && !p.Captured)
                .Aggregate(0ul, (acc, p) => acc | p.AttackMask(this));
            return BitUtil.Overlap(position, captures);
        }

        public bool TerminalNode()
        {
            return State != GameState.PLAY;
        }

        public ulong SidePieces(bool side)
        {
            return side ? WhitePieces : BlackPieces;
        }

        public string Fen()
        {
            var fen = "";

            for (var i = 7; i >= 0; i--)
            {
                var count = 0;
                for (var j = 0; j < 8; j++)
                {
                    var piece = FindPiece(BitUtil.CoordToBit(j, i));
                    if (piece == null)
                    {
                        count++;
                    }
                    else
                    {
                        if (count > 0)
                            fen += count;
                        count = 0;
                        var pieceChar = piece.Type switch
                        {
                            PieceTypes.PAWN => 'p',
                            PieceTypes.ROOK => 'r',
                            PieceTypes.KNIGHT => 'n',
                            PieceTypes.BISHOP => 'b',
                            PieceTypes.QUEEN => 'q',
                            PieceTypes.KING => 'k',
                            _ => '!'
                        };
                        fen += piece.Side ? Char.ToUpper(pieceChar) : pieceChar;
                    }
                }
                if (count > 0)
                    fen += count;
                if (i > 0)
                    fen += @"/";
            }

            fen += Turn ? " w" : " b";

            fen += $" {Castles.Fen()} {Passant.Fen()} {Clock.Fen()} {TurnNumber}";
            return fen;
        }

        // ----------------------------------------------------------------------- MOVE GENERATION --------------------------------------------
        public Move[] Moves()
        {
            return MoveList;
        }

        public void GenerateMoves()
        {
            if (State != GameState.PLAY)
                MoveList = new Move[0];
            else
                MoveList = PseudolegalMoves()
                    .Where(m => LegalMove(m))
                    .ToArray();
        }

        public Move[] PseudolegalMoves()
        {
            var movingPieces = Pieces.Where(p => p.Side == Turn && !p.Captured).ToArray();
            var moves = new Move[218];
            var moveCount = 0;
            for (var i = 0; i < movingPieces.Count(); i++)
            {
                var pieceMoves = movingPieces[i].Moves(this);
                pieceMoves.CopyTo(moves, moveCount);
                moveCount += pieceMoves.Count();
            }

            Array.Resize(ref moves, moveCount);
            return moves;
        }

        //Just updates the AllPieces, WhitePieces, and BlackPieces boards, checks if it puts you in check
        //And then puts them back
        // TO DO: One shudders to look upon this. Make it better
        public bool LegalMove(Move m)
        {
            var holdWhite = WhitePieces;
            var holdBlack = BlackPieces;

            if (m.Side)
            {
                WhitePieces = BitUtil.Remove(WhitePieces, m.Start);
                WhitePieces |= m.End;
                BlackPieces = BitUtil.Remove(BlackPieces, m.TargetSquare());
                if (m is CastleMove)
                {
                    WhitePieces = BitUtil.Remove(WhitePieces, ((CastleMove)m).RookStart);
                    WhitePieces |= ((CastleMove)m).RookEnd;
                }
            }
            else
            {
                BlackPieces = BitUtil.Remove(BlackPieces, m.Start);
                BlackPieces |= m.End;
                WhitePieces = BitUtil.Remove(WhitePieces, m.TargetSquare());
                if (m is CastleMove)
                {
                    BlackPieces = BitUtil.Remove(BlackPieces, ((CastleMove)m).RookStart);
                    BlackPieces |= ((CastleMove)m).RookEnd;
                }
            }
            AllPieces = WhitePieces | BlackPieces;

            Piece? capturedPiece = FindPiece(m.TargetSquare());
            if (capturedPiece != null)
            {
                capturedPiece.Captured = true;
            }

            var king = Array.Find(Pieces, p => p.Type == PieceTypes.KING && p.Side == m.Side);
            //Having an issue with this line but I'm confused as how to I could lose the King
            var check = king != null ? Attacked(m.Side, m.Start == king.Position ? m.End : king.Position) : true;

            WhitePieces = holdWhite;
            BlackPieces = holdBlack;
            AllPieces = holdWhite | holdBlack;

            if (capturedPiece != null)
            {
                capturedPiece.Captured = false;
            }

            return !check;
        }

        // -------------------------------------------------------------------- MOVE APPLICATION -----------------------------------------------------------

        public Move ApplyMove(Move m)
        {
            var pieceIndex = FindPieceIndex(m.Start);
            if (pieceIndex == -1)
                return m;
            Piece p = Pieces[pieceIndex];

            m = Capture.ApplyMove(m, pieceIndex);
            m = Move.ApplyMove(m, pieceIndex);
            m = Passant.ApplyMove(m, p);
            m = Castles.ApplyMove(m, p);
            m = Clock.ApplyMove(m, pieceIndex);
            m = Promote.ApplyMove(m, pieceIndex);

            //Store the current board state somewhere for 3/5 repetition

            //Next move
            if (Turn == Sides.Black)
                TurnNumber++;
            Turn = !Turn;
            Hash ^= TurnValue;

            Repetition.ApplyMove(m, pieceIndex);

            SetGameState();

            return m;
        }

        public void RemoveSquare(ulong position, bool side)
        {
            if (side)
                WhitePieces = BitUtil.Remove(WhitePieces, position);
            else
                BlackPieces = BitUtil.Remove(BlackPieces, position);
            AllPieces = BlackPieces | WhitePieces;
        }

        public void ReverseMove(Move m)
        {

            int pieceIndex = FindPieceIndex(m.End);
            if (pieceIndex == -1)
                return;
            var p = Pieces[pieceIndex];

            Capture.ReverseMove(m, pieceIndex);
            Move.ReverseMove(m, pieceIndex);
            Passant.ReverseMove(m);
            Clock.ReverseMove(m, pieceIndex);
            Castles.ReverseMove(m);
            Promote.ReverseMove(m, pieceIndex);

            //To Do: Pop the board state back one

            //Next move
            if (Turn == Sides.White)
                TurnNumber--;
            Turn = !Turn;
            Hash ^= TurnValue;
            Repetition.ApplyMove(m, pieceIndex);

            State = GameState.PLAY;
            GenerateMoves();
        }

        public void SetGameState()
        {
            GenerateMoves();
            if (MoveList.Count() == 0)
            {
                var king = Array.Find(Pieces, p => p.Type == PieceTypes.KING && p.Side == Turn);
                if (king != null)
                {
                    if (Attacked(Turn, king.Position))
                        State = Turn ? GameState.BLACK_WINS : GameState.WHITE_WINS;
                    else
                        State = GameState.DRAW;
                }
            }
        }
    }
}
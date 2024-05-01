using Engine.Pieces.Movers;
using Engine.Rules;
using System.ComponentModel;
using System.Drawing;

namespace Engine
{
    public interface IGameEngine<M> where M : IMove<M>
    {
        bool Moves(ref Span<M> moves, bool captureOnly = false);
        M ApplyMove(M move);
        void ReverseMove(M move);
        bool TerminalNode();
        ulong Hash { get; }
        bool Turn { get; }
        int MAXMOVES { get; }
    }
    public enum GameState
    {
        PLAY = 0,
        WHITE_WINS = 1,
        BLACK_WINS = 2,
        DRAW = 3
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
        // The maximum number of legal moves a board can have
        public int MAXMOVES => 218;

        public readonly static ulong[] Rows = new ulong[8]
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

        public readonly static Dictionary<string, ulong> Columns = new()
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
        public Piece[] Pieces = Array.Empty<Piece>();
        public bool Turn { get; private set; } = Sides.White;
        public int TurnNumber { get; private set; } = 0;
        public static ulong TurnValue { get; private set; }
        public GameState State { get; set; } = GameState.PLAY;
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~ RULES ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public PieceMovement Move { get; private set; }
        public Capture Capture { get; private set; }
        public EnPassant Passant { get; private set; }
        public FiftyMove Clock { get; private set; }
        public Castling Castles { get; private set; }
        public Promotion Promote { get; private set; }
        public Repetition Repetition { get; private set; }
        public LegalMoves LegalMoves { get; private set; }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ RECORDS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ulong AllPieces { get; set; } = 0;
        public ulong WhitePieces { get; set; } = 0;
        public ulong BlackPieces { get; set; } = 0;
        // The zobrist hash of the current board state is stored here
        // Individual rules access and update it as they handle relevant events
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
            LegalMoves = new LegalMoves(this);
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

            // Builds all the rules
            Move = new PieceMovement(this);
            Castles = new Castling(fields, this);
            Passant = new EnPassant(fields, this);
            Clock = new FiftyMove(fields, this);
            Promote = new Promotion(this);
            Capture = new Capture(this);
            Repetition = new Repetition(this);
            LegalMoves = new LegalMoves(this);
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
            Pieces[^1] = newPiece;
            Array.Sort(Pieces, (a, b) =>
            {
                if (a.Rank != b.Rank) return a.Rank - b.Rank;
                return (a.Side ? 0 : 1) - (b.Side ? 0 : 1);
            });

            Move?.TogglePiece(newPiece);
        }

        public static Piece NewPiece(ulong bit, PieceTypes t, bool side)
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

        // ---------------------------------------------------------- BOARD INFORMATION -----------------------------------------------
        public Piece? FindPiece(ulong position)
        {
            return Array.Find(Pieces, p => !p.Captured & p.Position == position);
        }

        // Like Array.FindIndex, it'll return -1 if there's no piece at that position
        public int FindPieceIndex(ulong position)
        {
            return Array.FindIndex(Pieces, p => !p.Captured & p.Position == position);
        }

        public King? GetKing(bool side)
        {
            if (Pieces.Length > 0)
            {
                return (King)Pieces[side ? 0 : 1];
            }
            return null;
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

        public bool DrawAvailable()
        {
            return Clock.DrawAvailable || Repetition.DrawAvailable;
        }

        // ----------------------------------------------------------------------- MOVE GENERATION --------------------------------------------

        //For when you can't be bothered to alloc your own
        public Move[] MoveArray(bool captureOnly = false)
        {
            Span<Move> moves = new Move[218];
            Moves(ref moves, captureOnly);
            return moves.ToArray();
        }

        // We use a Span of moves here for speed. The space for the moves is
        // allocated beforehand and then truncated
        public bool Moves(ref Span<Move> moves, bool captureOnly = false)
        {
            return SideMoves(ref moves, Turn, captureOnly);
        }

        // Returns false if the game is over and you can't actually have moves
        public bool SideMoves(ref Span<Move> moves, bool side, bool captureOnly = false)
        {
            if (State != GameState.PLAY)
            {
                return false;
            }
            else
            {
                var moveCount = 0;

                foreach (var piece in Pieces)
                {
                    if (!piece.Active(side))
                        continue;

                    foreach (var move in piece.Moves(this, captureOnly))
                    {
                        if (LegalMoves.LegalMove(move))
                        {
                            moves[moveCount] = move;
                            moveCount++;
                        }
                    }
                }

                // We don't know if the game is over until we actually generate the moves
                // Which might cause some problems downhill, I'm not sure
                if(moveCount == 0 && !captureOnly)
                {
                    EndGame();
                    return false;
                }

                if (DrawAvailable() && !captureOnly)
                {
                    moves[moveCount] = new Move(0, 0, side) { Draw = true };
                    moveCount++;
                }

                moves = moves.Slice(0, moveCount);
                return true;
            }
        }

        // Returns the number of legal moves for each side
        // Relevant for players to rate mobility
        public (int, int) MoveCount()
        {
            if (State != GameState.PLAY)
            {
                return (0, 0);
            }

            var white = 0;
            var black = 0;

            foreach (var piece in Pieces)
            {
                if (piece.Captured)
                    continue;

                foreach (var move in piece.Moves(this, false))
                {
                    if (LegalMoves.LegalMove(move))
                    {
                        if (piece.Side)
                        {
                            white++;
                        }
                        else
                        {
                            black++;
                        }
                    }
                }
            }

            if ((Turn && white == 0) || (!Turn && black == 0))
            {
                EndGame();
                return (0, 0);
            }

            if (DrawAvailable())
            {
                if (Turn)
                {
                    white++;
                }
                else
                {
                    black++;
                }
            }

            return (white, black);
        }

        // ----------------------------------------------------------------- MOVE APPLICATION ---------------------------------------------------------

        public Move ApplyMove(Move m)
        {
            if (m.Draw)
            {
                State = GameState.DRAW;
                return m;
            }

            var pieceIndex = FindPieceIndex(m.Start);
            if (pieceIndex == -1)
                return m;
            Piece p = Pieces[pieceIndex];

            m = Capture.ApplyMove(m, pieceIndex);
            m = Move.ApplyMove(m, pieceIndex);
            m = Passant.ApplyMove(m, pieceIndex);
            m = Castles.ApplyMove(m, pieceIndex);
            m = Clock.ApplyMove(m, pieceIndex);
            m = Promote.ApplyMove(m, pieceIndex);

            if (Turn == Sides.Black)
                TurnNumber++;
            Turn = !Turn;
            Hash ^= TurnValue;

            m = LegalMoves.ApplyMove(m, pieceIndex);
            Repetition.ApplyMove(m, pieceIndex);

            return m;
        }

        public void ReverseMove(Move m)
        {
            if (m.Draw)
            {
                State = GameState.PLAY;
                return;
            }

            int pieceIndex = FindPieceIndex(m.End);
            if (pieceIndex == -1)
                return;

            Capture.ReverseMove(m, pieceIndex);
            Move.ReverseMove(m, pieceIndex);
            Passant.ReverseMove(m, pieceIndex);
            Clock.ReverseMove(m, pieceIndex);
            Castles.ReverseMove(m, pieceIndex);
            Promote.ReverseMove(m, pieceIndex);


            if (Turn == Sides.White)
                TurnNumber--;
            Turn = !Turn;
            Hash ^= TurnValue;

            Repetition.ReverseMove(m, pieceIndex);
            LegalMoves.ReverseMove(m, pieceIndex);

            State = GameState.PLAY;
        }

        // Commits the current state of the board, so it can no longer be reversed
        public void ApplyMoveReal(Move m)
        {
            ApplyMove(m);
            Capture.Save();
            Castles.Save();
            Passant.Save();
            Clock.Save();
            Repetition.Save();
        }

        public void RemoveSquare(ulong position, bool side)
        {
            WhitePieces = BitUtil.Remove(WhitePieces, position);
            BlackPieces = BitUtil.Remove(BlackPieces, position);
            AllPieces = BlackPieces | WhitePieces;
        }

        public void EndGame()
        {
            if (LegalMoves.Check)
                State = Turn ? GameState.BLACK_WINS : GameState.WHITE_WINS;
            else
                State = GameState.DRAW;
        }
    }
}
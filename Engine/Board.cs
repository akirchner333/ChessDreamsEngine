using System;
using System.Collections;
using System.Drawing;

namespace Engine
{
	public interface IGameEngine<M> where M : class
	{
		List<M> Moves();
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

	public enum Castles
	{
		WhiteKingside =  0b0001,
		WhiteQueenside = 0b0010,
		BlackKingside =  0b0100,
		BlackQueenside = 0b1000
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

        public List<Piece> Pieces = new List<Piece>();

		public bool Turn = Sides.White;
		public int TurnNumber { get; private set; } = 0;
		public GameState State { get; set; } = GameState.PLAY;
		public bool drawAvailable = false;
		public int HalfmoveClock { get; set; } = 0;
		public int CastleRights { get; set; } = 0b1111;

		public Pawn? EnPassantTarget { get; private set; } = null;

		public ulong AllPieces { get; private set; } = 0;
		public ulong WhitePieces { get; private set; } = 0;
		public ulong BlackPieces { get; private set; } = 0;
		public List<Move> MoveList { get; private set; } = new List<Move>();

		// ------------------------------------------------------------- BOARD CREATION ------------------------------------------------
		public Board(){ }

		public static Board StartPosition()
		{
			return new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
		}

		public Board(string fen)
		{
			var fields = fen.Split(' ');
			//Piece placement
			var y = 7; //why do FENs start with 8 and go down?
			foreach(var row in fields[0].Split('/'))
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

			//Castling availability
			SetCastleRights(fields[2]);

			//En passant target square
			SetEnPassant(fields[3]);

			//Halfmove clock
			HalfmoveClock = Int32.Parse(fields[4]);

			//Fullmoves
			TurnNumber = Int32.Parse(fields[5]);

			SetGameState();
        }

		public void SetCastleRights(string castling)
		{
            CastleRights = 0;
            if (castling != "-")
            {
                if (castling.Contains('K'))
                    CastleRights |= (int)Castles.WhiteKingside;
                if (castling.Contains('Q'))
                    CastleRights |= (int)Castles.WhiteQueenside;
                if (castling.Contains('k'))
                    CastleRights |= (int)Castles.BlackKingside;
                if (castling.Contains('q'))
                    CastleRights |= (int)Castles.BlackQueenside;
            }
        }

		public void SetEnPassant(string passant)
		{
            if (passant != "-")
            {
                var enPassantSquare = BitUtil.AlgebraicToBit(passant);
                var targetSquare = Turn ? enPassantSquare >> 8 : enPassantSquare << 8;
                EnPassantTarget = FindPiece(targetSquare) as Pawn;
            }
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
            Piece newPiece = t switch
            {
                PieceTypes.PAWN => new Pawn(bit, side),
                PieceTypes.ROOK => new Rook(bit, side),
                PieceTypes.KNIGHT => new Knight(bit, side),
                PieceTypes.BISHOP => new Bishop(bit, side),
                PieceTypes.QUEEN => new Queen(bit, side),
                PieceTypes.KING => new King(bit, side),
                _ => new Rook(bit, side)
            };

            AllPieces |= newPiece.Position;

            if (side)
                WhitePieces |= newPiece.Position;
            else
                BlackPieces |= newPiece.Position;

            Pieces.Add(newPiece);
        }

        // --------------------------------------------------------------- BOARD INFORMATION -------------------------------------------------
        public Piece? FindPiece(ulong position)
        {
            return Pieces.Find(p => p.Position == position);
        }

        public Boolean HasCastleRights(Castles c)
        {
            return (CastleRights & (int)c) != 0;
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

			for(var i = 7; i >= 0; i--)
			{
				var count = 0;
				for(var j = 0; j < 8; j++)
				{
					var piece = FindPiece(BitUtil.CoordToBit(j, i));
					if(piece == null)
					{
						count++;
					}
					else
					{
						if(count > 0)
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
				if(i > 0)
					fen += @"/";
            }

			fen += Turn ? " w" : " b";

			if (CastleRights == 0)
				fen += " -";
			else
			{
				fen += " ";
				if ((CastleRights & (int)Castles.WhiteKingside) > 0)
					fen += "K";
                if ((CastleRights & (int)Castles.WhiteQueenside) > 0)
                    fen += "Q";
                if ((CastleRights & (int)Castles.BlackKingside) > 0)
                    fen += "k";
                if ((CastleRights & (int)Castles.BlackQueenside) > 0)
                    fen += "q";
            }

			if (EnPassantTarget == null)
				fen += " -";
			else
				fen += " " + BitUtil.BitToAlgebraic(Turn ? EnPassantTarget.Position << 8 : EnPassantTarget.Position >> 8);

			fen += $" {HalfmoveClock}";
			fen += $" {TurnNumber}";
			return fen;
		}

        // ----------------------------------------------------------------------- MOVE GENERATION --------------------------------------------
        public List<Move> Moves()
		{
			return MoveList;
		}
		
		public void GenerateMoves()
        {
            if (State != GameState.PLAY)
                MoveList = new List<Move>();
			else
				MoveList = PseudolegalMoves()
					.Where(m => LegalMove(m))
					.ToList();
        }

        public IEnumerable<Move> PseudolegalMoves()
		{
			return Pieces
				.Where(p => p.Side == Turn)
				.Select(p => p.Moves(this))
				.SelectMany(m => m);
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
				BlackPieces = BitUtil.Remove(BlackPieces, m.TargetSquare);
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
				WhitePieces = BitUtil.Remove(WhitePieces, m.TargetSquare);
				if (m is CastleMove)
				{
					BlackPieces = BitUtil.Remove(BlackPieces, ((CastleMove)m).RookStart);
					BlackPieces |= ((CastleMove)m).RookEnd;
				}
			}
			AllPieces = WhitePieces | BlackPieces;

			Piece? capturedPiece = FindPiece(m.TargetSquare);
			if (capturedPiece != null)
			{
				capturedPiece.Captured = true;
			}

            var king = Pieces.Find(p => p.Type == PieceTypes.KING && p.Side == m.Side);
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
			m.HalfMoves = HalfmoveClock;
            Piece? p = FindPiece(m.Start);
			if (p == null)
				return m;

            if (m.Capture)
            {
				var targetSquare = m is PassantMove ? ((PassantMove)m).TargetSquare : m.End;
                var target = FindPiece(targetSquare);

				if (target != null)
					m = CapturePiece(target, m);
            }

            m = MovePiece(p, m.Start, m.End, m);

			// Sets (or unsets) an en-passant target
			m.EnPassantTarget = EnPassantTarget;
            if (p.Type == PieceTypes.PAWN && (m.Start >> 16 == m.End || m.Start << 16 == m.End))
                EnPassantTarget = (Pawn)p;
            else
                EnPassantTarget = null;

            //Move the rook when castling
            if (m is CastleMove)
			{
				var rook = FindPiece(((CastleMove)m).RookStart);
                if ( rook != null)
                    MovePiece(rook, ((CastleMove)m).RookStart, ((CastleMove)m).RookEnd, m);
			}

			// Updates the available castles
			int effectedCastles = 0;
			if(p.Type == PieceTypes.ROOK)
			{
				if(BitUtil.Overlap(m.Start, Columns["A"]))
					effectedCastles |= p.Side ? (int)Castles.WhiteQueenside : (int)Castles.BlackQueenside;
				else if(BitUtil.Overlap(m.Start, Columns["H"]))
                    effectedCastles |= p.Side ? (int)Castles.WhiteKingside : (int)Castles.BlackKingside;
			}
			else if(p.Type == PieceTypes.KING)
			{
				if(p.Side)
				{
                    effectedCastles |= (int)Castles.WhiteKingside;
                    effectedCastles |= (int)Castles.WhiteQueenside;
                }
                else
				{
                    effectedCastles |= (int)Castles.BlackKingside;
                    effectedCastles |= (int)Castles.BlackQueenside;
                }
            }
            m.EffectedCastles = CastleRights & effectedCastles;
            CastleRights = BitUtil.Remove(CastleRights, effectedCastles);

			//Promotions
			if (m.Promoting)
			{
				Pieces.Remove(p);
				AddPiece(m.EndAlgebraic(), ((PromotionMove)m).Promotion, m.Side);
			}

            //Store the current board state somewhere for 3/5 repetition

            HalfmoveClock++;
			if (HalfmoveClock == 150)
				State = GameState.DRAW;
			else if (HalfmoveClock > 100)
				drawAvailable = true;
            //Next move
            if (Turn == Sides.Black)
				TurnNumber++;
            Turn = !Turn;

			SetGameState();

            return m;
        }

		public Move MovePiece(Piece p, ulong start, ulong end, Move m, bool reverse = false)
		{
            m = reverse ? p.ReverseMove(m) : p.ApplyMove(m);
            if (p.Side == Sides.Black)
                BlackPieces = BitUtil.Remove(BlackPieces, start) | end;
            else
                WhitePieces = BitUtil.Remove(WhitePieces, start) | end;
			AllPieces = BlackPieces | WhitePieces;

			if(p is Pawn)
			{
				HalfmoveClock = -1;
				drawAvailable = false;
			}

			return m;
        }

		public Move CapturePiece(Piece p, Move m)
		{
			m.Target = p.Type;
			if (p.Side)
                WhitePieces = BitUtil.Remove(WhitePieces, p.Position);
            else
				BlackPieces = BitUtil.Remove(BlackPieces, p.Position);
            AllPieces = BlackPieces | WhitePieces;

            Pieces.Remove(p);
            HalfmoveClock = -1;
			drawAvailable = false;

			return m;
        }

		public void ReverseMove(Move m)
		{
            Piece? p = FindPiece(m.End);
            if (p == null)
                return;

            if (m.Capture)
            {
                var targetSquare = m is PassantMove ? ((PassantMove)m).TargetSquare : m.End;
				AddPiece(targetSquare, m.Target, !m.Side);
            }

            MovePiece(p, m.End, m.Start, m, true);

            // Resets the en-passant target
            EnPassantTarget = m.EnPassantTarget;

            //Move the rook when castling
            if (m is CastleMove)
            {
                var rook = FindPiece(((CastleMove)m).RookEnd);
                if (rook != null)
                    MovePiece(rook, ((CastleMove)m).RookEnd, ((CastleMove)m).RookStart, m, true);
            }
            CastleRights |= m.EffectedCastles;

            //Promotions
            if (m.Promoting)
            {
                Pieces.Remove(p);
                AddPiece(m.Start, PieceTypes.PAWN, m.Side);
            }

            //To Do: Pop the board state back one

            HalfmoveClock = m.HalfMoves;
            if (HalfmoveClock <= 100)
                drawAvailable = false;
            //Next move
            if (Turn == Sides.White)
                TurnNumber--;
            Turn = !Turn;

            State = GameState.PLAY;
			GenerateMoves();
        }

		public void SetGameState()
		{
			GenerateMoves();
            if (MoveList.Count == 0)
            {
                var king = Pieces.Find(p => p.Type == PieceTypes.KING && p.Side == Turn);
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
using System;
using System.Collections;
using System.Drawing;

namespace Engine
{

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
    public class Board
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

		private bool _turn = Sides.White;
		private int _turnNumber = 0;
		public GameState State { get; set; } = GameState.PLAY;
		public bool drawAvailable = false;
		public int HalfmoveClock { get; set; } = 0;
		public int CastleRights { get; set; } = 0b1111;

		public Pawn? EnPassantTarget { get; private set; } = null;

		public ulong AllPieces { get; private set; } = 0;
		public ulong WhitePieces { get; private set; } = 0;
		public ulong BlackPieces { get; private set; } = 0;
		public Board(){ }

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
			_turn = (fields[1] == "w");

			//Castling availability
			SetCastleRights(fields[2]);
			

			//En passant target square
			if (fields[3] != "-")
			{
				var enPassantSquare = BitUtil.AlgebraicToBit(fields[3]);
				var targetSquare = _turn ? enPassantSquare >> 8 : enPassantSquare << 8;
                EnPassantTarget = FindPiece(targetSquare) as Pawn;
            }
                

			//Halfmove clock
			HalfmoveClock = Int32.Parse(fields[4]);

			//Fullmoves
			_turnNumber = Int32.Parse(fields[5]);
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

		public Boolean HasCastleRights(Castles c)
		{
			return (CastleRights & (int)c) != 0;
		}

		public Piece AddPiece(int x, int y, PieceTypes t, bool side)
		{
			Piece newPiece = t switch
			{
				PieceTypes.PAWN => new Pawn(x, y, side),
				PieceTypes.ROOK => new Rook(x, y, side),
				PieceTypes.KNIGHT => new Knight(x, y, side),
                PieceTypes.BISHOP => new Bishop(x, y, side),
				PieceTypes.QUEEN => new Queen(x, y, side),
				PieceTypes.KING => new King(x, y, side),
                _ => new Rook(x, y, side)
			};

			AllPieces |= newPiece.Position;

			if(side == Sides.Black)
				BlackPieces |= newPiece.Position;
			else
				WhitePieces |= newPiece.Position;

			Pieces.Add(newPiece);

			return newPiece;
		}

        public void AddPiece(string algebraic, PieceTypes t, bool side)
        {
            Piece newPiece = t switch
            {
                PieceTypes.PAWN => new Pawn(algebraic, side),
                PieceTypes.ROOK => new Rook(algebraic, side),
                PieceTypes.KNIGHT => new Knight(algebraic, side),
                PieceTypes.BISHOP => new Bishop(algebraic, side),
                PieceTypes.QUEEN => new Queen(algebraic, side),
                PieceTypes.KING => new King(algebraic, side),
                _ => new Rook(algebraic, side)
            };

            AllPieces |= newPiece.Position;

            if (side)
                WhitePieces |= newPiece.Position;
            else
                BlackPieces |= newPiece.Position;

            Pieces.Add(newPiece);
        }

		public IEnumerable<Move> PseudolegalMoves()
		{
			return Pieces
				.Where(p => p.Side == _turn)
				.Select(p => p.Moves(this))
				.SelectMany(m => m);
        }

        public List<Move> Moves()
		{
			if(State != GameState.PLAY)
				return new List<Move>();

			return PseudolegalMoves()
				.Where(m => LegalMove(m))
				.ToList();
		}

		//Just updates the AllPieces, WhitePieces, and BlackPieces boards, checks if it puts you in check
		//And then puts them back
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
			//We need a way to filter out the attacks of captured pieces
			//Without removing them the list (cause we're only pretending they've been captured)
            var check = Attacked(m.Side, m.Start == king!.Position ? m.End : king.Position);

			WhitePieces = holdWhite;
			BlackPieces = holdBlack;
			AllPieces = holdWhite | holdBlack;

            if (capturedPiece != null)
            {
                capturedPiece.Captured = false;
            }

            return !check;
        }

		//Is the given square under attack by the other side?
		public bool Attacked(bool side, ulong position)
		{
			var captures = Pieces
				.Where(p => p.Side != side && !p.Captured)
				.Aggregate(0ul, (acc, p) => acc | p.AttackMask(this));
			return BitUtil.Overlap(position, captures);
		}

		public void ApplyMove(Move m)
		{
            Piece? p = FindPiece(m.Start);
			if (p == null)
				return;

            if (m.Capture)
            {
				var targetSquare = m is PassantMove ? ((PassantMove)m).TargetSquare : m.End;
                var target = FindPiece(targetSquare);

				if (target != null)
					CapturePiece(target);
            }

            MovePiece(p, m.Start, m.End);

			// Sets (or unsets) an en-passant target
			if (p.Type == PieceTypes.PAWN && ((Pawn)p).NeverMoved)
                EnPassantTarget = (Pawn)p;
			else
				EnPassantTarget = null;

			//Move the rook when castling
			if(m is CastleMove)
			{
				var rook = FindPiece(((CastleMove)m).RookStart);
                if ( rook != null)
                    MovePiece(rook, ((CastleMove)m).RookStart, ((CastleMove)m).RookEnd);
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
            CastleRights = (CastleRights & effectedCastles) ^ CastleRights;

			//Promotions
			if (m.Promoting)
			{
				Pieces.Remove(p);
				AddPiece(m.EndAlgebraic(), ((PromotionMove)m).Promotion, m.Side);
			}

            //Store the current board state somewhere for 3/5 repetition

			//To Do: Is this supposed to be /moves/ or /half moves/? I'm not confident
            HalfmoveClock++;
			if (HalfmoveClock == 75)
				State = GameState.DRAW;
			else if (HalfmoveClock > 50)
				drawAvailable = true;
            //Next move
            if (_turn == Sides.Black)
				_turnNumber++;
            _turn = !_turn;

			if(Moves().Count == 0)
				State = _turn ? GameState.BLACK_WINS : GameState.WHITE_WINS;
        }

		public void MovePiece(Piece p, ulong start, ulong end)
		{
            p.MoveTo(end);
            if (p.Side == Sides.Black)
                BlackPieces = (BlackPieces ^ start) | end;
            else
                WhitePieces = (WhitePieces ^ start) | end;
			AllPieces = BlackPieces | WhitePieces;

			if(p is Pawn)
			{
				HalfmoveClock = -1;
				drawAvailable = false;
			}
        }

		public void CapturePiece(Piece p)
		{
			if (p.Side)
				BlackPieces = BitUtil.Remove(BlackPieces, p.Position);
            else
                WhitePieces = BitUtil.Remove(WhitePieces, p.Position);
			AllPieces = BlackPieces | WhitePieces;

            Pieces.Remove(p);
            HalfmoveClock = -1;
			drawAvailable = false;
        }

		public void ReverseMove(Move m)
		{
			//Dear god, how can I restore castling rights?
		}

		public Piece? FindPiece(ulong position)
		{
			return Pieces.Find(p => p.Position == position);
        }

	}
}
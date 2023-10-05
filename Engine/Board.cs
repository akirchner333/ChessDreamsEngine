using System;
using System.Collections;

namespace Engine
{
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

        private List<Piece> _pieces = new List<Piece>();
		private bool _turn = Sides.White;
		private int _turnNumber = 0;
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
			var x = 0;
			var y = 0;
			foreach(var row in fields[0].Split('/'))
			{
				foreach(var c in row)
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
                y++;
            }

			//Active color
			_turn = (fields[1] == "w");

			//Castling availability
			
			CastleRights = 0;
            if (fields[2] != "-")
            {
				if (fields[2].Contains("K"))
					CastleRights |= (int)Castles.WhiteKingside;
                if (fields[2].Contains("Q"))
                    CastleRights |= (int)Castles.WhiteQueenside;
                if (fields[2].Contains("k"))
                    CastleRights |= (int)Castles.BlackKingside;
                if (fields[2].Contains("q"))
                    CastleRights |= (int)Castles.BlackQueenside;
            }

			//En passant target square
			EnPassantTarget = FindPiece(BitUtilities.AlgebraicToBit(fields[3])) as Pawn;

			//Halfmove clock
			HalfmoveClock = Int32.Parse(fields[4]);

			//Fullmoves
			_turnNumber = Int32.Parse(fields[5]);
		}

		public void AddPiece(int x, int y, PieceTypes t, bool side)
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

			_pieces.Add(newPiece);
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

            _pieces.Add(newPiece);
        }

        public List<Move> Moves()
		{
			return _pieces
				.Where(p => p.Side == _turn)
				.Select(p => p.Moves(this))
				.SelectMany(m => m)
				.ToList();
			//TODO: Put something in here that'll filter out all the moves that put you in check
		}

		public void ApplyMove(Move m)
		{
            // Find the piece that's moving, update it's Position
            Piece? p = FindPiece(m.Start);
			if (p == null)
				return;

            p.ApplyMove(m);

            if (m.Capture)
            {
				var targetSquare = m is PassantMove ? ((PassantMove)m).TargetPosition : m.End;
                var target = FindPiece(targetSquare);

                if (target != null)
                    _pieces.Remove(target);

                if (m.Side)
                    BlackPieces = (BlackPieces & targetSquare) ^ BlackPieces;
                else
                    WhitePieces = (WhitePieces & targetSquare) ^ WhitePieces;
                HalfmoveClock = -1;
            }

            // Update the shared boards
            AllPieces = (AllPieces ^ m.Start) | m.End;
			if (m.Side == Sides.Black)
			{
				BlackPieces = (BlackPieces ^ m.Start) | m.End;
				_turnNumber++;
			}
			else
				WhitePieces = (WhitePieces ^ m.Start) | m.End;

			if (m is PassantMove)
				AllPieces ^= ((PassantMove)m).TargetPosition;

			// Sets (or unsets) an en-passant target
			if (p.Type == PieceTypes.PAWN)
			{
				if((m.End == m.Start >> 16 || m.End == m.Start << 16))
                    EnPassantTarget = (Pawn)p;
                HalfmoveClock = -1;
			}
			else
				EnPassantTarget = null;

			// Updates the available castles
			int effectedCastles = 0;
			if(p.Type == PieceTypes.ROOK)
			{
				if((m.Start & Columns["A"]) > 0)
				{
					effectedCastles |= p.Side ? (int)Castles.WhiteQueenside : (int)Castles.BlackQueenside;
				}
				else if((m.Start & Columns["H"]) > 0)
				{
                    effectedCastles |= p.Side ? (int)Castles.WhiteKingside : (int)Castles.BlackKingside;
                }
			}
			else if(p.Type == PieceTypes.KING)
			{
				if(p.Side)
				{
					effectedCastles |= (int)Castles.WhiteQueenside;
                    effectedCastles |= (int)Castles.WhiteKingside;
				}
				else
				{
                    effectedCastles |= (int)Castles.BlackQueenside;
                    effectedCastles |= (int)Castles.BlackKingside;
                }
			}
            CastleRights = (CastleRights & effectedCastles) ^ CastleRights;

            //Store the current board state somewhere for 3/5 repetition

			//Next move
            _turn = !_turn;
            HalfmoveClock++;
        }

		public Piece? FindPiece(ulong position)
		{
			return _pieces.Find(p => p.Position == position);
        }

	}
}
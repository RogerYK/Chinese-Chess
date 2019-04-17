using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
   public class Piece
   {
        public static Piece B_Bishop = new Piece(Chess.Properties.Resources.B_Bishop, ChessFlag.BLACK, PieceType.BISHOP);
        public static Piece B_Cannon = new Piece(Chess.Properties.Resources.B_Canon, ChessFlag.BLACK, PieceType.CANNON);
        public static Piece B_Car = new Piece(Chess.Properties.Resources.B_Car, ChessFlag.BLACK, PieceType.CAR);
        public static Piece B_Elephant = new Piece(Chess.Properties.Resources.B_Elephant, ChessFlag.BLACK, PieceType.ELEPHANT);
        public static Piece B_Horse = new Piece(Chess.Properties.Resources.B_Horse, ChessFlag.BLACK, PieceType.HORSE);
        public static Piece B_King = new Piece(Chess.Properties.Resources.B_King, ChessFlag.BLACK, PieceType.KING);
        public static Piece B_Pawn = new Piece(Chess.Properties.Resources.B_Pawn, ChessFlag.BLACK, PieceType.PAWN);
        public static Piece R_Bishop = new Piece(Chess.Properties.Resources.R_Bishop, ChessFlag.RED, PieceType.BISHOP);
        public static Piece R_Cannon = new Piece(Chess.Properties.Resources.R_Canon, ChessFlag.RED, PieceType.CANNON);
        public static Piece R_Car = new Piece(Chess.Properties.Resources.R_Car, ChessFlag.RED, PieceType.CAR);
        public static Piece R_Elephant = new Piece(Chess.Properties.Resources.R_Elephant, ChessFlag.RED, PieceType.ELEPHANT);
        public static Piece R_Horse = new Piece(Chess.Properties.Resources.R_Horse, ChessFlag.RED, PieceType.HORSE);
        public static Piece R_King = new Piece(Chess.Properties.Resources.R_King, ChessFlag.RED, PieceType.KING);
        public static Piece R_Pawn = new Piece(Chess.Properties.Resources.R_Pawn, ChessFlag.RED, PieceType.PAWN);

        public Bitmap Img {private  set; get; }

        public ChessFlag Flag { private set; get; }

        public PieceType Type { private set; get; }

        private Piece(Bitmap img, ChessFlag flag, PieceType type)
        {
            Img = img;
            Flag = flag;
            Type = type;
        }
   }


    public enum ChessFlag
    {
        RED,
        BLACK,
    }

    public enum PieceType
    {
        PAWN,
        CANNON,
        CAR,
        HORSE,
        ELEPHANT,
        BISHOP,
        KING,
    }

}

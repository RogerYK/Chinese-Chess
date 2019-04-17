
using System;
using System.Drawing;

namespace Chess.view
{
    static class Constant
    {
        public static readonly int BOARD_WIDTH = 700;
        public static readonly int BOARD_HEIGHT = 720;
        public static readonly int BOARD_ROW = 10;
        public static readonly int BOARD_COL = 9;
        public static readonly int CELL_SIZE = 67;
        public static readonly System.Windows.Point START_POS = new System.Windows.Point(55, 10);
        public static readonly int PIECE_SIZE = 67;

        public static Piece[,] INIT_PIECES => new Piece[10, 9]
        {
            {Piece.B_Car, Piece.B_Horse, Piece.B_Elephant, Piece.B_Bishop, Piece.B_King, Piece.B_Bishop, Piece.B_Elephant, Piece.B_Horse, Piece.B_Car},
            {null, null, null, null, null, null, null, null, null},
            {null, Piece.B_Cannon, null, null, null, null, null, Piece.B_Cannon, null},
            {Piece.B_Pawn, null, Piece.B_Pawn, null, Piece.B_Pawn, null, Piece.B_Pawn, null, Piece.B_Pawn},
            {null, null, null, null, null, null, null, null, null},
            {null, null, null, null, null, null, null, null, null},
            {Piece.R_Pawn, null, Piece.R_Pawn, null, Piece.R_Pawn, null, Piece.R_Pawn, null, Piece.R_Pawn},
            {null, Piece.R_Cannon, null, null, null, null, null, Piece.R_Cannon, null},
            {null, null, null, null, null, null, null, null, null},
            {Piece.R_Car, Piece.R_Horse, Piece.R_Elephant, Piece.R_Bishop, Piece.R_King, Piece.R_Bishop, Piece.R_Elephant, Piece.R_Horse, Piece.R_Car}
        };

    }
}
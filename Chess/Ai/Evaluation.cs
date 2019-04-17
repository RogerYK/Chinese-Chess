using Chess.chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.chess.MoveGenerator;

namespace Chess.Ai
{
    static class Evaluation
    {

        private static Dictionary<PieceType, int> baseValues = new Dictionary<PieceType, int>
        {
            {PieceType.KING,  10000},
            {PieceType.CAR, 600 },
            {PieceType.HORSE, 300 },
            {PieceType.CANNON, 300 },
            {PieceType.ELEPHANT, 110 },
            {PieceType.BISHOP, 110 },
            {PieceType.PAWN, 70 },
        };
        private static Dictionary<PieceType, int> flexValues = new Dictionary<PieceType, int>
        {
            {PieceType.KING,  0},
            {PieceType.CAR, 7 },
            {PieceType.HORSE, 13 },
            {PieceType.CANNON, 7 },
            {PieceType.ELEPHANT, 1 },
            {PieceType.BISHOP, 1 },
            {PieceType.PAWN, 15 },
        };

        

        private static int[,] pawnPosValues = new int[10, 9]
        {
            {0, 3, 6, 9, 12, 9, 6, 3, 0 },
            {18, 36, 56, 80, 120, 80, 56, 36, 18},
            {14, 26, 42, 60, 80, 60, 42, 26, 14 },
            {10, 20, 30, 34, 40, 34, 30, 20, 10 },
            {6, 12, 18, 18, 20, 18, 18, 12, 6 },
            {2, 0, 8, 0, 8, 0, 8, 0, 2 },
            {0, 0, -2, 0, 4, 0, -2, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        private static int[,] carPosValues = new int[10, 9]
        {
            {14, 14, 12, 18, 16, 18, 12, 14, 14 },
            {16, 20, 18, 24, 26, 24, 18, 20, 16},
            {12, 12, 12, 18, 18, 18, 12, 12, 12 },
            {12, 18, 16, 22, 22, 22, 16, 18, 12 },
            {12, 14, 12, 18, 18, 18, 12, 14, 12 },
            {12, 16, 14, 20, 20, 20, 14, 16, 12 },
            {6, 10, 8, 14, 14, 14, 6, 8, 4 },
            {4, 8, 6, 14, 12, 14, 6, 8, 4 },
            {8, 4, 8, 16, 8, 16, 8, 4, 8 },
            {-2, 10, 6, 14, 12, 14, 6, 10, -2 },
        };

        private static int[,] horsePosValues = new int[10, 9]
        {
            {4, 8, 16, 12, 4, 12, 16, 8, 4 },
            {4, 10, 28, 16, 8, 16, 28, 10, 4},
            {12, 14, 16, 20, 18, 20, 16, 14, 12 },
            {8, 24, 18, 24, 20, 24, 18, 24, 8 },
            {6, 16, 14, 18, 16, 18, 14, 16, 6 },
            {4, 12, 16, 14, 12, 14, 16, 12, 4 },
            {2, 6, 8, 6, 10, 6, 8, 6, 2 },
            {4, 2, 8, 8, 4, 8, 8, 2, 4 },
            {0, 2, 4, 4, -2, 4, 4, 2, 0 },
            {0, -4, 0, 0, 0, 0, 0, -4, 0 },
        };

        private static int[,] cannonPosValues = new int[10, 9]
        {
            {6, 4, 0, -10, -12, 10, 0, 4, 6 },
            {2, 2, 0, -4, -14, -4, 0, 2, 2 },
            {2, 2, 0, -10, -8, -10, 0, 2, 2 },
            {0, 0, -2, 4, 10, 4, -2, 0, 0 },
            {0, 0, 0, 2, 8, 2, 0, 0, 0 },
            {-2, 0, 4, 2, 6, 2, 4, 0, -2 },
            {0, 0, 0, 2, 4, 2, 0, 0, 0 },
            {4, 0, 8, 6, 10, 6, 8, 0, 4 },
            {0, 2, 4, 6, 6, 6, 4, 2, 0 },
            {0, 0, 2, 6, 6, 6, 2, 0, 0 },
        };

        private static Dictionary<PieceType, int[,]> piecePosValues = new Dictionary<PieceType, int[,]>
        {
            { PieceType.PAWN, pawnPosValues },
            {PieceType.CANNON, cannonPosValues },
            {PieceType.CAR, carPosValues },
            {PieceType.HORSE, horsePosValues },
        };


        public static int EvaluteValue(Board board, ChessFlag flag)
        {
            int redValue = 0;
            int blackValue = 0;
            int[,] pieceConditions = new int[10, 9];
            int[,] pieceFlex = new int[10, 9];
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    Piece p = board[r, c];
                    if (p == null)
                        continue;
                    foreach (var pos in GetDropPoses(board, new Coord(r, c)))
                    {
                        pieceFlex[r, c]++;
                        if (board[pos] != null)
                        {
                            if (board[pos].Flag == p.Flag && board[pos].Type != PieceType.KING)
                            {
                                pieceConditions[pos.Row, pos.Col]++;
                            }
                            else
                            {
                                pieceConditions[pos.Row, pos.Col]--;
                            }
                        }
                    }
                }
            }
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (board[r, c] != null)
                    {
                        int value = 0;
                        var p = board[r, c];
                        value += baseValues[p.Type];
                        value += flexValues[p.Type] * pieceFlex[r, c];
                        if (piecePosValues.TryGetValue(p.Type, out var posValue))
                        {
                            if (p.Flag == ChessFlag.RED)
                            {
                                value += posValue[r, c];
                            }
                            else
                            {
                                value += posValue[board.Rows - r - 1, c];
                            }
                        }
                        if (pieceConditions[r, c] < 0)
                        {
                            value = (int)(value / 5.0);
                        }
                        if (p.Flag == ChessFlag.RED)
                        {
                            redValue += value;
                        }
                        else
                        {
                            blackValue += value;
                        }
                    }
                }
            }
            return flag == ChessFlag.RED ? redValue - blackValue : blackValue - redValue;
        }

    }
}

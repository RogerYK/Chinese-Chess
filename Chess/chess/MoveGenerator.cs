using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.chess
{
    static class MoveGenerator
    {
        enum Direction
        {
            UP,
            RIGHT,
            DOWN,
            LEFT
        }

        public static Dictionary<Piece, Func<Board, Coord, IEnumerable<Coord>>> dropFunctions = new Dictionary<Piece, Func<Board, Coord, IEnumerable<Coord>>>
        {
            {Piece.B_Pawn,  GetPawnDropPoses},
            {Piece.B_Cannon, GetCannonDropPoses},
            {Piece.B_Car, GetCarDorpPoses },
            {Piece.B_Horse, GetHorseDropPoses },
            {Piece.B_Elephant, GetElephantPoses },
            {Piece.B_Bishop, GetBishopDropPoses },
            {Piece.B_King, GetKingDropPoses },
            {Piece.R_Pawn,  GetPawnDropPoses},
            {Piece.R_Cannon, GetCannonDropPoses},
            {Piece.R_Car, GetCarDorpPoses },
            {Piece.R_Horse, GetHorseDropPoses },
            {Piece.R_Elephant, GetElephantPoses },
            {Piece.R_Bishop, GetBishopDropPoses },
            {Piece.R_King, GetKingDropPoses },
        };


        public static IEnumerable<Move> GetAllMoves(Board board, ChessFlag flag)
        {
            for (int r = 0; r <board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (board[r, c] != null && board[r, c].Flag == flag)
                    {
                        var from = new Coord(r, c);
                        foreach (var to in GetDropPoses(board, from))
                        {
                            yield return new Move(from, to);
                        }
                    }
                }
            }
        }

        public static List<Coord> GetDropPoses(Board board, Coord from)
        {
            if (board[from] == null) return null;

            return dropFunctions[board[from]](board, from)
                .Where(pos => board.IsInBoard(pos) && (board[pos] == null || board[pos].Flag != board[from].Flag))
                .ToList();
        }

        public static List<Coord> GetDropPosesNoFlag(Board board, Coord from)
        {
            if (board[from] == null) return null;

            return dropFunctions[board[from]](board, from)
                .Where(pos => board.IsInBoard(pos))
                .ToList();
        }

        public static IEnumerable<Coord> GetPawnDropPoses(Board board, Coord from)
        {
            var piece = board[from.Row, from.Col];
            var poses = new List<Coord>();
            var rowOffset = piece.Flag == ChessFlag.BLACK ? 1 : -1;

            if ((from.Row >= 5 && piece.Flag == ChessFlag.BLACK)
                    || (from.Row < 5 && piece.Flag == ChessFlag.RED))
            {
                poses.Add(new Coord(from.Row, from.Col - 1));
                poses.Add(new Coord(from.Row, from.Col + 1));
                poses.Add(new Coord(from.Row + rowOffset, from.Col));
            }
            else
            {
                poses.Add(new Coord(from.Row + rowOffset, from.Col));
            }
            return poses;
        }

        public static IEnumerable<Coord> GetCannonDropPoses(Board board, Coord from)
        {
            int[][] offsets = { new int[]{ 1, 0 }, new int[]{ -1, 0 }, new int[]{ 0, -1 }, new int[]{ 0, 1 } };
            foreach (var offset in offsets)
            {
                var target = new Coord(from.Row, from.Col);
                target.Row += offset[0];
                target.Col += offset[1];
                while (board.IsInBoard(target) && board[target] == null)
                {
                    yield return target;
                    target.Row += offset[0];
                    target.Col += offset[1];
                }
                target.Row += offset[0];
                target.Col += offset[1];
                if (board.IsInBoard(target))
                {
                    while (board.IsInBoard(target) && board[target] == null)
                    {
                        target.Row += offset[0];
                        target.Col += offset[1];
                    }
                    if (board.IsInBoard(target))
                    {
                        yield return target;
                    }
                }
            }
        }

        public static IEnumerable<Coord> GetCarDorpPoses(Board board, Coord from)
        {
            int[][] offsets = { new int[]{ 1, 0 }, new int[]{ -1, 0 }, new int[]{ 0, -1 }, new int[]{ 0, 1 } };
            foreach (var offset in offsets)
            {
                var target = from;
                target.Row += offset[0];
                target.Col += offset[1];
                while (board.IsInBoard(target) && board[target] == null)
                {
                    yield return target;
                    target.Row += offset[0];
                    target.Col += offset[1];
                }
                if (board.IsInBoard(target))
                {
                    yield return target;
                }
            }
        }

        public static IEnumerable<Coord> GetHorseDropPoses(Board board, Coord from)
        {
            int[][] offsets = { new int[]{2, 1}, new int[]{ 2, -1},
                            new int[]{ -2, 1}, new int[]{ -2, -1 },
                            new int[]{1, -2 }, new int[] {1, 2},
                            new int[] {-1, -2},new int[]{-1, 2}};
            return offsets.Select(offset =>
                    new
                    {
                        pos = new Coord(from.Row + offset[0], from.Col + offset[1]),
                        obs = new Coord(from.Row + offset[0] / 2, from.Col + offset[1]/2)
                    })
                .Where(item => board.IsInBoard(item.obs) && board[item.obs] == null)
                .Select(item => item.pos);
        }

        private static IEnumerable<Coord> GetElephantPoses(Board board, Coord from)
        {
            var poses = new List<Coord>();
            var flag = board[from].Flag;
            int[][] offsets = { new int[]{ 2, 2 }, new int[]{ 2, -2 }, new int[]{ -2, 2 }, new int[]{ -2, -2 } };
            return offsets.Select(offset => new
            {
                pos =  new Coord(from.Row + offset[0], from.Col + offset[1]),
                obs = new Coord(from.Row + offset[0]/2, from.Col + offset[1]/2)
            }).Where(item => board.IsInBoard(item.obs) && board[item.obs] == null)
                .Select(item => item.pos)
                .Where(pos => (flag == ChessFlag.BLACK && pos.Row < 5)
                    || (flag == ChessFlag.RED && pos.Row >= 5));
        }

        private static IEnumerable<Coord> GetBishopDropPoses(Board board, Coord from)
        {
            var flag = board[from].Flag;
            int[][] offsets = { new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 }, new int[] { -1, 1 } };
            return offsets.Select(offset => new Coord(from.Row + offset[0], from.Col + offset[1]))
                .Where(pos => (pos.Col >= 3 && pos.Col <= 5
                    && ((flag == ChessFlag.BLACK && pos.Row < 3) || (flag == ChessFlag.RED && pos.Row >= 7)))
                );
        }

        private static IEnumerable<Coord> GetKingDropPoses(Board board, Coord from)
        {
            var flag = board[from].Flag;
            int[][] offsets = { new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 } };
            return offsets.Select(offset => new Coord(from.Row + offset[0], from.Col + offset[1]))
                .Where(pos => (pos.Col >= 3 && pos.Col <= 5
                    && ((flag == ChessFlag.BLACK && pos.Row < 3) || (flag == ChessFlag.RED && pos.Row >= 7)))
                    );
        }
    }
}

using Chess.chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.chess.MoveGenerator;

namespace Chess.Ai
{
    interface ISearchEngine
    {

        Move SeachABestMove(Board board, ChessFlag flag);
    }

    class NegaScoutEngine : ISearchEngine
    {
        public int MaxDepth { private set; get; }

        private Move bestMove;

        private int counter;
        private int error;

        public NegaScoutEngine()
        {
            MaxDepth = 4;
        }

        public Move SeachABestMove(Board board, ChessFlag flag)
        {
            error = counter = 0;
            NegaScout(0, int.MinValue + 10, int.MaxValue, board, flag);
            Console.WriteLine($"搜索共预测{counter}次， 错误{error}次。");
            return bestMove;
        }

        private int NegaScout(int depth, int alpha, int beta, Board board, ChessFlag findFlag)
        {
            ChessFlag curFlag = (depth % 2 == 0) ? findFlag : 
                (findFlag == ChessFlag.RED ? ChessFlag.BLACK : ChessFlag.RED);
            if (depth >= MaxDepth || board.IsOver())
            {
                return Evaluation.EvaluteValue(board, curFlag);
            }

            int best;
            var moves = GetAllMoves(board, curFlag).ToList(); 
            var iter = moves.GetEnumerator();
            iter.MoveNext();
            var move = iter.Current;
            board.Move(move);
            best = -NegaScout(depth+1, -beta, -alpha, board, findFlag);
            board.UndoMove();
            if (depth == 0)
            {
                bestMove = move;
            }
            while (iter.MoveNext())
            {
                if (best > beta)
                {
                    break;
                }
                if (best > alpha)
                {
                    alpha = best;
                }

                move = iter.Current;
                board.Move(move);
                var score = -NegaScout(depth + 1, Math.Max(-beta, -alpha - 100), -alpha, board, findFlag);
                if (score > alpha + 10 && score < beta)
                {
                    score = -NegaScout(depth + 1, -beta, -alpha, board, findFlag);
                    error++;
                }
                counter++;
                board.UndoMove();

                if (score > best)
                {
                    best = score;
                    if (depth == 0)
                    {
                        bestMove = move;
                    }
                }
            }
            return best;
        }

    }
}

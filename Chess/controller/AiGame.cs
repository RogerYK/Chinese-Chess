using Chess.Ai;
using Chess.chess;
using Chess.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Chess.chess.MoveGenerator;

namespace Chess.controller
{
    class AiGame
    {
        private ChessFlag AiFlag { set; get; } = ChessFlag.BLACK;

        private view.Board viewBoard;

        private chess.Board board;

        private chess.Board seachBoard = new chess.Board();

        private ISearchEngine searchEngine = new NegaScoutEngine();

        private bool searching = false;

        private Coord? selectedPos;

        private List<Coord> dropPoses;


        private GameWindow gameWindow = new GameWindow();

        public AiGame()
        {
            InitAiGame();
        }

        public void Show()
        {
            gameWindow.Show();
        }

        private void InitAiGame()
        {
            gameWindow.Title = "中国象棋-人机";
            viewBoard = gameWindow.Board;
            board = gameWindow.Board.ChessBoard;
            gameWindow.Board.ClickPos += OnClickPos;
        }

        private void OnClickPos(chess.Board board, Coord pos)
        {
            if (!searching)
            {

                if (board[pos] != null)
                {
                    var piece = board[pos];
                    if (board[pos].Flag != AiFlag)
                    {
                        selectedPos = pos;
                        dropPoses = GetDropPoses(board, pos);
                    }
                    else
                    {
                        if (dropPoses != null && dropPoses.Exists(p => p.Equals(pos)))
                        {
                            MovePiece(selectedPos.Value, pos);
                        }
                    }
                }
                else
                {
                    if (dropPoses != null && dropPoses.Exists(p => p.Equals(pos)))
                    {
                        MovePiece(selectedPos.Value, pos);
                    }
                }
            }
        }

        private async Task MovePiece(Coord from, Coord to)
        {
            board.Move(new Move(from, to));
            seachBoard.Move(new Move(from, to));
            viewBoard.InvalidateVisual();
            if (board.IsOver())
            {
                ShowResult();
                return;
            }

            searching = true;
            var move = await Task.Run(() =>
            {
                return searchEngine.SeachABestMove(seachBoard, AiFlag);

            });
            board.Move(move);
            seachBoard.Move(move);
            viewBoard.InvalidateVisual();
            searching = false;
            if (board.IsOver())
            {
                ShowResult();
            }
        }

        private void ShowResult()
        {
            if (board.IsOver())
            {
                var winner = board.Winner();
                var winnerStr = (winner == ChessFlag.BLACK) ? "黑方" : "红方";
                MessageBox.Show($"恭喜{winnerStr}方获胜！！", "游戏结束");
                gameWindow.Dispatcher.Invoke(() => gameWindow.Close());
            }
        }
    }
}

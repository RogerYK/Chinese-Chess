using Chess.chess;
using Chess.network;
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
    class HumanGame
    {
        private GameWindow gameWindow;

        private view.Board viewBoard;

        private chess.Board board;

        private LocalSocket socket;

        private bool turnSelf = false;

        private bool waitOk = false;

        private Move waitMove;

        private Coord? selectedPos;

        private List<Coord> dropPoses;

        private ChessFlag selfFlag;

        public HumanGame(LocalSocket socket, ChessFlag flag)
        {
            selfFlag = flag;
            gameWindow = new GameWindow();
            gameWindow.Title = "中国象棋-双人";
            viewBoard = gameWindow.Board;
            viewBoard.UpChessFlag = (flag == ChessFlag.BLACK) ? ChessFlag.RED : ChessFlag.BLACK;
            viewBoard.ClickPos += OnClickPos;
            board = viewBoard.ChessBoard;
            this.socket = socket;
            socket.ReceivedOk += OnReceiveOk;
            socket.ReceivedMove += OnAnotherMove;
            if (flag == ChessFlag.RED)
            {
                turnSelf = true;
            }
            gameWindow.Closed += Exit;
        }

        private void Exit(object sender, EventArgs args)
        {
            Task.Run(async () =>
            {
                await socket.Exit();
            }).Wait();
        }

        public void Show()
        {
            gameWindow.Show();
        }

        private void OnClickPos(chess.Board board, Coord cd)
        {
            if (turnSelf && (!waitOk))
            {
                if (board[cd] != null)
                {
                    var piece = board[cd];
                    if (board[cd].Flag == selfFlag)
                    {
                        selectedPos = cd;
                        dropPoses = GetDropPoses(board, cd);
                    }
                    else
                    {
                        if (dropPoses != null && dropPoses.Exists(p => p.Equals(cd)))
                        {
                            MovePiece(selectedPos.Value, cd);
                        }
                    }
                }
                else
                {
                    if (dropPoses != null && dropPoses.Exists(p => p.Equals(cd)))
                    {
                        MovePiece(selectedPos.Value, cd);
                    }
                }
            }
        }

        private void MovePiece(Coord from, Coord to)
        {
            Move move = new Move(from, to);
            waitMove = move;
            waitOk = true;
            socket.SendMove(move);
        }

        private void OnReceiveOk() => gameWindow.Dispatcher.Invoke(() =>
       {
           waitOk = false;
           board.Move(waitMove);
           viewBoard.InvalidateVisual();
           turnSelf = false;
           if (board.IsOver())
               ShowResult();
           Console.WriteLine("Receive OK");
       });

        private void OnAnotherMove(Move move) => gameWindow.Dispatcher.Invoke(() =>
        {
            board.Move(move);
            viewBoard.InvalidateVisual();
            socket.SendOk();
            turnSelf = true;
            if (board.IsOver())
            {
                ShowResult();
            }
            Console.WriteLine("Receive Move");
        });

      
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

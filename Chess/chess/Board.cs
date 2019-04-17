using Chess.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.chess
{
    public class Board
    {
        struct Record
        {
            public Move Move { set; get; }
            public Piece TargetPiece {set; get;}

            public Record(Move move, Piece piece)
            {
                Move = move;
                TargetPiece = piece;
            }
        }

        private Piece[,] pieces = Constant.INIT_PIECES;

        private Stack<Record> history = new Stack<Record>();

        public int Rows = Constant.BOARD_ROW;

        public int Cols = Constant.BOARD_COL;

        public bool IsOver()
        {
            int cnt = 0;
            foreach (var p in pieces)
            {
                if (p == null)
                    continue;
                if (p.Type == PieceType.KING)
                {
                    cnt++;
                }
            }
            return cnt != 2;
        }

        public Piece this[int r, int c]
        {
            get
            {
                return pieces[r, c];
            }

            set
            {
                pieces[r, c] = value;
            }
        }

        public Piece this[Coord pos]
        {
            get
            {
                return pieces[pos.Row, pos.Col];
            }

            set
            {
                pieces[pos.Row, pos.Col] = value;
            }
        }

        public bool IsInBoard(Coord pos)
        {
            return pos.Row >= 0 && pos.Row < Rows && pos.Col >= 0 && pos.Col < Cols;
        }

        public void Move(Move move)
        {
            Record record = new Record(move, this[move.To]);
            history.Push(record);
            this[move.To] = this[move.From];
            this[move.From] = null;
        }

        public void UndoMove()
        {
            var record = history.Pop();
            var move = record.Move;
            this[move.From] = this[move.To];
            this[move.To] = record.TargetPiece;
        }

        public ChessFlag Winner()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c =0; c < Cols; c++)
                {
                    if (pieces[r, c] != null && pieces[r, c].Type == PieceType.KING)
                    {
                        return pieces[r, c].Flag;
                    }
                }
            }
            throw new Exception("NoWinner");
        }
        
    }
}

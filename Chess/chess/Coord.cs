using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.chess
{
    [Serializable]
    public struct Coord
    {
        public int Row { set; get; }
        public int Col { set; get; }

        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    [Serializable]
    public struct Move
    {
        public Coord From { set; get; }
        public Coord To { set; get; }

        public Move(Coord from, Coord to)
        {
            From = from;
            To = to;
        }
    }
}

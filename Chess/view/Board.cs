using Chess.chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Chess.view.Helper;


namespace Chess.view
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Chess.view"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Chess.view;assembly=Chess.view"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:Board/>
    ///
    /// </summary>
    public class Board : Control
    {
        static Board()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Board), new FrameworkPropertyMetadata(typeof(Board)));
        }

        class MVConvert
        {
            public ChessFlag UpFlag { set; get; } = ChessFlag.BLACK;

            public Coord ViewToModel(System.Windows.Point pos)
            {
                double ySpan = pos.Y - Constant.START_POS.Y;
                if (ySpan > 5 * Constant.CELL_SIZE)
                {
                    pos.Y -= 18;
                }
                int c = (int)(pos.X - Constant.START_POS.X) / Constant.CELL_SIZE;
                int r = (int)(pos.Y - Constant.START_POS.Y) / Constant.CELL_SIZE;
                if (UpFlag != ChessFlag.BLACK)
                {
                    r = Constant.BOARD_ROW - 1 - r;
                }
                return new Coord { Row = r, Col = c };
            }

            public System.Windows.Point ModelToView(Coord cd)
            {
                if (UpFlag != ChessFlag.BLACK)
                {
                    cd.Row = Constant.BOARD_ROW - cd.Row - 1;
                }
                double x = cd.Col * Constant.CELL_SIZE + Constant.START_POS.X;
                double y = cd.Row * Constant.CELL_SIZE + Constant.START_POS.Y;
                if (cd.Row >= 5)
                {
                    y += 18;
                }
                return new System.Windows.Point(x, y);
            }
        }

        private MVConvert convert = new MVConvert();

        private Coord? selectedPos;

        private Coord[] CanDropPoses { set; get; }

        private chess.Board chessBoard;

        public chess.Board ChessBoard
        {
            get
            {
                return chessBoard;
            }
            set
            {
                chessBoard = value;
            }
        }

        public ChessFlag UpChessFlag
        {
            set
            {
                convert.UpFlag = value;
            }
            get
            {
                return convert.UpFlag;
            }
        }

        public event Action<chess.Board, Coord> ClickPos;

        public Board()
        {
            Width = Constant.BOARD_WIDTH;
            Height = Constant.BOARD_HEIGHT;
            MouseDown += RaiseClick;
            ChessBoard = new chess.Board();
        }

        private void RaiseClick(object send, MouseButtonEventArgs args)
        {
            var pos = args.GetPosition(this);
            double offsetX = pos.X - Constant.START_POS.X;
            double offsetY = pos.Y - Constant.START_POS.Y;
            if (offsetX >= 0 && offsetX <= Constant.CELL_SIZE * Constant.BOARD_COL
                && offsetY >= 0 && offsetY <= Constant.CELL_SIZE * Constant.BOARD_ROW)
            {
                ClickPos?.Invoke(ChessBoard, convert.ViewToModel(pos));
            }

        }


        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawImage(BitmapToImageSource(Chess.Properties.Resources.board),
                    new Rect(0, 0, Width, Height));

            if (ChessBoard != null)
            {
                RenderPieces(dc);
            }

        }

        private void RenderPieces(DrawingContext dc)
        {
            for (int r = 0; r < ChessBoard.Rows; r++)
            {
                for (int c = 0; c < ChessBoard.Cols; c++)
                {
                    if (ChessBoard[r, c] != null)
                    {
                        var pos = convert.ModelToView(new chess.Coord { Row = r, Col = c });

                        dc.DrawImage(BitmapToImageSource(ChessBoard[r, c].Img),
                            new Rect(pos, new Size(ChessBoard[r, c].Img.Width, ChessBoard[r, c].Img.Height)));
                    }
                }
            }
        }
    }
}

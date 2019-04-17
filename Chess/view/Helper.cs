using Chess.chess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Interop.Imaging;


namespace Chess.view
{
    static class Helper
    {
        public static ImageSource BitmapToImageSource(Bitmap map)
        {
            IntPtr hBitmap = map.GetHbitmap();
            return CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static Coord ViewToModel(System.Windows.Point pos)
        {
            double ySpan = pos.Y - Constant.START_POS.Y;
            if (ySpan > 5 * Constant.CELL_SIZE)
            {
                pos.Y -= 18;
            }
            int c = (int)(pos.X - Constant.START_POS.X)/Constant.CELL_SIZE;
            int r = (int)(pos.Y - Constant.START_POS.Y)/Constant.CELL_SIZE;
            return new Coord { Row = r, Col = c };
        }

        public static System.Windows.Point ModelToView(Coord cd)
        {
            double x = cd.Col * Constant.CELL_SIZE + Constant.START_POS.X;
            double y = cd.Row * Constant.CELL_SIZE + Constant.START_POS.Y;
            if (cd.Row >= 5) y += 18;
            return new System.Windows.Point(x, y);
        }

        public static T CastTo<T>(this object o)
        {
            return (T)o;
        }
    }
}

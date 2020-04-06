using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellToolDK;
using System.Drawing;

namespace Colocalization
{
    class ROIManager
    {
        private static List<Point> GetPointsInOvalTracking(ROI roi, TifFileInfo fi, int frame)
        {
            //FillEllipse(roi, fi, 0);
            //get the location of the first value
            Point p = roi.GetLocation(frame)[0];
            int X = p.X;
            int Y = p.Y;
            int W = roi.Width;
            int H = roi.Height;
            //create shablon for preventing retacking the same value
            bool[,] shablon = new bool[fi.sizeY, fi.sizeX];

            return CalculateEllipse(shablon, X, Y, W, H);
        }
        private static List<Point> CalculateEllipse(bool[,] shablon, int Xn, int Yn, int Wn, int Hn)
        {
            List<Point> pList = new List<Point>();

            int left = Xn;
            int top = Yn;
            int right = Xn + Wn;
            int bottom = Yn + Hn;

            int a, b, x, y, temp;
            int old_y;
            int d1, d2;
            int a2, b2, a2b2, a2sqr, b2sqr, a4sqr, b4sqr;
            int a8sqr, b8sqr, a4sqr_b4sqr;
            int fn, fnw, fw;
            int fnn, fnnw, fnwn, fnwnw, fnww, fww, fwnw;

            if (right < left)
            {
                temp = left;
                left = right;
                right = temp;
            }
            if (bottom < top)
            {
                temp = top;
                top = bottom;
                bottom = temp;
            }

            a = (right - left) / 2;
            b = (bottom - top) / 2;

            x = 0;
            y = b;

            a2 = a * a;
            b2 = b * b;
            a2b2 = a2 + b2;
            a2sqr = a2 + a2;
            b2sqr = b2 + b2;
            a4sqr = a2sqr + a2sqr;
            b4sqr = b2sqr + b2sqr;
            a8sqr = a4sqr + a4sqr;
            b8sqr = b4sqr + b4sqr;
            a4sqr_b4sqr = a4sqr + b4sqr;

            fn = a8sqr + a4sqr;
            fnn = a8sqr;
            fnnw = a8sqr;
            fnw = a8sqr + a4sqr - b8sqr * a + b8sqr;
            fnwn = a8sqr;
            fnwnw = a8sqr + b8sqr;
            fnww = b8sqr;
            fwnw = b8sqr;
            fww = b8sqr;
            d1 = b2 - b4sqr * a + a4sqr;

            while ((fnw < a2b2) || (d1 < 0) || ((fnw - fn > b2) && (y > 0)))
            {
                DrawHorizontalOvalLine(left + x, right - x, top + y, shablon, pList); // Replace with your own span filling function. The hard-coded numbers were color values for testing purposes and can be ignored.
                DrawHorizontalOvalLine(left + x, right - x, bottom - y, shablon, pList);

                y--;
                if ((d1 < 0) || (fnw - fn > b2))
                {
                    d1 += fn;
                    fn += fnn;
                    fnw += fnwn;
                }
                else
                {
                    x++;
                    d1 += fnw;
                    fn += fnnw;
                    fnw += fnwnw;
                }
            }

            fw = fnw - fn + b4sqr;
            d2 = d1 + (fw + fw - fn - fn + a4sqr_b4sqr + a8sqr) / 4;
            fnw += b4sqr - a4sqr;

            old_y = y + 1;

            while (x <= a)
            {
                if (y != old_y) // prevent overdraw
                {
                    DrawHorizontalOvalLine(left + x, right - x, top + y, shablon, pList);
                    DrawHorizontalOvalLine(left + x, right - x, bottom - y, shablon, pList);
                }

                old_y = y;
                x++;
                if (d2 < 0)
                {
                    y--;
                    d2 += fnw;
                    fw += fwnw;
                    fnw += fnwnw;
                }
                else
                {
                    d2 += fw;
                    fw += fww;
                    fnw += fnww;
                }
            }

            return pList;
        }
        private static void DrawHorizontalOvalLine(int left, int right, int y, bool[,] shablon, List<Point> pList)
        {
            if (left < 0) left = 0;
            if (y < 0) y = 0;
            if (left >= shablon.GetLength(1)) left = shablon.GetLength(1) - 1;
            if (y >= shablon.GetLength(0)) y = shablon.GetLength(0) - 1;

            if (right < 0) right = 0;
            if (right >= shablon.GetLength(1)) right = shablon.GetLength(1) - 1;

            for (int x = left; x <= right; x++)
                if (shablon[y, x] == false)
                {
                    shablon[y, x] = true;
                    pList.Add(new Point(x, y));
                }
        }
        private static List<Point> GetPointsInRectangleTracking(ROI roi, TifFileInfo fi, int frame)
        {
            //get the location of the first value
            Point p = roi.GetLocation(frame)[0];
            int X = p.X;
            int Y = p.Y;
            int W = roi.Width;
            int H = roi.Height;
            //create shablon for preventing retacking the same value
            bool[,] shablon = new bool[fi.sizeY, fi.sizeX];
            
            X -= roi.D * roi.Stack;
            Y -= roi.D * roi.Stack;
            W += (roi.D + roi.D) * roi.Stack;
            H += (roi.D + roi.D) * roi.Stack;

            return CalculateRectangle(shablon, X, Y, W, H);

        }
        private static List<Point> CalculateRectangle(bool[,] shablon, int X, int Y, int W, int H)
        {
            List<Point> pList = new List<Point>();

            int X1 = X + W;
            int Y1 = Y + H;

            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
            if (X1 >= shablon.GetLength(1)) X1 = shablon.GetLength(1) - 1;
            if (Y1 >= shablon.GetLength(0)) Y1 = shablon.GetLength(0) - 1;

            for (int curY = Y; curY <= Y1; curY++)
                for (int curX = X; curX <= X1; curX++)
                    if (shablon[curY, curX] == false)
                    {
                        shablon[curY, curX] = true;
                        pList.Add(new Point(curX, curY));
                    }

            return pList;
        }
        private static List<Point> GetPolygonPoints(Point[] points, TifFileInfo fi)
        {
            List<Point> pList = new List<Point>();
            List<int> xList = new List<int>();
            int x, y, swap, i, j, maxY = 0, minY = fi.sizeY - 1;
            //check the size of the polygon
            foreach (Point p in points)
            {
                if (p.Y > maxY && p.Y < fi.sizeY) maxY = p.Y;
                if (p.Y < minY && p.Y >= 0) minY = p.Y;
            }
            //scan lines for Y coords
            for (y = minY; y <= maxY; y++)
            {
                //prepare list for X coords
                xList.Clear();
                j = points.Length - 1;
                //calculate X points via tgA function
                for (i = 0; i < points.Length; i++)
                {
                    if ((points[i].Y < y && points[j].Y >= y) ||
                        (points[j].Y < y && points[i].Y >= y))
                    {
                        // tgA = (y2-y1)/(x2-x1)
                        x = (int)((((y - points[i].Y) * (points[j].X - points[i].X)) /
                            (points[j].Y - points[i].Y)) + points[i].X);

                        xList.Add(x);
                    }

                    j = i;
                }
                //break if there is no points in the line
                if (xList.Count == 0) continue;
                //sort by value via bubble loop
                i = 0;
                while (i < xList.Count - 1)
                {
                    j = i + 1;
                    if (xList[i] > xList[j])
                    {
                        swap = xList[i];
                        xList[i] = xList[j];
                        xList[j] = swap;

                        if (i > 0) i--;
                    }
                    else
                    {
                        i++;
                    }
                }
                //find all points inside the bounds 2 by 2
                for (i = 0; i < xList.Count; i += 2)
                {
                    j = i + 1;
                    if (xList[i] >= fi.sizeX) break;
                    if (xList[j] > 0)
                    {
                        if (xList[i] < 0) xList[i] = 0;
                        if (xList[j] >= fi.sizeX) xList[j] = fi.sizeX - 1;

                        for (x = xList[i]; x <= xList[j]; x++)
                            pList.Add(new Point(x, y));
                    }
                }
            }

            return pList;
        }
        public static List<Point> GetROIPoints(int frame,ROI roi,TifFileInfo fi)
        {
            switch (roi.Shape)
            {
                case 0:
                    return GetPointsInRectangleTracking(roi, fi, frame);                   
                case 1:
                    return GetPointsInOvalTracking(roi, fi, frame);                    
                default:
                   return GetPolygonPoints(roi.GetLocation(frame),fi);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Colocalization
{
    class Operations
    {
        /// <summary>
        /// Calculates points array with values corresponding to each pixel in both images
        /// </summary>
        /// <param name="image1">First image</param>
        /// <param name="image2">Secound image</param>
        /// <returns>Points array with values corresponding to each pixel in both images</returns>
        public static Point[] PairImages(byte[][] image1, byte[][] image2)
        {
            Point[] output = new Point[image1.Length * image1[0].Length];

            for (int y = 0, i = 0; y < image1.Length; y++)
                for (int x = 0; x < image1[0].Length; x++, i++)
                    output[i] = new Point((int)image1[y][x], (int)image2[y][x]);

            return output;
        }
        public static Point[] PairImages(byte[][] image1, byte[][] image2, Point[] input)
        {
            Point[] output = new Point[input.Length];
            Point p = Point.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                p = input[i];
                output[i] = new Point((int)image1[p.Y][p.X], (int)image2[p.Y][p.X]);
            }

            return output;
        }
        public static Point[] PairImages(ushort[][] image1,ushort[][] image2, Point[] input)
        {
            Point[] output = new Point[input.Length];
            Point p = Point.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                p = input[i];
                output[i] = new Point((int)image1[p.Y][p.X], (int)image2[p.Y][p.X]);
            }

            return output;
        }
        /// <summary>
        /// Calculates points array with values corresponding to each pixel in both images
        /// </summary>
        /// <param name="image1">First image</param>
        /// <param name="image2">Secound image</param>
        /// <returns>Points array with values corresponding to each pixel in both images</returns>
        public static Point[] PairImages(ushort[][] image1, ushort[][] image2)
        {
            Point[] output = new Point[image1.Length * image1[0].Length];

            for (int y = 0, i = 0; y < image1.Length; y++)
                for (int x = 0; x < image1[0].Length; x++, i++)
                    output[i] = new Point((int)image1[y][x], (int)image2[y][x]);

            return output;
        }
        public static PointF[] ConvertPointArrayToPointF(Point[] input)
        {
            PointF[] output = new PointF[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new PointF((float)input[i].X, (float)input[i].Y);

            return output;
        }
        public static Point[] ConvertPointFArrayToPoint(PointF[] input)
        {
            Point[] output = new Point[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new Point((int)input[i].X, (int)input[i].Y);

            return output;
        }
        public static PointF[] ScalePointF(PointF[] input,float scale)
        {
            PointF[] output = new PointF[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new PointF(input[i].X* scale, input[i].Y* scale);

            return output;
        }
        public static PointF[] ScalePoint(Point[] input, float scale)
        {
            PointF[] output = new PointF[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new PointF((float)input[i].X * scale, (float)input[i].Y * scale);

            return output;
        }
        public static PointF[] AddYToPoints(PointF[] input, float Yval)
        {
            PointF[] output = new PointF[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new PointF(input[i].X,Yval- input[i].Y);

            return output;
        }
        public static PointF[] TranslatePoint(PointF[] input, Point corner)
        {
            PointF[] output = new PointF[input.Length];

            for (int i = 0; i < input.Length; i++)
                output[i] = new PointF(input[i].X + corner.X, input[i].Y + corner.Y);

            return output;
        }
        public static int MaxValueInPoints(Point[] input)
        {
            int output = int.MinValue;

            foreach(var p in input)
            {
                if (output < p.X) output = p.X;
                if (output < p.Y) output = p.Y;
            }
            return output;
        }
        public static int MinValueInPoints(Point[] input)
        {
            int output = int.MaxValue;

            foreach (var p in input)
            {
                if (output > p.X) output = p.X;
                if (output > p.Y) output = p.Y;
            }
            return output;
        }
        public static float[] GetLUT(int max, int min)
        {
            float[] output = new float[max+1];
            float scale = 1f / ((float)max - (float)min);
            float val = 0f;

            for (int i = min; i <= max; i++, val += scale)
                if (val <= 1f)                
                    output[i] = val;                
                else
                    output[i] = 1f;
            
            return output;
        }
        /// <summary>
        /// R^2 - coeffitient of determination
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Rsquared(Point[] input)
        {
            return Math.Pow(PCC(input),2);
        }
        /// <summary>
        /// Pearson's correlation coefitient - Menders
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double PCC(Point[] input)
        {
            double output = 0;

            double Ravg = GetMean(input, 0);
            double Gavg = GetMean(input, 1);
            double dev = GetDeviations(input, Gavg, Ravg);
            double RdevPow = GetPowDeviations(input, 0, Ravg);
            double GdevPow = GetPowDeviations(input, 1, Gavg);

            output = dev / Math.Sqrt(RdevPow * GdevPow);

            return output;
        }
        /// <summary>
        /// Manders overlap coefficient
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double MOC(Point[] input)
        {
            double output = 0;

            double Ravg = 0;
            double Gavg = 0;
            double dev = GetDeviations(input, Gavg, Ravg);
            double RdevPow = GetPowDeviations(input, 0, Ravg);
            double GdevPow = GetPowDeviations(input, 1, Gavg);

            output = dev / Math.Sqrt(RdevPow * GdevPow);

            return output;
        }
        private static double GetMean(Point[] input, int index)
        {
            double output = 0;
            double counter = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (index == 0)
                    output += (double)input[i].X;
                else
                    output += (double)input[i].Y;

                counter++;
            }

            return output/counter;

        }
        private static double GetDeviations(Point[] input, double Gavg, double Ravg)
        {
            double output = 0;

            for (int i = 0; i < input.Length; i++)
            {
               
                    output += ((double)input[i].X-Ravg)*((double)input[i].Y - Gavg);
            }

            return output;

        }
        private static double GetPowDeviations(Point[] input, int index, double avg)
        {
            double output = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (index == 0)
                    output += Math.Pow((double)input[i].X - avg,2.0);
                else
                    output += Math.Pow((double)input[i].Y - avg,2.0);

            }

            return output;
        }
        public static Point[] FilterPointsByThresholds(Point[] input, int threshold1, int threshold2)
        {
            List<Point> output = new List<Point>();

            for (int i = 0; i < input.Length; i++)
                if ((int)input[i].X >= threshold1 || input[i].Y >= threshold2)
                    output.Add(new Point((int)input[i].X, (int)input[i].Y));

            return output.ToArray();
        }
       /// <summary>
       /// Mander's Colocalization Coefficient
       /// </summary>
       /// <param name="input"></param>
       /// <param name="threshold1"></param>
       /// <param name="threshold2"></param>
       /// <returns></returns>
        public static double[] MCC(Point[] input, int threshold1,int threshold2)
        {
            double sum1coloc = 0;
            double sum2coloc = 0;
            double sum1 = 0;
            double sum2 = 0;

            foreach(var p in input)
            {
                sum1 += (double)p.X;
                sum2 += (double)p.Y;
                if (p.Y >= threshold2) sum1coloc += (double)p.X;
                if (p.X >= threshold1) sum2coloc += (double)p.Y;
            }

            double[] output = new double[2];

            if (sum1 != 0) output[0] = sum1coloc / sum1;
            if (sum2 != 0) output[1] = sum2coloc / sum2;

            return  output;
        }
    }
}

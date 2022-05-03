using Bitmap;

namespace QRCodes.Reader
{
    class Shape
    {
        //Variables
        private readonly BitMap binarized;
        private readonly Coordinate p1, p2, p3, p4;
        private int estimatedSize = -1;

        private double a;
        private double b;
        private double c;
        private double d;
        private double e;
        private double f;
        private double g;
        private double h;
        
        //Constructeurs
        internal Shape(BitMap binarized, Coordinate p1, Coordinate p2, Coordinate p3, Coordinate p4)
        {
            this.binarized = binarized;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public int GetEstimatedSize()
        {
            return estimatedSize; 
        }

        public void SetEstimatedSize(int size)
        {
            this.estimatedSize = size;

            double v1 = p1.Y, v2 = p2.Y, v3 = p3.Y, v4 = p4.Y;
            double u1 = p1.X, u2 = p2.X, u3 = p3.X, u4 = p4.X;

            double du1 = (u3 - u2) * size;
            double du2 = (u1 - u4) * size;
            double du3 = u2 - u3 + u4 - u1;

            double dv1 = (v3 - v2) * size;
            double dv2 = (v1 - v4) * size;
            double dv3 = v2 - v3 + v4 - v1;

            double d = du1 * dv2 - dv1 * du2;

            this.g = (du3 * dv2 - dv3 * du2) / d;
            this.h = (du1 * dv3 - dv1 * du3) / d;
            this.a = (u3 - u2) / size + g * u3;
            this.b = (u1 - u2) / size + h * u1;
            this.c = u2;
            this.d = (v3 - v2) / size + g * v3;
            this.e = (v1 - v2) / size + h * v1;
            this.f = v2;
        }

        //Methodes
        public bool IsBlack(int y, int x)
        {
            double xx = x + 0.5;
            double yy = y + 0.5;
            double q = g * xx + h * yy + 1;
            int u = (int)((a * xx + b * yy + c) / q);
            int v = (int)((d * xx + e * yy + f) / q);
            return Get(v, u);
        }
        private bool Get(int y, int x)
        {
            int count = 0;
            if (binarized.GetPixel(y + 0, x + 0) == Colors.BLACK) count++;
            if (binarized.GetPixel(y + 1, x + 0) == Colors.BLACK) count++;
            if (binarized.GetPixel(y - 1, x + 0) == Colors.BLACK) count++;
            if (binarized.GetPixel(y + 0, x + 1) == Colors.BLACK) count++;
            if (binarized.GetPixel(y + 0, x - 1) == Colors.BLACK) count++;
            return count >= 3;
        }
        public Coordinate Centroid()
        {
            return Coordinate.Mean(new Coordinate[] { p1, p2, p3, p4 });
        }
        public Coordinate FarestPoint(Coordinate from)
        {
            double maxD = p1.Dist(from);
            Coordinate max = p1;

            if (p2.Dist(from) > maxD)
            {
                max = p2;
                maxD = p2.Dist(from);
            }
            if (p3.Dist(from) > maxD)
            {
                max = p3;
                maxD = p3.Dist(from);
            }
            if (p4.Dist(from) > maxD)
                max = p4;

            return max;
        }
    }
}

using Bitmap;

namespace QRCodes.Reader
{
    class PerspectivCalculator
    {
        //Variables
        private BitMap image;
        private double a;
        private double b;
        private double c;
        private double d;
        private double e;
        private double f;
        private double g;
        private double h;
        
        //Constructeurs
        public PerspectivCalculator(BitMap image, int size, Coordinate p1, Coordinate p2, Coordinate p3, Coordinate p4)
        {
            this.image = image;

            double v1 = p1.Y, v2 = p2.Y, v3 = p3.Y, v4 = p4.Y;
            double u1 = p1.X, u2 = p2.X, u3 = p3.X, u4 = p4.X;

            double du1 = (u3 - u2) * size;
            double du2 = (u1 - u4) * size;
            double du3 = u2 - u3 + u4 - u1;

            double dv1 = (v3 - v2) * size;
            double dv2 = (v1 - v4) * size;
            double dv3 = v2 - v3 + v4 - v1;

            double d = du1 * dv2 - dv1 * du2;
            if (d == 0)
                d = 1;

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
        public bool Get(int y, int x)
        {
            double xx = x + 0.5;
            double yy = y + 0.5;
            double q = g * xx + h * yy + 1;
            int u = (int)((a * xx + b * yy + c) / q);
            int v = (int)((d * xx + e * yy + f) / q);
            return IsBlack(v, u);
        }
        private bool IsBlack(int y, int x)
        {
            int count = 0;
            if (image.GetPixel(y + 0, x + 0) == Colors.BLACK) count++;
            if (image.GetPixel(y + 1, x + 0) == Colors.BLACK) count++;
            if (image.GetPixel(y - 1, x + 0) == Colors.BLACK) count++;
            if (image.GetPixel(y + 0, x + 1) == Colors.BLACK) count++;
            if (image.GetPixel(y + 0, x - 1) == Colors.BLACK) count++;

            image.SetPixel(y + 0, x + 0, Colors.MAGENTA);
            image.SetPixel(y + 1, x + 0, Colors.MAGENTA);
            image.SetPixel(y - 1, x + 0, Colors.MAGENTA);
            image.SetPixel(y + 0, x + 1, Colors.MAGENTA);
            image.SetPixel(y + 0, x - 1, Colors.MAGENTA);
            return count >= 3;
        }
    }
}

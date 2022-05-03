using Bitmap;
using System;

namespace QRCodes.Reader
{
    internal class QRCodeShapeFinder
    {
        //Variables
        private readonly BitMap binarized;
        private readonly Shape s1, s2, s3;
        private Shape f1, f2, f3;
        private Coordinate p1, p2, p3, p4;
        private Shape found = null;

        //Constructeurs
        public QRCodeShapeFinder(BitMap binarized, Shape s1, Shape s2, Shape s3)
        {
            this.binarized = binarized;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
        }

        //Methodes
        public Shape Find()
        {
            if (found != null)
                return found;

            FindF123();
            FindP();
            found = new Shape(binarized, p1, p2, p3, p4);
            FindSize();
            return found;
        }
        private void FindF123()
        {
            Coordinate c1 = s1.Centroid(), c2 = s2.Centroid(), c3 = s3.Centroid();
            double d12 = c1.Dist(c2), d13 = c1.Dist(c3), d23 = c2.Dist(c3);
            if (d12 > d13 && d12 > d23)
            {
                f2 = s3;
                FindF13(s1, s2);
            }
            else if (d13 > d12 && d13 > d23)
            {
                f2 = s2;
                FindF13(s1, s3);
            }
            else if (d23 > d12 && d23 > d13)
            {
                f2 = s1;
                FindF13(s2, s3);
            }
        }
        private void FindF13(Shape r1, Shape r3)
        {
            Coordinate c1 = r1.Centroid(), c3 = r3.Centroid();
            Coordinate c2 = f2.Centroid();
            double u1 = c1.X - c2.X, u2 = c3.X - c2.X;
            double v1 = c1.Y - c1.Y, v2 = c3.Y - c2.Y;
            if (u1*v2 - v1*u2 < 0)
            {
                f1 = r3;
                f3 = r1;
            }
            else
            {
                f1 = r1;
                f3 = r3;
            }
        }
        private void FindP()
        {
            Coordinate mean = Coordinate.Mean(new Coordinate[] { f1.Centroid(), f2.Centroid(), f3.Centroid() });
            
            this.p2 = f2.FarestPoint(mean);
            Coordinate p22 = f2.FarestPoint(p2);
            this.p1 = f1.FarestPoint(p22);
            Coordinate p12 = f1.FarestPoint(p2);
            this.p3 = f3.FarestPoint(p22);
            Coordinate p32 = f3.FarestPoint(p2);

            Coordinate i1 = Intersection(p2, p22, p1, p12);
            Coordinate i2 = Intersection(p2, p22, p3, p32);
            Coordinate i3 = Intersection(p1, p12, p3, p32);

            this.p4 = Coordinate.Mean(new Coordinate[] { i1, i2, i3 });
        }
        private static Coordinate Intersection(Coordinate d1p1, Coordinate d1p2, Coordinate d2p1, Coordinate d2p2)
        {
            double a1 = (d1p1.Y - d1p2.Y) / (d1p2.X - d1p2.X);
            double a2 = (d2p1.Y - d2p2.Y) / (d2p2.X - d2p2.X);
            double b1 = d1p1.Y - a1 * d1p1.X;
            double b2 = d2p1.Y - a2 * d2p1.X;

            if (a1 == a2)
                throw new ArgumentException();

            if (d1p1.X == d1p2.X)
                return new Coordinate(d1p1.X, d2p1.Y + a2 * (d1p2.X - d2p1.X));

            if (d2p1.X == d2p2.X)
                return new Coordinate(d2p1.X, d1p1.Y + a1 * (d2p2.X - d1p1.X));

            double x = (b2 - b1) / (a1 - a2);
            double y = x * a1 + b1;
            return new Coordinate(x, y);
        }
        private void FindSize()
        {
            Coordinate middle = Coordinate.Middle(f1.Centroid(), f3.Centroid());
            Coordinate f11 = f1.FarestPoint(middle);
            Coordinate f12 = f1.FarestPoint(f11);
            Coordinate f31 = f3.FarestPoint(middle);
            Coordinate f32 = f3.FarestPoint(f31);

            double size = FindSize(f11.Dist(f12), f31.Dist(f32), f11.Dist(f31));
            int version = (int) ((size + 17) / 4 + 0.5);
            found.SetEstimatedSize(version * 4 + 17);
        }
        private static double FindSize(double f1Size, double f2Size, double totalSize)
        {
            double f = Math.Max(f1Size / f2Size, f2Size / f1Size) - 1;
            double d = 1 + f * (1.0 - 1.0/Math.PI);
            return totalSize * 14.0 / (f1Size + f2Size) * d;
        }
    }
}

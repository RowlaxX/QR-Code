using System;

namespace QRCodes.Reader
{
    class Coordinate
    {
        //Variables
        public double X { get; private set; }
        public double Y { get; private set; }

        //Constructeurs
        public Coordinate(double y, double x)
        {
            this.X = x;
            this.Y = y;
        }

        //Methodes statiques
        public static Coordinate Mean(Coordinate[] coords)
        {
            Coordinate result = new Coordinate(0, 0);
            foreach (Coordinate coord in coords)
                result.Add(coord);
            result.X /= coords.Length;
            result.Y /= coords.Length;
            return result;
        }

        public static double Dist(Coordinate c1, Coordinate c2)
        {
            double x = c2.X - c1.X;
            double y = c2.Y - c1.Y;
            return Math.Sqrt(x * x + y * y);
        }
        public static double Atan2(Coordinate c1, Coordinate c2)
        {
            double y = c2.Y - c1.Y;
            double x = c2.X - c1.X;
            return Math.Atan2(y, x);
        }
        public static Coordinate Add(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.Y + c2.Y, c1.X + c2.X);
        }
        public static Coordinate Middle(Coordinate c1, Coordinate c2)
        {
            return new Coordinate((c1.Y + c2.Y) / 2, (c1.X + c2.X) / 2);
        }
        //Methodes
        public double Dist(Coordinate another)
        {
            return Dist(this, another);
        }
        public double Atan2(Coordinate another)
        {
            return Atan2(this, another);
        }
        public Coordinate Add(Coordinate another)
        {
            return Add(this, another);
        }
        public Coordinate Add(int y, int x)
        {
            return Add(this, new Coordinate(y, x));
        }
        public override string ToString()
        {
            return "Y=" + Y + "  X=" + X;
        }
    }
}

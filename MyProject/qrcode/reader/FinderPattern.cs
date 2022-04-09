using Bitmap;
using System;

namespace QRCodes.Reader
{
    class FinderPattern
    {
        //Variables
        public Coordinate TopLeft { get; private set; }
        public Coordinate TopRight { get; private set; }
        public Coordinate BottomLeft { get; private set; }
        public Coordinate BottomRight { get; private set; }
        public Coordinate Centroid { get; private set; }
        public double PPM { get; private set; }


        //Constructeurs
        public FinderPattern(BitMap image, Coordinate topLeft, Coordinate bottomLeft, double length)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            this.TopLeft = topLeft ?? throw new ArgumentNullException(nameof(topLeft));
            this.BottomLeft = bottomLeft ?? throw new ArgumentNullException(nameof(bottomLeft));
            this.PPM = length/7.0;

            int diffY = topLeft.Y - bottomLeft.Y;
            int diffX = topLeft.X - bottomLeft.X;

            double module = Math.Sqrt(diffY * diffY + diffX * diffX);
            if (module < length/1.1 || module > length*1.1)
                throw new ApplicationException("This pattern is not correct.");

            double m = diffX / (double)(diffY);

            int ax, ay;
            int black = 0;
            int white = 0;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    ax = (int)(i * m) + j;
                    ay = (int)(j * -m) + i;
                    if (image.GetPixel(bottomLeft.Y + ay, bottomLeft.X + ax) == Colors.WHITE)
                        white++;
                    else
                        black++;
                }

            image.Save("centroid.bmp");
            double blackModuleRatio = 33.0 / 49.0;
            double blackRatio = (double)black / (length * length);

            if (blackRatio < blackModuleRatio / 1.1 || blackRatio > blackModuleRatio * 1.1)
                throw new ApplicationException("Modules are unbalanced.");

            this.BottomRight = LocateBottomRight(image);
            this.TopRight = LocateTopRight(image);
            this.Centroid = Coordinate.Middle(  Coordinate.Middle(TopRight, BottomLeft), 
                                                Coordinate.Middle(TopLeft, BottomRight));
        }
        private Coordinate LocateBottomRight(BitMap image)
        {
            int x1 = BottomLeft.X;
            int y1 = BottomLeft.Y;
            int y;
            for (int i = 0; i < PPM*7; i++)
            {
                x1++;
                for (y = y1 - 2; y < y1 + 2; y++)
                    if (image.GetPixel(y, x1) == Colors.WHITE && image.GetPixel(y+1,x1) == Colors.BLACK)
                    {
                        y1 = y + 1;
                        break;
                    }
                if (y >= y1 + 2)
                {
                    x1--;
                    break;
                }
            }
            return new Coordinate(y1, x1);
        }
        private Coordinate LocateTopRight(BitMap image)
        {
            int x1 = TopLeft.X;
            int y1 = TopLeft.Y;
            int y;
            for (int i = 0; i < PPM * 7; i++)
            {
                x1++;
                for (y = y1 + 1; y >= y1 - 2; y--)
                    if (image.GetPixel(y+1, x1) == Colors.WHITE && image.GetPixel(y, x1) == Colors.BLACK)
                    {
                        y1 = y;
                        break;
                    }
                if (y < y1 - 2)
                {
                    x1--;
                    break;
                }
            }
            return new Coordinate(y1, x1);
        }
    }
}

using System;
using Bitmap;

namespace QRCodes.Reader
{
    
    class QRCodeReader
    {
        //Variables
        private readonly BitMap image = null;
        public QRCode Readed { get; private set; }
        public BitMap Binarized { get { return image; } }

        //Constructeurs
        public QRCodeReader(BitMap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            this.image = Binarize(image);
        }

        //Methodes statiques
        private static BitMap GrayScale(BitMap rawImage)
        {
            BitMap grayScaled = new(rawImage.Height, rawImage.Width);
            byte result;
            Color color;
            for (int i = 0; i < grayScaled.Height; i++)
                for (int j = 0; j < grayScaled.Width; j++)
                {
                    color = rawImage.GetPixel(i, j);
                    result = (byte)((77 * color.R + 151 * color.G + 28 * color.B) / 256);
                    grayScaled.SetPixel(i, j, new Color(result, result, result));
                }

            return grayScaled;
        }
        private static BitMap Binarize(BitMap rawImage)
        {
            BitMap image = GrayScale(rawImage);
            byte gray;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                {
                    gray = image.GetPixel(i, j).R;
                    if (gray > 112)
                        image.SetPixel(i, j, Colors.WHITE);
                    else
                    {
                        image.SetPixel(i, j, Colors.BLACK);
                    }
                }
            return image;
        }

        //Methodes
        public QRCode Read()
        {
            Scanner scanner = new(image);
            while (scanner.HasNext())
                scanner.Next();
            scanner.Locate();
            scanner.Organize();
            
            int size = CalculateSize(scanner);
            Coordinate middle = Coordinate.Middle(scanner.TopRight.Centroid, scanner.BottomLeft.Centroid);

            Coordinate p1 = CalculateP(middle, scanner.BottomLeft);
            Coordinate p2 = CalculateP(middle, scanner.TopLeft);
            Coordinate p3 = CalculateP(middle, scanner.TopRight);
            Coordinate p4 = CalculateP4(p1, p2, p3);

            PerspectivCalculator pc = new(image, size, p1, p2, p3, p4);
            bool[,] mat = new bool[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    mat[i, j] = pc.Get(i, j);

            Readed = QRCode.Read(mat);
            return Readed;
        }
        private Coordinate CalculateP(Coordinate middle, FinderPattern pattern)
        {
            double dtl, dtr, dbl, dbr;
            dtl = middle.Dist(pattern.TopLeft);
            dtr = middle.Dist(pattern.TopRight);
            dbl = middle.Dist(pattern.BottomLeft);
            dbr = middle.Dist(pattern.BottomRight);

            double max = Math.Max(Math.Max(dtl, dtr), Math.Max(dbl, dbr));

            if (max == dtl)
                return pattern.TopLeft;
            else if (max == dtr)
                return pattern.TopRight;
            else if (max == dbl)
                return pattern.BottomLeft;
            else
                return pattern.BottomRight;
        }
        private Coordinate CalculateP4(Coordinate p1, Coordinate p2, Coordinate p3)
        {
            double mY = (p1.Y + p3.Y) / 2.0;
            double mX = (p1.X + p3.X) / 2.0;
            return new((int)(2 * mY) - p2.Y, (int)(2 * mX) - p2.X);
        }
        private int CalculateSize(Scanner scanner)
        {
            FinderPattern tl = scanner.TopLeft;
            FinderPattern bl = scanner.BottomLeft;
            FinderPattern tr = scanner.TopRight;

            double ty = Math.Abs(tr.Centroid.Y - tl.Centroid.Y);
            double tx = Math.Abs(tr.Centroid.X - tl.Centroid.X);
            double ly = Math.Abs(tl.Centroid.Y - bl.Centroid.Y);
            double lx = Math.Abs(tl.Centroid.X - bl.Centroid.X);

            int sty = CalculateSize(ty, tl.PPM, tr.PPM);
            int stx = CalculateSize(tx, tl.PPM, tr.PPM);
            int slx = CalculateSize(lx, tl.PPM, bl.PPM);
            int sly = CalculateSize(ly, tl.PPM, bl.PPM);

            if (sty > stx)
                return (sty + slx) / 2;
            else
                return (stx + sly) / 2;
        }
        private int CalculateSize(double distance, double ppm1, double ppm2)
        {
            int module = 7;
            double traveled = 0;
            double ppm;
            
            while (traveled < distance)
            {
                ppm = (traveled / distance) * (ppm2 - ppm1) + ppm1;
                traveled += ppm;
                module++;
            }

            return module;
        }
        
    }
}

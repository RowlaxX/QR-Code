using System;
using System.Collections.Generic;
using Bitmap;

namespace QRCodes.Reader
{
    
    class QRCodeReader
    {
        //Variables
        private readonly BitMap binarized;

        private FinderPatternFinder finderPatternFinder = null;
        private QRCodeShapeFinder shapeFinder = null;

        //Constructeurs
        public QRCodeReader(BitMap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            this.binarized = Binarize(image);
        }

        //Methodes statiques
        private static BitMap Binarize(BitMap rawImage)
        {
            BitMap grayScaled = new(rawImage.Height, rawImage.Width);
            Color color;
            for (int i = 0; i < grayScaled.Height; i++)
                for (int j = 0; j < grayScaled.Width; j++)
                {
                    color = rawImage.GetPixel(i, j);
                    grayScaled.SetPixel(i, j, ((77 * color.R + 151 * color.G + 28 * color.B) / 256) > 128 ? Colors.WHITE : Colors.BLACK);
                }

            return grayScaled;
        }

        //Methodes
        public QRCode Read()
        {
            Shape shape = FindShape();
            int size = shape.GetEstimatedSize();
            bool[,] mat = new bool[size, size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    mat[y, x] = shape.IsBlack(y, x);

            return QRCode.Read(mat);
        } 
        private List<Shape> FindPattern()
        {
            if (finderPatternFinder == null)
                finderPatternFinder = new FinderPatternFinder(this.binarized);
            return finderPatternFinder.getFinderPatterns();
        }

        private Shape FindShape()
        {
            if (shapeFinder == null)
            {
                List<Shape> patterns = FindPattern();
                shapeFinder = new QRCodeShapeFinder(binarized, patterns[0], patterns[1], patterns[2]);
            }
            return shapeFinder.Find();
        }
    }
}

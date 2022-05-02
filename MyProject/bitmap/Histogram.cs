using System;

namespace Bitmap
{
    class Histogram
    {
        //Variables
        private int[] tab = new int[256];
        
        //Constructeurs
        public Histogram(BitMap image)
        {
            if (image == null)
                throw new ArgumentNullException("image may not be null.");

            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width;j++)
                    tab[image.GetPixel(i,j).Gray()]++;
        }

        //Methodes
        public BitMap ToBitmap()
        {
            BitMap image = new(200, 256);
            double scale = Max(tab);
            int t;

            Color color;
            for (int  x = 0; x < 256; x++)
            {
                t = (int)((tab[x] / scale) * 200.0);
                color = new((byte)(64 + x / 2), 0, 0);

                for (int y = 0; y < t; y++)
                    image.SetPixel(199 - y, x, color);
            }

            return image;
        }
        private static int Max(int[] array)
        {
            int max = int.MinValue;
            foreach (int e in array)
                if (e > max)
                    max = e;
            return max;
        }
    }
}
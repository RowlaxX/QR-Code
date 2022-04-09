using System;

namespace Bitmap
{
    class Histogram
    {
        //Variables
        private int[] R = new int[256];
        private int[] B = new int[256];
        private int[] G = new int[256];

        //Constructeurs
        public Histogram(BitMap image)
        {
            if (image == null)
                throw new ArgumentNullException("image may not be null.");

            Color color;
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width;j++)
                {
                    color = image.GetPixel(i, j);
                    R[color.R]++;
                    G[color.G]++;
                    B[color.B]++;
                }
        }

        //Methodes
        public BitMap ToBitmap()
        {
            BitMap image = new BitMap(200, 256);
            int scale = MaxBlue() + MaxGreen() + MaxRed();

            int tR, tB, tG;
            int maxT;
            for (int  x = 0; x < 256; x++)
            {
                tR = (int)( ((double)R[x] / scale) * 200.0);
                tB = (int)(((double)B[x] / scale) * 200.0);
                tG = (int)(((double)G[x] / scale) * 200.0);
                maxT = Math.Max(tR, Math.Max(tB, tG));

                for (int y = 0;  y < maxT; y++)
                    image.SetPixel(y, x, new Color(
                        (byte)(y < tR ? 0x55 : 0xFF),
                        (byte)(y < tG ? 0x55 : 0xFF),
                        (byte)(y < tB ? 0x55 : 0xFF)
                    ));
            }

            return image;
        }
        public int MaxBlue()
        {
            int max = 0;
            foreach (int e in B)
                if (e > max)
                    max = e;
            return max;
        }
        public int MaxRed()
        {
            int max = 0;
            foreach (int e in R)
                if (e > max)
                    max = e;
            return max;
        }
        public int MaxGreen()
        {
            int max = 0;
            foreach (int e in G)
                if (e > max)
                    max = e;
            return max;
        }
    }
}
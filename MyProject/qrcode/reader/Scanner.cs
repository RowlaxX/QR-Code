using Bitmap;
using System;
using System.Collections.Generic;

namespace QRCodes.Reader
{
    class Scanner
    {
        //Variables
        private readonly BitMap binarized;
        private readonly List<Coordinate> potential = new(0xFF);
        private readonly List<FinderPattern> patterns = new(3);
        private readonly int[] scanned;
        private readonly int width;
        private readonly int maxModuleSize;
        private int line = 0;
        private int scannedItem = 0;
        private bool located = false;

        public FinderPattern TopLeft { get; private set; } = null;
        public FinderPattern TopRight { get; private set; } = null;
        public FinderPattern BottomLeft { get; private set; } = null;

        //Constructeurs
        public Scanner(BitMap binarized)
        {
            this.binarized = binarized ?? throw new ArgumentNullException(nameof(binarized));
            this.width = (int)binarized.Width;
            this.scanned = new int[width / 2];
            this.maxModuleSize = (int)Math.Min(binarized.Height, binarized.Width) / 21;
        }

        //Methodes
        public bool HasNext()
        {
            return line < binarized.Height;
        }
        public void Next()
        {
            scanned[0] = 0;
            scannedItem = Next(this.scanned, line, 0);

            if (scannedItem > 5)
                Check();

            line++;
        }
        private int Next(int[] array, int y, int x)
        {
            Color last = Colors.BLACK, next;
            Color color;
            int s = 0;
            for (int i = x; i < width; i++)
            {
                color = binarized.GetPixel(y, i);
                if (color == last)
                    array[s]++;
                else
                {
                    if (i + 1 == width)
                        array[s]++;
                    else
                    {
                        next = binarized.GetPixel(y, i + 1);
                        if (color == next)
                        {
                            s++;
                            if (s >= array.Length)
                                break;
                            array[s] = 1;
                            last = color;
                        }
                        else
                            array[s]++;
                    }
                }
            }
            return s+1;
        }
        private void Check()
        {
            int index = 0;
            for (int i = 0; i < scannedItem - 5; i++)
            {
                if (i % 2 == 0)
                    Check(index, i);

                index += scanned[i];
            }
        }
        private void Check(int x, int index)
        {
            for (int i = 0; i < 5; i++)
                if (scanned[index + i] > (i == 2 ? 3 : 1) * maxModuleSize)
                    return;

            int w1 = scanned[index + 0];
            int w2 = scanned[index + 1];
            int w3 = scanned[index + 2];
            int w4 = scanned[index + 3];
            int w5 = scanned[index + 4];
            
            //Finder pattern
            int W = w1 + w2 + w3 + w4 + w5;
            double w = W / 7.0;
            double down = w - 1.5;
            double up = w + 2.0;

            if (w1 < down || w1 > up)
                return;
            if (w2 < down || w2 > up)
                return;
            if (w4 < down || w4 > up)
                return;
            if (w5 < down || w5 > up)
                return;
            if (w3 < 3 * w - 2 || w3 > 3 * w + 2)
                return;
            if (w3 < Math.Max(w1 + w2, w4 + w5))
                return;

            potential.Add(new Coordinate(line, x));
        }
        public void Locate()
        {
            if (HasNext())
                throw new ApplicationException("You must scan all the image first.");
            if (located)
                return;

            MergeAndVerify();
            if (patterns.Count != 3)
                throw new ApplicationException("Unable to find 3 patterns finder. Found=" + patterns.Count);

            located = true;
        }
        private void MergeAndVerify()
        {
            List<Coordinate> list = new(10);
            Coordinate c, d;

            for (int i = potential.Count - 1; i >= 0; i--)
            {
                c = potential[i];
                list.Clear();
                list.Add(c);
                potential.RemoveAt(i);

                for (int j = i - 1; j >= 0; j--)
                {
                    d = potential[j];
                    if (d.Y == c.Y)
                        continue;
                    if (d.X < c.X - 2 || d.X > c.X + 2)
                        continue;
                    if (d.Y + 2 < c.Y)
                        break;
                    potential.RemoveAt(j);
                    i--;
                    list.Add(d);
                    c = d;
                }

                if (list.Count >= 2)
                    Verify(list);
            }
        }
        private void Verify(List<Coordinate> list)
        {
            int[] t = new int[5];
            double length = 0;
            foreach (Coordinate c in list)
            {
                t[0] = 0;
                Next(t, c.Y, c.X);
                for (int i = 0; i < 5; i++)
                    length += t[i];
            }
            length /= list.Count;

            int x;

            Coordinate top = list[^1];
            int x1 = top.X;
            int y1 = top.Y;
            for (int i = 0; i < length; i++)
            {
                y1++;
                for (x = x1 - 2; x < x1 + 2; x++)
                    if (binarized.GetPixel(y1, x) == Colors.WHITE && binarized.GetPixel(y1, x + 1) == Colors.BLACK)
                    {
                        x1 = x + 1;
                        break;
                    }

                if (x >= x1 + 2)
                {
                    y1--;
                    break;
                }
            }

            Coordinate bottom = list[0];
            int x2 = bottom.X;
            int y2 = bottom.Y;
            for (int i = 0; i < length; i++)
            {
                y2--;
                for (x = x2 - 2; x < x2 + 2; x++)
                    if (binarized.GetPixel(y2, x) == Colors.WHITE && binarized.GetPixel(y2, x + 1) == Colors.BLACK)
                    {
                        x2 = x + 1;
                        break;
                    }

                if (x >= x2 + 2)
                {
                    y2++;
                    break;
                }
            }

            try
            {
                patterns.Add(new(binarized, new(y1, x1), new(y2, x2), length));
            }
            catch (ApplicationException) {}
        }
        public void Organize()
        {
            if (patterns.Count != 3)
                throw new ApplicationException("You first need to locate 3 pattern finder.");

            FinderPattern f1 = patterns[0];
            FinderPattern f2 = patterns[1];
            FinderPattern f3 = patterns[2];

            double d1 = f1.Centroid.Dist(f2.Centroid);
            double d2 = f1.Centroid.Dist(f3.Centroid);
            double d3 = f2.Centroid.Dist(f3.Centroid);

            if (d3 > d2 && d3 > d1)
            {
                TopLeft = f1;
                f1 = f3;
            }
            if (d2 > d3 && d2 > d1)
            {
                TopLeft = f2;
                f2 = f3;
            }
            if (d1 > d2 && d1 > d3)
                TopLeft = f3;

            Coordinate fp3 = f1.Centroid;
            Coordinate fp2 = f2.Centroid;
            Coordinate fp1 = TopLeft.Centroid;

            if ( (fp3.X - fp2.X)*(fp1.Y - fp2.Y) < (fp3.Y - fp2.Y)*(fp1.X - fp2.X))
            {
                BottomLeft = f1;
                TopRight = f2;
            }
            else
            {
                BottomLeft = f2;
                TopRight = f1;
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Bitmap
{
    class Fractals
    {
        //Variables
        private static readonly uint MandelbrotPaletteLength = 2048;
        private static readonly SortedDictionary<double, Color> MandelbrotGradients = new()
        {
            [0.0]=new Color(0,7,100),
            [0.16]=new Color(32, 107, 203),
            [0.42]=new Color(217, 235, 235),
            [0.6425]=new Color(235, 150, 0),
            [0.8575]=new Color(0,2,0)
        };
        private static Color[] MandelbrotPalette = null;

        //Methodes
        private static void InitMandelbrotPalette()
        {
            if (MandelbrotPalette != null)
                return;

            int n = MandelbrotGradients.Count;

            MandelbrotPalette = new Color[MandelbrotPaletteLength];
            List<double> gradientX = new(n);
            List<Color> gradientColor = new(n);

            foreach(KeyValuePair<double, Color> entry in MandelbrotGradients)
            {
                gradientX.Add(entry.Key);
                gradientColor.Add(entry.Value);
            }

            double fr, fg, fb;
            double diff;
            int start, end;
            for (int  i = 0; i < MandelbrotGradients.Count; i++)
            {
                diff = i + 1 == n ? 1 - gradientX[i] : gradientX[(i + 1) % n] - gradientX[i];
                fr = ((int)gradientColor[(i + 1) % n].R - (int)gradientColor[i].R) / diff;
                fg = ((int)gradientColor[(i + 1) % n].G - (int)gradientColor[i].G) / diff;
                fb = ((int)gradientColor[(i + 1) % n].B - (int)gradientColor[i].B) / diff;
                start = (int)(gradientX[i] * MandelbrotPaletteLength);
                end = i == n - 1 ? (int)MandelbrotPaletteLength : (int)(gradientX[i + 1] * MandelbrotPaletteLength);

                for (int j = start; j < end; j++)
                    MandelbrotPalette[j] = new Color(
                        (byte)(fr * ((double)(j - start) / MandelbrotPaletteLength) + gradientColor[i].R),
                        (byte)(fg * ((double)(j - start) / MandelbrotPaletteLength) + gradientColor[i].G),
                        (byte)(fb * ((double)(j - start) / MandelbrotPaletteLength) + gradientColor[i].B));
            }

            BitMap palette = new(100, (int)MandelbrotPaletteLength);
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < MandelbrotPaletteLength; j++)
                    palette.SetPixel(i, j, MandelbrotPalette[j]);
            palette.Save("palette.bmp");
        }
        public static BitMap Mandelbrot(int height, int width, uint maxIteration)
        {
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            InitMandelbrotPalette();

            BitMap fractal = new(height, width);
            int k;
            double x0, y0, xTemp, x, y, smoothed;
            Color color;
            int colorI;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    fractal.SetPixel(i, j, Colors.BLACK);
                    y0 = ((double)i / (double)height) * 3.0 - 1.5;
                    x0 = ((double)j / (double)width) * 3.0 - 2.0;
                    x = 0;
                    y = 0;

                    for (k = 0; k < maxIteration; k++)
                    {
                        if (x * x + y * y > 4)//Module > 2
                            break;
                        xTemp = x * x - y * y + x0;
                        y = 2 * x * y + y0;
                        x = xTemp;
                    }

                    

                    smoothed = Math.Log2(Math.Log2(x * x + y * y) / 2);  // log_2(log_2(|p|))
                    colorI = (int)(Math.Sqrt(k + 10 - smoothed) * 256) % (int)MandelbrotPaletteLength;
                    color = MandelbrotPalette[colorI];
                    
                    fractal.SetPixel(i, j, color);
                }

            return fractal;
        }
        public static BitMap Mandelbrot(int heigth, int width)
        {
            return Mandelbrot(heigth, width, 1000);
        }
    }
}
using System;

namespace Bitmap
{
    class Colors
    {
        //Couleurs usuelles
        public static Color BLACK = new Color(0, 0, 0);
        public static Color WHITE = new Color(255, 255, 255);
        public static Color GRAY = new Color(0x88, 0x88, 0x88);
        public static Color DARK_GRAY = new Color(0x44, 0x44, 0x44);
        public static Color LIGHT_GRAY = new Color(0xBB, 0xBB, 0xBB);
        public static Color RED = new Color(255, 0, 0);
        public static Color BLUE = new Color(0, 0, 255);
        public static Color GREEN = new Color(0, 255, 0);
        public static Color MAGENTA = new Color(255, 0, 255);

        //Mean class
        public class Mean
        {
            private double R = 0, G = 0, B = 0;
            private double countR = 0, countG = 0, countB = 0;
            public bool DoBlue { get; set; } = true;
            public bool DoRed { get; set; } = true;
            public bool DoGreen { get; set; } = true;

            public Mean() { }

            public void PutColor(Color color, double coeff)
            {
                if (color == null)
                    color = Colors.WHITE;
                if (coeff == 0)
                    return;
                
                if (DoRed)
                {
                    this.R += color.R * coeff;
                    this.countR += coeff;
                }
                if (DoGreen)
                {
                    this.G += color.G * coeff;
                    this.countG += coeff;
                }
                if (DoBlue)
                {
                    this.B += color.B * coeff;
                    this.countB += coeff;
                }
            }

            public void PutColor(Color color)
            {
                PutColor(color, 1);
            }

            private static byte Get(double factor, double sum)
            {
                if (factor < 0)
                {
                    if (sum < 0)
                        return Get(-factor, -sum);
                    else if (sum >= 0)
                        return 0;
                }
                if (factor == 0)
                {
                    if (sum < 0)
                        return (byte)(-sum % 256);
                    else
                        return (byte)(sum % 256);
                }
                else
                {
                    if (sum < 0)
                        return 0;
                    else if (sum / factor > 255)
                        return 255;
                    else
                        return (byte)(sum / factor);
                }
                    
            }

            public Color Build()
            {
                byte r = Get(countR, R);
                byte g = Get(countG, G);
                byte b = Get(countB, B);
                return new Color(r, g, b);
            }
        }
        public static Mean[,] NewMeans(int height, int width)
        {
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height may be greater than 0.");
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width may be greater than 0.");

            Colors.Mean[,] mat = new Colors.Mean[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    mat[i, j] = new Colors.Mean();
            return mat;
        }
        public static Color[,] Build(Colors.Mean[,] means)
        {
            if (means == null)
                throw new ArgumentNullException("means may not be null.");

            int height = means.GetLength(0);
            int width = means.GetLength(1);

            Color[,] colors = new Color[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    colors[i, j] = means[i, j].Build();

            return colors;
        }

        //Methodes
        public static Color Graynify(Color color)
        {
            byte mean = (byte)((color.G + color.B + color.R) / 3);
            return new Color(mean, mean, mean);
        }
        public static Color Blackify(Color color)
        {
            byte mean = (byte)((color.G + color.B + color.R) / 3);
            if (mean < 128)
                return BLACK;
            else
                return WHITE;
        }
        public static Color Inverse(Color color)
        {
            return new Color( (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
        }
    }
}

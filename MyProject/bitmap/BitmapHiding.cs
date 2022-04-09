using System;

namespace Bitmap
{
    class BitmapHiding
    {
        //Methodes
        public static BitMap Hide(BitMap source, BitMap toHide)
        {
            if (source == null)
                throw new ArgumentNullException("source may not be null.");
            if (toHide == null)
                throw new ArgumentNullException("toHide may not be null.");

            int height = (int)source.Height;
            int width = (int)source.Width;

            if (source.Height != toHide.Height)
                throw new ArgumentException("both image must have the same height.");
            if (source.Width != toHide.Width)
                throw new ArgumentException("both image must have the same width.");

            BitMap result = new BitMap(height, width);
            Color sourceColor, toHideColor;
            byte r, g, b;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    sourceColor = source.GetPixel(i, j);
                    toHideColor = toHide.GetPixel(i, j);
                    r = (byte)((sourceColor.R / 16) * 16 + toHideColor.R / 16);
                    g = (byte)((sourceColor.G / 16) * 16 + toHideColor.G / 16);
                    b = (byte)((sourceColor.B / 16) * 16 + toHideColor.B / 16);
                    result.SetPixel(i, j, new Color(r, g, b));
                }

            return result;
        }
        public static BitMap Recover(BitMap source)
        {
            if (source == null)
                throw new NullReferenceException("source may not be null.");

            int height = (int)source.Height;
            int width = (int)source.Width;
            BitMap newImage = new BitMap(height, width);

            Color color;
            byte r, g, b;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    color = source.GetPixel(i, j);
                    r = (byte)((color.R % 16) * 16);
                    g = (byte)((color.G % 16) * 16);
                    b = (byte)((color.B % 16) * 16);
                    newImage.SetPixel(i, j, new Color(r, g, b));
                }

            return newImage;
        }
    }
}


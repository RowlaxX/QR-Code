using System;
using System.IO;

namespace Bitmap
{
    class BitMap
    {
        //Enums
        public enum Directions { Horizontal, Vertical }

        //Attributs
        private readonly Color[,] colors;
        public Header Header { get; private set; }
        public Informations Information { get; private set; }
        public uint Width { get { return Information.Width; } }
        public uint Height { get { return Information.Height; } }

        //Constructeurs
        public BitMap(int height, int width)
        {
            this.colors = new Color[height, width];
            this.Header = new Header(height, width);
            this.Information = new Informations(height, width);
        }
        public BitMap(uint height, uint width) : this((int)height, (int)width) { }
        public BitMap(BitMap original, int newHeight, int newWidth) : this(original, newHeight, newWidth, false) { }
        public BitMap(BitMap original, double heightScaleFactor, double widthScaleFactor) : this(
                original,
                (int)(original.Height * heightScaleFactor),
                (int)(original.Width * widthScaleFactor),
                true) { }
        public BitMap(BitMap original, double scaleFactor) : this(original, scaleFactor, scaleFactor) { }
        public BitMap(BitMap original, int newHeight, int newWidth, bool scale)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));
            if (newWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(newWidth));
            if (newHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(newHeight));

            Header = new Header(newHeight, newWidth);
            Information = new Informations(newHeight, newWidth);
            colors = new Color[newHeight, newWidth];

            int minHeight = Math.Min((int)original.Height, newHeight);
            int minWidth = Math.Min((int)original.Width, newWidth);

            if (!scale)
                for (int i = 0; i < minHeight; i++)
                    for (int j = 0; j < minWidth; j++)
                        colors[i, j] = original.colors[i, j];
            else
                colors = Scale(original.colors, newHeight, newWidth);
        }
        public BitMap(BitMap original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            this.Header = original.Header.Clone();
            this.Information = original.Information.Clone();
            this.colors = (Color[,])original.colors.Clone();
        }
        public BitMap(string file) : this(new FileStream(file, FileMode.Open)) {}
        public BitMap(Stream sr)
        {
            if (sr == null)
                throw new ArgumentNullException(nameof(sr));

            using (sr)
            {
                this.Header = new Header(sr);
                this.Information = new Informations(sr);
                this.colors = new Color[this.Height, this.Width];
                LoadPixels(sr);
            }
        }

        //Methodes statiques
        private static Color[,] Scale(Color[,] original, int newHeight, int newWidth)
        {
            int height = original.GetLength(0);
            int width = original.GetLength(1);

            if (height == newHeight && width == newWidth)
                return (Color[,])original.Clone();

            Color[,] scaled = new Color[newHeight, newWidth];
            double di = height / (double)newHeight;
            double dj = width / (double)newWidth;

            for (int i = 0; i < newHeight; i++)
                for (int j = 0; j < newWidth; j++)
                    scaled[i, j] = original[(int)(i * di), (int)(j * dj)];//Not need to clone since Color is unmodifiable

            return scaled;
        }
        
        //Methodes
        private void LoadPixels(Stream stream)
        {
            byte[] rgb = new byte[3];

            for (int i = (int)this.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    if (stream.Read(rgb, 0, 3) != 3)
                        throw new ApplicationException("The file is missing pixels data at y=" + i + " x=" + j + ". Make sure that the image is not compressed.");

                    this.colors[i, j] = new Color((byte)rgb[2], (byte)rgb[1], (byte)rgb[0]);
                }

                //Bourrage
                for (int j = 0; j < Information.EndPadding; j++)
                    if (stream.ReadByte() != 0x00)
                        throw new ApplicationException("Illegal padding at the end of line " + i);
            }       
        }
        public Color GetPixel(int y, int x)
        {
            if (colors[y, x] == null)
                return Colors.WHITE;
            return colors[y, x];
        }
        public void SetPixel(int y, int x, Color color)
        {
            colors[y, x] = color;
        }
        public bool SetPixels(int y, int x, BitMap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (image == this && y == 0 && x == 0)
                return true;
            if (x + image.Width < 0 || x >= Width)
                return false;
            if (y + image.Height < 0 || y >= Height)
                return false;

            int maxI = (int)(y + image.Height > this.Height ? this.Height : image.Height);
            int maxJ = (int)(x + image.Width > this.Width ? this.Width : image.Width);



            for (int i = 0; i < maxI; i++)
                for (int j = 0; j < maxJ; j++)
                    try
                    {

                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }

            return !(x < 0 ||
                     y < 0 ||
                     maxI != this.Height ||
                     maxJ != this.Width);
        }
        public BitMap Clone()
        {
            return new BitMap(this);
        }
        public void Save(string file)
        {
            Save(new FileStream(file, FileMode.Create));
        }
        public void Save(Stream stream)
        {
            Save(stream, true);
        }
        public void Save(Stream stream, bool close)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            try
            {
                stream = new BufferedStream(stream, 8192);
                stream.Write(Header.ToByteArray());
                stream.Write(Information.ToByteArray());

                for (int i = (int)this.Height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < this.Width; j++)
                        GetPixel(i, j).WriteTo(stream);

                    //BOURAGE
                    for (int j = 0; j < Information.EndPadding; j++)
                        stream.WriteByte(0x00);
                }
            }
            finally
            {
                stream.Flush();
                if (close)
                    stream.Close();
            }
        }
        public BitMap Extract(int y1, int x1, int y2, int x2)
        {
            int height = y2 - x1;
            int width = x2 - x1;
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height),"illegal height");
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "illegal width");

            BitMap image = new(height, width);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    image.SetPixel(i, j, this.GetPixel(i + y1, j + x1));

            return image;
        }
        public BitMap Resize(int newHeight, int newWidth)
        {
            return new BitMap(this, newHeight, newWidth, false);
        }
        public BitMap Extends(int addHeight, int addWidth)
        {
            if (addHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(addHeight));
            if (addWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(addWidth));

            return new BitMap(this, (int)(Height + addHeight), (int)(Width + addWidth), false);
        }
        public BitMap Trim(Directions direction, int trim)
        {
            if (direction == Directions.Vertical)
                return Trim(trim, 0);
            return Trim(0, trim);
        }        
        public BitMap Trim(int removeHeight, int removeWidth)
        {
            if (removeHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(removeHeight));
            if (removeWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(removeWidth));

            return new BitMap(this, (int)(Height - removeWidth), (int)(Width - removeWidth), false);
        }
        public BitMap Extends(Directions direction, int add)
        {
            if (direction == Directions.Vertical)
                return Extends(add, 0);
            return Extends(0, add);
        }
        public BitMap Scale(double factor)
        {
            return new BitMap(this, factor);
        }
        public BitMap Scale(double heightFactor, double widthFactor)
        {
            return new BitMap(this, heightFactor, widthFactor);
        }
        public BitMap Scale(int newHeight, int newWidth)
        {
            return new BitMap(this, newHeight, newWidth, true);
        }
        public BitMap Mirror(Directions direction)
        {
            BitMap inversed = new( (int)Height, (int)Width);

            if (direction == Directions.Horizontal)
                for (int i = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++)
                        inversed.colors[i, j] = this.colors[i, this.Width - 1 - j];
            else
                for (int i = 0; i < this.Height; i++)
                    for (int j = 0; j < this.Width; j++)
                        inversed.colors[i, j] = this.colors[this.Height - 1 - i, j];
            
            return inversed;
        }
        public BitMap Blackify()
        {
            BitMap copy = Clone();

            for (int i = 0; i < this.Height; i++)
                for (int j = 0; j < this.Width; j++)
                    copy.colors[i, j] = Colors.Blackify(copy.colors[i, j]);

            return copy;
        }
        public BitMap InverseColors()
        {
            BitMap copy = Clone();

            for (int i = 0; i < this.Height; i++)
                for (int j = 0; j < this.Width; j++)
                    copy.colors[i, j] = Colors.Inverse(copy.colors[i, j]);

            return copy;
        }
        public BitMap Graynify()
        {
            BitMap copy = Clone();

            for (int i = 0; i < this.Height; i++)
                for (int j = 0; j < this.Width; j++)
                    copy.colors[i, j] = Colors.Graynify(copy.colors[i, j]);

            return copy;
        }
        public BitMap Rotate(double degree)
        {
            return Rotate(degree, false);
        }
        public BitMap Rotate(double degree, bool resize)
        {
            //Prerotating
            double radian = (degree * Math.PI) / 180;
            int destHeight = (int)this.Height;
            int destWidth = (int)this.Width;
            double midHeight = destHeight / 2.0;
            double midWidth = destWidth / 2.0;
            double midDestHeight = midHeight;
            double midDestWidth = midWidth;
            double maxModule = Math.Sqrt(midHeight * midHeight + midWidth * midWidth);

            if (resize)
            {
                double t = Math.Atan2(midHeight, midWidth);
                double c0 = Math.Cos(t + radian);
                double c3 = Math.Cos(-t + radian);
                double s0 = Math.Sin(t + radian);
                double s3 = Math.Sin(-t + radian);
                destHeight = (int)(2.0 * maxModule * Math.Max(Math.Abs(s0), Math.Abs(s3)));
                destWidth = (int)(2.0 * maxModule * Math.Max(Math.Abs(c0), Math.Abs(c3)));
                midDestWidth = destWidth / 2.0;
                midDestHeight = destHeight / 2.0;
            }

            BitMap image = new(destHeight, destWidth);

            //Rotating
            double module;
            double angle;
            double sourceX, sourceY;

            double dx, dy;
            for (int y = 0; y < destHeight ; y++)
                for (int x = 0; x < destWidth; x++)
                {
                    dx = x - midDestWidth;
                    dy = y - midDestHeight;
                    module = Math.Sqrt(dx * dx + dy * dy);

                    if (module > maxModule)//Optimisation
                        continue;

                    angle = Math.Atan2(dy, dx) + radian;
                    sourceX = Math.Cos(angle) * module;
                    sourceY = Math.Sin(angle) * module;

                    if (midHeight + sourceY >= 0 && midHeight + sourceY < Height && midWidth + sourceX >= 0 && midWidth + sourceX < Width)
                        image.colors[y, x] = this.colors[(int)(midHeight + sourceY), (int)(midWidth + sourceX)];
                }

            return image;
        }
        public BitMap ApplyKernel(Kernel kernel, KernelSettings settings)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            BitMap copy = this.Clone();
            
            for (int i = 0; i < this.Height; i++)
                for (int j = 0; j < this.Width; j++)
                    ApplyKernel(kernel, settings, copy, i, j);

            return copy;
        }
        public BitMap ApplyKernel(Kernel kernel)
        {
            return ApplyKernel(kernel, new KernelSettings());
        }
        private void ApplyKernel(Kernel kernel, KernelSettings settings, BitMap destination, int y, int x)
        {
            Colors.Mean mean = new();
            mean.PutColor(this.colors[y, x], kernel.Get(0, 0));
            mean.DoBlue = settings.BlueChannel;
            mean.DoGreen = settings.GreenChannel;
            mean.DoRed = settings.RedChannel;

            int from = kernel.From;
            int to = kernel.To;
            double f = 0;
            for (int i = from; i <= to; i++)
                for (int j = from; j <= to; j++)
                    try
                    {
                        if (i == 0 && j == 0)
                            continue;

                        f = kernel.Get(i, j);
                        if (f == 0)
                            continue;

                        mean.PutColor(this.colors[y + i, x + j], f);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        if (settings.Border == KernelSettings.Borders.Extend)
                            continue;
                        else if (settings.Border == KernelSettings.Borders.Wrap)
                        {
                            int y2 = y + i;
                            int x2 = x + j;

                            if (y2 < 0)
                                y2 = (int)this.Height - 1;
                            else if (y2 >= this.Height)
                                y2 = i - 1;

                            if (x2 < 0)
                                x2 = (int)this.Width - 1;
                            else if (x2 >= this.Width)
                                x2 = j - 1;

                            mean.PutColor(this.colors[y2, x2], f);
                        }
                        else if (settings.Border == KernelSettings.Borders.Crop)
                            return;
                    }

            destination.colors[y, x] = mean.Build();
        }
        public BitMap Sharpen()
        {
            return ApplyKernel(Kernels.Sharpen);
        }
        public BitMap Blur()
        {
            return ApplyKernel(Kernels.Blur);
        }
        public BitMap EdgeEnhance()
        {
            return ApplyKernel(Kernels.EdgeEnhance);
        }
        public BitMap EdgeDetect()
        {
            return ApplyKernel(Kernels.EdgeDetect);
        }
        public BitMap Emboss()
        {
            return ApplyKernel(Kernels.Emboss);
        }
        public Histogram CreateHistogram()
        {
            return new Histogram(this);
        }
    }
}
using MyProject;
using System;
using System.IO;

namespace Bitmap
{
    class Informations
    {
        //Constantes
        public const int SIZE = 40;

        //Variables
        public uint HeaderSize { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public ushort ColorPlanesCount { get; private set; }
        public ushort BitPerPixel { get; private set; }
        public uint Compression { get; private set; }
        public uint ImageSize { get; private set; }
        public uint HorizontalResolution { get; private set; }
        public uint VerticalResolution { get; private set; }
        public uint ColorInPalette { get; private set; }
        public uint ImportantColors { get; private set; }
        public uint EndPadding { get
            {
                return (4 - (Width * 3) % 4) % 4;
            }
        }

        //Constructeurs
        public Informations(int heigth, int width)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (heigth <= 0)
                throw new ArgumentOutOfRangeException(nameof(heigth));

            this.HeaderSize = 40;
            this.Width = (uint)width;
            this.Height = (uint)heigth;
            this.ColorPlanesCount = 1;
            this.BitPerPixel = 24;
            this.Compression = 0;

            uint bytePerLine = (uint)width * 3 + EndPadding;

            this.ImageSize = bytePerLine * (uint)heigth;
            this.HorizontalResolution = 0;
            this.VerticalResolution = 0;
            this.ColorInPalette = 0;
            this.ImportantColors = 0;
        }
        public Informations(byte[] bytes)
        {
            Init(bytes);
        }
        public Informations(Stream sr)
        {
            byte[] bytes = new byte[SIZE];
            sr.Read(bytes, 0, SIZE);
            Init(bytes);
        }

        //Methodes
        private void Init(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length != SIZE)
                throw new ArgumentException("bytes must have a length of " + SIZE);

            this.HeaderSize = Utils.ReadUIntEndianness(0, bytes);
            this.Width = Utils.ReadUIntEndianness(4, bytes);
            this.Height = Utils.ReadUIntEndianness(8, bytes);
            this.ColorPlanesCount = Utils.ReadUShortEndianness(12, bytes);
            this.BitPerPixel = Utils.ReadUShortEndianness(14, bytes);
            this.Compression = Utils.ReadUIntEndianness(16, bytes);
            this.ImageSize = Utils.ReadUIntEndianness(20, bytes);
            this.HorizontalResolution = Utils.ReadUIntEndianness(24, bytes);
            this.VerticalResolution = Utils.ReadUIntEndianness(28, bytes);
            this.ColorInPalette = Utils.ReadUIntEndianness(32, bytes);
            this.ImportantColors = Utils.ReadUIntEndianness(36, bytes);
        }
        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[SIZE];
            Utils.WriteEndianness(HeaderSize, 0, bytes);
            Utils.WriteEndianness(Width, 4, bytes);
            Utils.WriteEndianness(Height, 8, bytes);
            Utils.WriteEndianness(ColorPlanesCount, 12, bytes);
            Utils.WriteEndianness(BitPerPixel, 14, bytes);
            Utils.WriteEndianness(Compression, 16, bytes);
            Utils.WriteEndianness(ImageSize, 20, bytes);
            Utils.WriteEndianness(HorizontalResolution, 24, bytes);
            Utils.WriteEndianness(VerticalResolution, 28, bytes);
            Utils.WriteEndianness(ColorInPalette, 32, bytes);
            Utils.WriteEndianness(ImportantColors, 36, bytes);
            return bytes;
        }
        public Informations Clone()
        {
            return new Informations(ToByteArray());
        }
    }
}

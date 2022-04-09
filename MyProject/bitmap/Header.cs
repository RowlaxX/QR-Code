using MyProject;
using System;
using System.IO;

namespace Bitmap
{
    class Header
    {
        //Constantes
        public const int SIZE = 14;

        //Variables
        public ushort HeaderField { get; private set; }
        public uint Size { get; private set; }
        public ushort Reserved1 { get; private set; }
        public ushort Reserved2 { get; private set; }
        public uint Offset { get; private set; }

        //Constructeurs
        public Header(int height, int width)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("Width must be greater than 0.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("Heigth must be greater than 0.");

            this.HeaderField = 0x4D42; //"BM"
            this.Reserved1 = 0x0000;
            this.Reserved2 = 0x0000;
            this.Offset = SIZE + Informations.SIZE;

            int bytePerLine = width * 3;
            if (bytePerLine % 4 != 0)
                bytePerLine += 4 - bytePerLine % 4;

            this.Size = (uint)(bytePerLine * height + Offset);
        }
        public Header(Stream sr)
        {
            byte[] bytes = new byte[SIZE];
            sr.Read(bytes, 0, SIZE);
            Init(bytes);
        }
        public Header(byte[] bytes)
        {
            Init(bytes);
        }

        //Methodes
        private void Init(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes may not be null.");
            if (bytes.Length != SIZE)
                throw new ArgumentException("bytes must have a length of " + SIZE);

            this.HeaderField = Utils.ReadUShortEndianness(0, bytes);
            this.Size = Utils.ReadUIntEndianness(2, bytes);
            this.Reserved1 = Utils.ReadUShortEndianness(6, bytes);
            this.Reserved2 = Utils.ReadUShortEndianness(8, bytes);
            this.Offset = Utils.ReadUIntEndianness(10, bytes);
        }
        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[SIZE];
            Utils.WriteEndianness(HeaderField, 0x00, bytes);
            Utils.WriteEndianness(Size, 0x02, bytes);
            Utils.WriteEndianness(Reserved1, 0x06, bytes);
            Utils.WriteEndianness(Reserved2, 0x08, bytes);
            Utils.WriteEndianness(Offset, 0x0A, bytes);
            return bytes;
        }
        public Header Clone()
        {
            return new Header(ToByteArray());
        }
    }
}

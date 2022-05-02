using System.IO;

namespace Bitmap
{
    class Color
    {
        //Variables
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }

        //Constructeurs
        public Color(byte red, byte green, byte blue)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
        }

        //Methodes
        public void WriteTo(Stream sr)
        {
            sr.WriteByte(B);
            sr.WriteByte(G);
            sr.WriteByte(R);
        }
        public Color Clone()
        {
            return new Color(R, G, B);
        }
        public byte Gray()
        {
            return (byte)((R + G + B) / 3);
        }
    }
}

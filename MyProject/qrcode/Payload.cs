namespace QRCodes
{
    abstract class Payload
    {
        //Variables
        protected readonly bool[] data;
        public bool[] Data { get { return (bool[])data.Clone(); } }
        public int BitCount { get { return data.Length; } }
        public Encoding.Types Encoding { get; private set; }
        public abstract int Length { get; } 
        public abstract string Content { get; }

        //Constructeurs
        protected Payload(Encoding.Types encoding, int bitCount)
        {
            this.Encoding = encoding;
            this.data = new bool[bitCount];
        }

        //Methodes
        public bool GetBit(int index)
        {
            return data[index];
        }
    }
}
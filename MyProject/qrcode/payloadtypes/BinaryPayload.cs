using QRCodes;
using System;
using MyProject;

namespace QRCodes.PayloadTypes
{
    class BinaryPayload : Payload
    {
        //Methodes statiques
        private static bool IsValid(char c)
        {
            return c >= 0x00 && c <= 0xFF;
        }
        public static bool IsValid(string s)
        {
            foreach (char c in s)
                if (!IsValid(c))
                    return false;
            return true;
        }
        public static int CalculateBitCount(int length)
        {
            return length * 8;
        }
        public static int CalculateLength(int bitCount)
        {
            if (bitCount % 8 != 0)
                throw new ArgumentException("illegal bitCount", nameof(bitCount));
            return bitCount / 8;
        }
        public static BinaryPayload Encode(string s)
        {
            if (!IsValid(s))
                throw new ApplicationException("invalid ascii string.");

            return From(System.Text.Encoding.Latin1.GetBytes(s));
        }
        public static BinaryPayload From(byte[] bytes)
        {
            BinaryPayload payload = new(bytes.Length * 8);
            payload.bytes = (byte[])bytes.Clone();

            for (int i = 0; i < bytes.Length; i++)
                Utils.WriteEndiannessBits(bytes[i], payload.data, i * 8, 8);

            return payload;
        }
        public static BinaryPayload Decode(bool[] message, int index, int length)
        {
            int bitCount = CalculateBitCount(length);
            BinaryPayload payload = new(bitCount);
            for (int i = 0; i < bitCount; i++)
                payload.data[i] = message[index + i];

            payload.bytes = new byte[length];
            for (int i = 0; i < length; i++)
                payload.bytes[i] = (byte)Utils.ReadEndiannessBits(payload.data, i * 8, 8);

            return payload;
        }

        //Attributs
        public override int Length { get { return bytes.Length; } }
        public byte[] Bytes { get { return (byte[])bytes.Clone(); } }
        public override string Content { get { return System.Text.Encoding.Latin1.GetString(bytes); } }

        private byte[] bytes;

        //Constructeurs
        private BinaryPayload(int bitCount) : base(QRCodes.Encoding.Types.Binary, bitCount) { }
    }
}

using MyProject;
using System;
using System.Text;

namespace QRCodes.PayloadTypes
{
    class KanjiPayload : Payload
    {
        //Methodes statiques
        private static bool IsValid(char c)
        {
            return (c >= 0x8140 && c <= 0x9FFC) || (c >= 0xE040 && c <= 0xEBBF);
        }
        public static bool IsValid(string s)
        {
            foreach (char c in s)
                if (!IsValid(c))
                    return false;
            return true;
        }
        private static int ToInt(char c)
        {
            if (c >= 0x8140 && c <= 0x9FFC)
                c = (char)(c - 0x0897);
            else
                c = (char)(c - 0xC140);

            return (c >> 8) * 0xC0 + (c % 0xFF);
        }
        private static char FromInt(int i)
        {
            int m = ((i / 0x0C0) << 8) | (i % 0x0C0);
            if (m < 0x01F00)
                return (char)(m + 0x08140);
            else
                return (char)(m + 0x0C140); 
        }
        public static int CalculateBitCount(int length)
        {
            return length * 13;
        }
        public static int CalculateLength(int bitCount)
        {
            if (bitCount % 13 != 0)
                throw new ArgumentException("illegal bitCount");
            return bitCount / 13;
        }
        public static KanjiPayload Encode(string s)
        {
            if (!IsValid(s))
                throw new ArgumentException("invalid string.");

            KanjiPayload payload = new(CalculateBitCount(s.Length));
            payload.content = s;

            for (int i = 0; i < s.Length; i++)
                Utils.WriteEndiannessBits(ToInt(s[i]), payload.data, i * 13, 13);

            return payload;    
        }
        public static KanjiPayload Decode(bool[] message, int index, int length)
        {
            KanjiPayload payload = new(CalculateBitCount(length));
            StringBuilder sb = new(length);

            for (int i = 0; i < payload.BitCount; i++)
                payload.data[i] = message[index + i];

            for (int i = 0; i < length; i++)
                sb.Append(FromInt((int)Utils.ReadEndiannessBits(payload.data, i*13, 13)));

            payload.content = sb.ToString();
            return payload;
        }

        //Variables
        public override int Length { get { return content.Length; } }
        public override string Content { get { return content; } }

        private string content;

        //Constructeurs
        private KanjiPayload(int bitCount) : base(QRCodes.Encoding.Types.Kanji, bitCount) { }
    }
}

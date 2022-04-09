using MyProject;
using System;
using System.Text;

namespace QRCodes.PayloadTypes
{
    class NumericPayload : Payload
    {
        //Methodes statiques
        public static bool IsValid(char c)
        {
            return c >= '0' && c <= '9';
        }
        public static bool IsValid(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (!IsValid(s[i]))
                    return false;
            return true;
        }
        public static int CalculateBitCount(int length)
        {
            if (length % 3 == 0)
                return (length / 3) * 10;
            if (length % 3 == 1)
                return (length / 3) * 10 + 4;
            return (length / 3) * 10 + 7;
        }
        public static int CalculateLength(int bitCount)
        {
            int tripletCount = bitCount / 10;
            int remaining = bitCount % 10;
            if (remaining == 0)
                return tripletCount * 3;
            if (remaining == 4)
                return tripletCount * 3 + 1;
            if (remaining == 7)
                return tripletCount * 3 + 2;
            throw new ArgumentException("illegal bitCount", nameof(bitCount));
        }
        public static NumericPayload Decode(bool[] message, int index, int length)
        {
            NumericPayload payload = new(CalculateBitCount(length));
            for (int i = 0; i < payload.BitCount; i++)
                payload.data[i] = message[i + index]; 

            StringBuilder sb = new(length);
            int tripletCount = length / 10;
            int remaining = (length % 10) / 3;
            short n;

            for (int i = 0; i < tripletCount; i++)
            {
                n = (short)Utils.ReadEndiannessBits(message, i * 10 + index, 10);
                sb.Append(n);
            }

            if (remaining > 0)
            {
                n = (short)Utils.ReadEndiannessBits(message, tripletCount * 10 + index, length % 10);
                sb.Append(n);
            }

            payload.content = sb.ToString();
            return payload;
        }
        public static NumericPayload Encode(ulong l)
        {
            return Encode(Convert.ToString(l));
        }
        public static NumericPayload Encode(string s)
        {
            if (!IsValid(s))
                throw new ApplicationException(s + " is not a valid numeric payload.");

            NumericPayload payload = new(CalculateBitCount(s.Length));
            payload.content = s;

            int n;
            for(int i = 0; i < s.Length - 2; i += 3)
            {
                n = (s[i + 0] - '0') * 100 + (s[i + 1] - '0') * 10 + (s[i + 2] - '0') * 1;
                Utils.WriteEndiannessBits(n, payload.data, (i / 3) * 10, 10);
            }

            if (s.Length % 3 == 2)
            {
                n = (s[^2] - '0') * 10 + (s[^1] - '0') * 1;
                Utils.WriteEndiannessBits(n, payload.data, payload.BitCount - 7, 7);
            }

            if (s.Length % 3 == 1)
            {
                n = (s[^1] - '0') * 1;
                Utils.WriteEndiannessBits(n, payload.data, payload.BitCount - 4, 4);
            }

            return payload;
        }

        //Variables
        private string content;
        public override String Content { get { return content; } }
        public override int Length { get { return Content.Length; } }

        //Constructeurs
        private NumericPayload(int bitCount) : base(QRCodes.Encoding.Types.Numeric, bitCount) { }
    }
}

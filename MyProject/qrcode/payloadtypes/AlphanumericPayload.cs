using MyProject;
using System;
using System.Text;

namespace QRCodes.PayloadTypes
{
    class AlphanumericPayload : Payload
    {
        //Methode statiques
        private static char ToChar(int a)
        {
            if (a < 0 || a > 44)
                throw new ArgumentOutOfRangeException(nameof(a));

            if (a <= 9)
                return (char)('0' + a);
            if (a <= 35)
                return (char)('A' + a - 10);
            if (a == 36)
                return ' ';
            if (a == 37)
                return '$';
            if (a == 38)
                return '%';
            if (a == 39)
                return '*';
            if (a == 40)
                return '+';
            if (a == 41)
                return '-';
            if (a == 42)
                return '.';
            if (a == 43)
                return '/';
            //if (a == 44) 
                return ':';
        }
        private static int ToInt(char c)
        {
            if (c >= '0' && c <= '1')
                return c - '0';
            if (c >= 'A' && c <= 'Z')
                return c - 'A' + 10;
            if (c == ' ')
                return 36;
            if (c == '$')
                return 37;
            if (c == '%')
                return 38;
            if (c == '*')
                return 39;
            if (c == '+')
                return 40;
            if (c == '-')
                return 41;
            if (c == '.')
                return 42;
            if (c == '/')
                return 43;
            if (c == ':')
                return 44;
            throw new ArgumentException(c + " is not an alphanumeric character.");
        }
        private static bool IsValid(char c)
        {
            try
            {
                ToInt(c);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
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
            return (length / 2) * 11 + (length % 2) * 6;
        }
        public static int CalculateLength(int bitCount)
        {
            int pairCount = bitCount / 11;
            bool odd = bitCount % 11 == 6;

            if (bitCount % 11 != 0 && !odd)
                throw new ArgumentException("Illegal length.");

            return pairCount + (odd ? 1 : 0);
        }
        
        public static AlphanumericPayload Encode(string s)
        {
            if (!IsValid(s))
                throw new ApplicationException(s + " is not an alphanumeric string.");

            AlphanumericPayload payload = new(CalculateBitCount(s.Length));
            payload.content = s;

            int n;
            for (int i = 0; i < s.Length - 1; i += 2)
            {
                n = 45 * ToInt(s[i]) + ToInt(s[i + 1]);
                Utils.WriteEndiannessBits(n, payload.data, (i / 2) * 11, 11);
            }

            if (s.Length % 2 == 1)
                Utils.WriteEndiannessBits(ToInt(s[^1]), payload.data, payload.BitCount - 6, 6);

            return payload;
        }
        public static AlphanumericPayload Decode(bool[] message, int index, int length)
        {
            int bitCount = CalculateBitCount(length);
            AlphanumericPayload payload = new(bitCount);
            for (int i = 0; i < bitCount; i++)
                payload.data[i] = message[index + i];
            
            StringBuilder sb = new(length);
            short n;

            for (int i = 0; i < length - 1; i+=2)
            {
                n = (short)Utils.ReadEndiannessBits(message, index + i * 11, 11);
                sb.Append(ToChar(n / 45));
                sb.Append(ToChar(n % 45));
            }

            if (length % 2 == 1)
            {
                n = (short)Utils.ReadEndiannessBits(message, (length - 1) * 11 + index, 6);
                sb.Append(ToChar(n));
            }

            payload.content = sb.ToString();
            return payload;
        }
        
        //Variables
        public override string Content { get { return content; } }
        public override int Length { get { return content.Length; } }

        private string content;

        //Constructeurs
        private AlphanumericPayload(int bitcount) : base(QRCodes.Encoding.Types.Alphanumeric, bitcount) { }
    }
}
using System;
using System.Collections.Generic;
using MyProject;

namespace QRCodes
{
    class Format
    {
        //Variables
        private readonly static Dictionary<byte, short> ECM = new();
        private readonly static Dictionary<byte, int> V = new();
        
        //Init
        private static void Init()
        {
            if (ECM.Count != 0)
                return;

            for (byte i = 0b00000; i <= 0b11111; i++)
                AddECM(i);

            for (byte i = 1; i <= 40; i++)
                AddV(i);
        }
        private static void AddECM(byte b)
        {
            int temp = b << 10;
            int generator;

            int pow;
            while ((pow = Utils.FastLog2(temp)) >= 11)
            {
                generator = 0b10100110111 << pow - 11;
                temp ^= generator;
            }

            temp += b << 10;
            temp ^= 0b101010000010010;
            ECM.Add(b, (short)temp);
        }
        private static void AddV(byte b)
        {
            int temp = b << 12;
            int generator;

            int pow;
            while ((pow = Utils.FastLog2(temp)) >= 13)
            {
                generator = 0b1111100100101 << pow - 13;
                temp ^= generator;
            }

            temp += b << 12;
            V.Add(b, temp);
        }

        //ECM
        public static short GetFormatInformation(ErrorCorrection.Levels ecLevel, Mask mask)
        {
            Init();
            return ECM[(byte)((ErrorCorrection.ToInt(ecLevel) << 3) + mask.Type)];
        }
        public static ECMInfo ECMInfoFrom(short s0, short s1)
        {
            Init();
            short s00 = (short)(s0 >> 10);
            short s01 = (short)(s0 - (s00 << 10));
            short s10 = (short)(s1 >> 10);
            short s11 = (short)(s1 - (s10 << 10));

            byte b;
            short v0, v1;
            foreach(KeyValuePair<byte, short> kvp in ECM)
            {
                v0 = (short)(kvp.Value >> 10);
                v1 = (short)(kvp.Value - (v0 << 10));
                b = 0;

                if (s00 == v0) b++;
                if (s10 == v0) b++;
                if (s01 == v1) b+=2;
                if (s11 == v1) b+=2;

                if (b >= 3)
                    return new ECMInfo(kvp.Key);
            }

            throw new ApplicationException("Unable to parse format information.");
        }
        public class ECMInfo
        {
            public Mask Mask { get; private set; }
            public ErrorCorrection.Levels ECLevel { get; private set; }

            public ECMInfo(byte s)
            {
                this.ECLevel = ErrorCorrection.FromInt(s / 8);
                this.Mask = new Mask(s % 8);
            }
        }

        //V
        public static int GetVersionInformation(byte version)
        {
            Init();
            return V[version];
        }
        public static byte VersionFrom(int v0, int v1)
        {
            Init();
            short v00 = (short)(v0 >> 12);
            short v01 = (short)(v0 - (v00 << 12));
            short v10 = (short)(v1 >> 12);
            short v11 = (short)(v1 - (v10 << 12));

            short d0, d1;
            byte b;
            foreach (KeyValuePair<byte, int> kvp in V)
            {
                d0 = (short)(kvp.Value >> 12);
                d1 = (short)(kvp.Value - (d0 << 12));
                b = 0;

                if (v00 == d0) b++;
                if (v10 == d0) b++;
                if (v01 == d1) b+=2;
                if (v11 == d1) b+=2;

                if (b >= 3)
                    return kvp.Key;
            }

            throw new ApplicationException("Unable to parse version information.");
        }
    }
}

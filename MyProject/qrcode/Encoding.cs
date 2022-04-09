using System;

namespace QRCodes
{
    public class Encoding
    {
        //Methodes
        public static int ToInt(Types type)
        {
            if (type == Types.Numeric)
                return 0b0001;
            if (type == Types.Alphanumeric)
                return 0b0010;
            if (type == Types.Binary)
                return 0b0100;
            if (type == Types.Kanji)
                return 0b1000;
            throw new ArgumentException("unknow type " + type);
        }
        public static Types FromInt(int i)
        {
            if (i == 0b0001)
                return Types.Numeric;
            if (i == 0b0010)
                return Types.Alphanumeric;
            if (i == 0b0100)
                return Types.Binary;
            if (i == 0b1000)
                return Types.Kanji;
            throw new ArgumentException("unknow type " + i);
        }

        //Enum
        public enum Types
        {
            Numeric,
            Alphanumeric,
            Binary,
            Kanji
        }
    }
}

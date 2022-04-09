using System;

namespace QRCodes
{
    class Mask
    {
        //Variables
        public int Type { get; private set; }

        //Constructeurs
        public Mask(int type)
        {
            if (!(type >= 0x000 && type <= 0b111))
                throw new ArgumentException("Invalid type : " + type);
            this.Type = type;
        }

        //Methodes
        public bool Apply(int y, int x)
        {
            //Formulas
            if (Type == 0)
                return (x + y) % 2 == 0;
            else if (Type == 1)
                return (y % 2) == 0;
            else if (Type == 2)
                return x % 3 == 0;
            else if (Type == 3)
                return (y + x) % 3 == 0;
            else if (Type == 4)
                return (x / 2 + y / 3) % 2 == 0;
            else if (Type == 5)
                return (x * y) % 2 + (x * y) % 3 == 0;
            else if (Type == 6)
                return ((y * x) % 2 + (y * x) % 3) % 2 == 0;
            else if (Type == 7)
                return ((x + y) % 2 + (y * x) % 3) % 2 == 0;

            throw new InvalidOperationException("Invalid mask : " + Type);
        }
    }
}

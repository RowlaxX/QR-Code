namespace Bitmap
{
    class Kernel
    {
        //Enum Types
        public enum Types { Three, Five }

        //Variables
        public Types Type { get; private set; }
        public double[,] Mat { get; private set; }
        public int Size
        {
            get
            {
                if (this.Type == Types.Three)
                    return 3;
                return 5;
            }
        }
        public int From
        {
            get
            {
                if (this.Type == Types.Three)
                    return -1;
                return -2;
            }
        }
        public int To
        {
            get
            {
                return -From;
            }
        }

        //Constructeurs
        public Kernel(Types type)
        {
            this.Type = type;
            if (Type == Types.Three)
                this.Mat = new double[3, 3];
            else
                this.Mat = new double[5, 5];
        }
        public Kernel(Types type, double[,] mat) : this(type)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    this.Mat[i, j] = mat[i, j];
        }
        public Kernel() : this(Types.Three) {}

        //Methodes
        public Kernel Clone()
        {
            Kernel copy = new Kernel();
            copy.Type = this.Type;
            copy.Mat = (double[,])this.Mat.Clone();
            return copy;
        }
        public double Get(int i, int j)
        {
            if (Type == Types.Three)
                return Mat[1 + i, 1 + j];
            return Mat[2 + i, 2 + j];
        }
    }
}
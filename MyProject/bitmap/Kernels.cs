namespace Bitmap
{
    class Kernels
    {
        //Variables
        public static Kernel Sharpen = new Kernel(Kernel.Types.Three, new double[,] {
                {0, -1, 0 },
                {-1, 5, -1},
                {0, -1, 0}
        });

        public static Kernel Blur = new Kernel(Kernel.Types.Three, new double[,] {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
        });

        public static Kernel Blur5x5 = new Kernel(Kernel.Types.Three, new double[,] {
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1}
        });

        public static Kernel EdgeEnhance = new Kernel(Kernel.Types.Three, new double[,] {
                {0, 0, 0},
                {-1, 1, 0},
                {0, 0, 0}
        });

        public static Kernel EdgeDetect = new Kernel(Kernel.Types.Three, new double[,] {
                {0, 1, 0 },
                {1, -4, 1},
                {0, 1, 0}
        });

        public static Kernel Emboss = new Kernel(Kernel.Types.Three, new double[,] {
                {-2, -1, 0 },
                {-1, 1, 1},
                {0, 1, 2}
        });
    }
}

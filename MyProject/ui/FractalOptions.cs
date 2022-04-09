using System.Windows.Controls;
using Bitmap;

namespace MyProject.ui
{
    class FractalOptions : Options
    {
        //Variables
        private readonly Button create = new();

        //Constructeurs
        public FractalOptions(MainWindow mainWindow) : base(mainWindow, 1, 1)
        {
            create.Content = "Créer une fractale";
            create.Height = 80;

            Add(create, 0, 0);

            create.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            create.Click += Delegate(Click);
        }
        //Methodes
        private void Click()
        {
            MainWindow.Output = Fractals.Mandelbrot(600, 800, 1000);
        }
    }
}

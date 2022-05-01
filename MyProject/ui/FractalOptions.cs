using System;
using System.Windows.Controls;
using Bitmap;

namespace MyProject.ui
{
    class FractalOptions : Options
    {
        //Variables
        private readonly Button create = new();
        private readonly TextBox maxIteration = new();
        private readonly TextBox height = new();
        private readonly TextBox width = new();
        private readonly TextBox topLeftY = new(), topLeftX = new(), bottomRightY = new(), bottomRightX = new();

        //Constructeurs
        public FractalOptions(MainWindow mainWindow) : base(mainWindow, 5, 4)
        {
            create.Content = "Créer une fractale";

            Add("Iterations : ", 0, 0);
            Add(maxIteration, 1, 0);
            Add(create, 3, 0);

            Add("Coordonnée Y coin haut gauche : ", 0, 1);
            Add(topLeftY, 1, 1);
            Add("Coordonnée X coin haut gauche : ", 2, 1);
            Add(topLeftX, 3, 1);

            Add("Coordonnée Y coin bas droite : ", 0, 2);
            Add(bottomRightY, 1, 2);
            Add("Coordonnée X coin bas droite : ", 2, 2);
            Add(bottomRightX, 3, 2);

            Add("Hauteur : ", 0, 3);
            Add(height, 1, 3);
            Add("Largeur : ", 2, 3);
            Add(width, 3, 3);

            create.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            height.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            width.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            topLeftX.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            topLeftY.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            bottomRightX.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            bottomRightY.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            maxIteration.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            create.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            bottomRightX.Text = "2";
            bottomRightY.Text = "-2";
            topLeftY.Text = "2";
            topLeftX.Text = "-2";
            height.Text = "1080";
            width.Text = "1920";
            maxIteration.Text = "50";

            create.Click += Delegate(Click);
        }
        //Methodes
        private void Click()
        {
            uint mi = Convert.ToUInt32(maxIteration.Text);
            int h = Convert.ToInt32(height.Text);
            int w = Convert.ToInt32(width.Text);
            double tlY = Convert.ToDouble(topLeftY.Text);
            double tlX = Convert.ToDouble(topLeftX.Text);
            double brY = Convert.ToDouble(bottomRightY.Text);
            double brX = Convert.ToDouble(bottomRightX.Text);

            MainWindow.Output = Fractals.Mandelbrot(h, w, mi, tlY, tlX, brY, brX);
        }
    }
}

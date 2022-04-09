using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmap;
using System.Windows.Controls;

namespace MyProject.ui
{
    class BitmapHideOptions : Options
    {
        //Variables
        private readonly Button button = new();

        //Constructeurs
        public BitmapHideOptions(MainWindow mainWindow) : base(mainWindow, 1, 1)
        {
            button.Content = "Cacher l'entrée dans...";
            button.Click += Delegate(Hide);
            Add(button, 0, 0);

            button.Height = 80;
            button.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }
        //Methodes
        private void Hide()
        {
            BitMap input = MainWindow.Input;
            BitMap hideIn = new BitMap(MainWindow.LoadImage(), (int)input.Height, (int)input.Width, true);
            MainWindow.Output = BitmapHiding.Hide(hideIn, input);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmap;
using System.Windows.Controls;

namespace MyProject.ui
{
    class BitmapRecoverOptions : Options
    {
        //Variables
        private Button button = new();

        //Constructeurs
        public BitmapRecoverOptions(MainWindow mainWindow) : base(mainWindow, 1, 1)
        {
            button.Content = "Retrouver l'image depuis l'entrée";
            button.Height = 80;
            button.Click += Delegate(Apply);

            Add(button, 0, 0);

            button.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }
        //Methodes
        private void Apply()
        {
            MainWindow.Output = BitmapHiding.Recover(MainWindow.Input);
        }
    }
}

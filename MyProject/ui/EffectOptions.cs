using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyProject.ui
{
    class EffectOptions : Options
    {
        //Variables
        private readonly Button graynify = new(), blackify = new(), inverse = new();

        //Constructeurs
        public EffectOptions(MainWindow mainWindow) : base(mainWindow, 1, 3)
        {
            Add(graynify, 0, 0);
            Add(blackify, 0, 1);
            Add(inverse, 0, 2);

            graynify.Content = "Griser l'entrée";
            graynify.Height = 60;
            graynify.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            graynify.Click += Delegate(Graynify);

            blackify.Content = "Binariser l'entrée";
            blackify.Height = 60;
            blackify.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            blackify.Click += Delegate(Blackify);

            inverse.Content = "Inverser les couleurs";
            inverse.Height = 60;
            inverse.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            inverse.Click += Delegate(Inverse);
        }
        //Methodes
        private void Blackify()
        {
            MainWindow.Output = MainWindow.Input.Blackify();
        }
        private void Graynify()
        {
            MainWindow.Output = MainWindow.Input.Graynify();
        }
        private void Inverse()
        {
            MainWindow.Output = MainWindow.Input.InverseColors();
        }
    }
}

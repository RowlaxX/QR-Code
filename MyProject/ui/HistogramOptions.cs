using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyProject.ui
{
    class HistogramOptions : Options
    {
        //Variables
        private readonly Button create = new();

        //Constructeurs
        public HistogramOptions(MainWindow main) : base(main, 1, 1)
        {
            create.Content = "Créer l'histogramme";
            create.Click += Delegate(Apply);
            Add(create, 0, 0);

            create.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            create.Height = 80;
        }
        //Methodes
        private void Apply()
        {
            MainWindow.Output = MainWindow.Input.CreateHistogram().ToBitmap().Scale(2);
        }
    }
}

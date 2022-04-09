using Bitmap;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MyProject.ui
{
    class ResizeOptions : Options
    {
        //Variables
        public bool KeepProportion { get { return proportion.IsChecked.Value; } }
        public bool IsPixelMode { get { return pixelMode.IsChecked.Value; } }

        private readonly Label heightHint = new(), widthHint = new();
        private readonly TextBox heightInput = new(), widthInput = new();
        private readonly RadioButton pixelMode = new(), ratioMode = new();
        private readonly CheckBox proportion = new();
        private readonly Button apply = new();

        //Constructeurs
        public ResizeOptions(MainWindow window) : base(window, 1, 2)
        {
            heightInput.Width = 150;
            heightInput.Height = 30;
            widthInput.Width = 150;
            widthInput.Height = 30;
            pixelMode.Content = "Taille en pixel";
            pixelMode.Checked += Delegate(SetPixelMode);
            ratioMode.Content = "Ratio";
            ratioMode.Checked += Delegate(SetRatioMode);
            proportion.Content = "Conserver les proportions ?";
            apply.Content = "Redimensionner";
            apply.Width = 120;
            apply.Height = 40;
            apply.Click += Delegate(Resize);

            SetPixelMode();
            Add(heightHint, 0, 0);
            Add(heightInput, 0, 0);
            Add(widthHint, 0, 0);
            Add(widthInput, 0, 0);

            Add(pixelMode, 0, 1);
            Add(ratioMode, 0, 1);
            Add(proportion, 0, 1);
            Add(apply, 0, 1);
        }

        public void SetRatioMode()
        {
            pixelMode.IsChecked = false;
            ratioMode.IsChecked = true;
            heightInput.Text = "100";
            widthInput.Text = "100";
            heightHint.Content = "Entrer un nouveau ratio pour la hauteur (en %).";
            widthHint.Content = "Entrer un nouveau ratio pour la largeur (en %).";
        }
        public void SetPixelMode()
        {
            pixelMode.IsChecked = true;
            ratioMode.IsChecked = false;
            BitMap input = MainWindow.HasInput ? MainWindow.Input : null;
            heightInput.Text = input == null ? "1080" : "" + input.Height;
            widthInput.Text = input == null ? "1920" : "" + input.Width;
            heightHint.Content = "Entrer une nouvelle hauteur en pixel.";
            widthHint.Content = "Entrer une nouvelle largeur en pixel.";
        }
        public void Resize()
        {
            BitMap input = MainWindow.Input;
            int height = int.Parse(heightInput.Text);
            int width = int.Parse(widthInput.Text);
            
            if (!IsPixelMode)
            {
                height = (int)(height / 100.0 * input.Height);
                width = (int)(width / 100.0 * input.Width);
            }

            MainWindow.Output = new BitMap(input, height, width, KeepProportion);
        }
    }
}

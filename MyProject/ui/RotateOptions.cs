using System;
using System.Text;
using System.Windows.Controls;
using Bitmap;

namespace MyProject.ui
{
    class RotateOptions : Options
    {
        //Variables
        public int Degree 
        {
            get
            {
                return (int)slider.Value;
            }
        }
        public bool Resize
        {
            get
            {
                return resize.IsChecked.Value;
            }
        }

        private readonly Button horizontalMirror = new(), verticalMirror = new();
        private readonly Label sliderHint = new();
        private readonly Slider slider = new();
        private readonly TextBox angleInput = new();
        private readonly CheckBox resize = new();
        private readonly Button apply = new();

        //Constructeurs
        public RotateOptions(MainWindow mainWindow) : base(mainWindow, 1, 2)
        {
            apply.Content = "Appliquer";
            apply.Click += Delegate(Rotate);
            apply.Width = 180;
            apply.Height = 30;

            horizontalMirror.Content = "Inverser horizontalement";
            horizontalMirror.Height = 35;
            horizontalMirror.Width = 180;
            horizontalMirror.Click += Delegate(HorizontalMirror);

            verticalMirror.Content = "Inverser vertialement";
            verticalMirror.Height = 35;
            verticalMirror.Width = 180;
            verticalMirror.Click += Delegate(VerticalMirror);

            sliderHint.Content = "Angle (en degrée) :";
            slider.Minimum = 0;
            slider.Maximum = 360;
            slider.Width = 180;
            slider.ValueChanged += delegate { UpdateAngleInput(); };

            resize.Content = "Calculer la nouvelle taille ?";
            
            angleInput.Height = 30;
            angleInput.Width = 100;
            angleInput.Text = "0";
            angleInput.TextChanged += delegate { UpdateSlider(); };

            Add(sliderHint, 0, 0);
            Add(slider, 0, 0);
            Add(angleInput, 0, 0);
            Add(resize, 0, 0);
            Add(apply, 0, 0);

            Add(horizontalMirror, 0, 1);
            Add(verticalMirror, 0, 1);
        }
        private void UpdateSlider()
        {
            string toParse = angleInput.Text;

            StringBuilder sb = new(toParse.Length);
            foreach (char c in toParse)
                if (c >= '0' && c <= '9')
                    sb.Append(c);

            if (sb.Length == 0)
                return;

            string parsed = sb.ToString();
            int value = int.Parse(parsed);

            if (value > 360)
                value = 360;

            angleInput.Text = value.ToString();
            slider.Value = value;
        }
        private void UpdateAngleInput()
        {
            angleInput.Text = ((int)slider.Value).ToString();
        }
        private void HorizontalMirror()
        {
            Mirror(BitMap.Directions.Horizontal);
        }
        private void VerticalMirror()
        {
            Mirror(BitMap.Directions.Vertical);
        }
        private void Mirror(BitMap.Directions direction)
        {
            MainWindow.Output = MainWindow.Input.Mirror(direction);
        }
        private void Rotate()
        {
            MainWindow.Output = MainWindow.Input.Rotate(Degree, Resize);
        }
    }
}

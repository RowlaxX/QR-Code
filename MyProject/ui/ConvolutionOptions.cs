using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Bitmap;

namespace MyProject.ui
{
    class ConvolutionOptions : Options
    {
        //Methodes statiques
        private static Grid CreateCustomKernelGrid()
        {
            Grid grid = new Grid();
            for (int i = 0; i < 5; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    TextBox tb = new();
                    tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                    tb.VerticalAlignment = VerticalAlignment.Stretch;
                    tb.VerticalContentAlignment = VerticalAlignment.Center;
                    tb.HorizontalContentAlignment = HorizontalAlignment.Left;
                    tb.Text = "0";
                    tb.Margin = new(1, 1, 1, 1);
                    tb.TextChanged += delegate { tb.Text = Format(tb.Text); };
                    SetRow(tb, i);
                    SetColumn(tb, j);
                    grid.Children.Add(tb);
                }

            return grid;
        }
        private static string Format(string s)
        {
            StringBuilder sb = new(s.Length);
            if (s.Contains('-'))
                sb.Append('-');

            foreach (char c in s)
                if (c >= '0' && c <= '9')
                    sb.Append(c);

            return sb.ToString();
        }

        //Variables
        private CheckBox customKernel = new();
        private ComboBox customKernelSize = new();
        private CheckBox doR = new(), doG = new(), doB = new();
        private ComboBox borders = new();
        private Button apply = new();
        private RadioButton blur = new(), sharpen = new(), edgeEnhance = new(), edgeDetect = new(), emboss = new();
        private Grid ck = new();

        public bool CustomKernel { get { return customKernel.IsChecked.Value; } }
        public bool DoR { get { return doR.IsChecked.Value; } }
        public bool DoG { get { return doG.IsChecked.Value; } }
        public bool DoB { get { return doB.IsChecked.Value; } }
        public KernelSettings.Borders Border
        {
            get
            {
                if (borders.SelectedIndex == 0)
                    return KernelSettings.Borders.Extend;
                if (borders.SelectedIndex == 1)
                    return KernelSettings.Borders.Wrap;
                return KernelSettings.Borders.Crop;
            }
        }
        public KernelSettings KernelSettings
        {
            get
            {
                KernelSettings settings = new KernelSettings();
                settings.RedChannel = DoR;
                settings.BlueChannel = DoB;
                settings.GreenChannel = DoG;
                settings.Border = Border;
                return settings;
            }
        }
        public Kernel.Types KernelType { 
            get 
            {
                if (customKernelSize.SelectedIndex == 0)
                    return Kernel.Types.Three;
                return Kernel.Types.Five;
            } 
        }
        public Kernel Kernel
        {
            get
            {
                if (CustomKernel)
                    return ReadCustomKernel();
                else
                {
                    if (blur.IsChecked.Value)
                        return Kernels.Blur;
                    if (edgeDetect.IsChecked.Value)
                        return Kernels.EdgeDetect;
                    if (edgeEnhance.IsChecked.Value)
                        return Kernels.EdgeEnhance;
                    if (emboss.IsChecked.Value)
                        return Kernels.Emboss;
                    if (sharpen.IsChecked.Value)
                        return Kernels.Sharpen;
                }

                throw new ApplicationException("Unknow kernel.");
            }
        }

        //Constructeurs
        public ConvolutionOptions(MainWindow mainWindow) : base(mainWindow, 1, 4)
        {
            ck = CreateCustomKernelGrid();
            customKernel.Content = "Matrice personalisée ?";
            customKernel.Click += Delegate(UpdateKernel);
            customKernelSize.Items.Add("3x3");
            customKernelSize.Items.Add("5x5");
            customKernelSize.SelectedIndex = 0;
            customKernelSize.SelectionChanged += delegate { UpdateKernelMode(); };
            blur.GroupName = "Kernels";
            blur.Content = "Blur";
            blur.IsChecked = true;
            edgeDetect.GroupName = "Kernels";
            edgeDetect.Content = "Edge Detect";
            edgeEnhance.GroupName = "Kernels";
            edgeEnhance.Content = "Edge Enhance";
            emboss.GroupName = "Kernels";
            emboss.Content = "Emboss";
            sharpen.GroupName = "Kernels";
            sharpen.Content = "Sharpen";
            doR.IsChecked = true;
            doG.IsChecked = true;
            doB.IsChecked = true;
            doR.Content = "Red channel";
            doG.Content = "Green channel";
            doB.Content = "Blue channel";

            borders.Items.Add("Extends");
            borders.Items.Add("Wrap");
            borders.Items.Add("Crop");
            borders.SelectedIndex = 0;

            apply.Content = "Appliquer";
            apply.Height = 50;
            apply.Click += Delegate(Apply);

            Add(customKernel, 0, 0);
            Add(customKernelSize, 0, 0);
            Add(doR, 0, 2);
            Add(doG, 0, 2);
            Add(doB, 0, 2);
            Add("Mode", 0, 3);
            Add(borders, 0, 3);
            Add(apply, 0, 3);

            UpdateKernel();
            UpdateKernelMode();

            apply.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        //Methodes
        private void UpdateKernelMode()
        {
            SetCustomKernelMode(KernelType);
        }
        private void SetCustomKernelMode(Kernel.Types type)
        {
            int row, column;
            Visibility v = type == Kernel.Types.Five ? Visibility.Visible : Visibility.Hidden;
            foreach(UIElement e in ck.Children)
            {
                row = GetRow(e);
                column = GetColumn(e);

                if (row == 0 || row == 4 || column == 0 || column == 4)
                    e.Visibility = v;
            }
        }
        private Kernel ReadCustomKernel()
        {
            int size = KernelType == Kernel.Types.Three ? 3 : 5;
            double[,] mat = new double[size, size];
            int row;
            int column;
            int value;
            foreach(UIElement e in ck.Children)
            {
                row = GetRow(e);
                column = GetColumn(e);
                if (size == 3)
                {
                    row--;
                    column--;
                }

                if (row < 0 || row >= size)
                    continue;
                if (column < 0 || column >= size)
                    continue;

                try
                {
                    value = int.Parse(((TextBox)e).Text);
                }
                catch (Exception)
                {
                    value = 0;
                }
                mat[row, column] = value;
            }
            return new Kernel(KernelType, mat);
        }
        private void UpdateKernel()
        {
            GetPanel(0, 1).Children.Clear();

            if (CustomKernel)
            {
                customKernelSize.Visibility = Visibility.Visible;
                GetPanel(0, 1).Children.Add(ck);
            }
            else
            {
                customKernelSize.Visibility = Visibility.Hidden;
                Add(blur, 0, 1);
                Add(sharpen, 0, 1);
                Add(edgeEnhance, 0, 1);
                Add(edgeDetect, 0, 1);
                Add(emboss, 0, 1);
            }
        }
        private void Apply()
        {
            MainWindow.Output = MainWindow.Input.ApplyKernel(Kernel, KernelSettings);
        }
    }
}

using MyProject.UI;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Bitmap;
using System.IO;
using Microsoft.Win32;
using MyProject.ui;

namespace MyProject
{
    //TODO UI     : Proposer des options pour la fractale
    //TODO Bitmap : Construire image par le haut
    //TODO Bitmap : Corriger matrice convolution
    //TODO Bitmap : Corriger rotation

    public partial class MainWindow : Window
    {
        //Methodes statiques
        internal static BitmapImage ToBitmapImage(string filename)
        {
            return ToBitmapImage(new BitMap(Environment.CurrentDirectory + "/resources/" + filename));
        }
        internal static BitmapImage ToBitmapImage(BitMap bitmap)
        {
            using (MemoryStream memory = new())
            {
                bitmap.Save(memory, false);

                memory.Position = 0;
                BitmapImage bitmapimage = new();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        internal static BitMap LoadImage()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Choisissez une image.";
            openFileDialog.DefaultExt = "bmp";
            openFileDialog.Filter = "Bitmap image (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog().Value)
                return new BitMap(openFileDialog.OpenFile());

            return null;
        }
        internal static void SaveImage(BitMap toSave)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Title = "Enregistrer sous...";
            saveFileDialog.FileName = "output.bmp";
            saveFileDialog.DefaultExt = "bmp";
            saveFileDialog.Filter = "Bitmap image (*.bmp)|*.bmp";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog().Value)
            {
                toSave.Save(saveFileDialog.OpenFile());
                MessageBox.Show("Enregistré", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Variables
        internal BitMap Input
        {
            get
            {
                if (input == null)
                    throw new ApplicationException("Il n'y a pas d'entrée.");
                return input;
            }
            set
            {
                input = value;
                InputImage.Source = input == null ? null : ToBitmapImage(input);
                InputDim.Text = "Entrée : " + (input == null ? null : input.Width + "x" + input.Height);
            }
        }
        internal BitMap Output
        {
            get
            {
                if (output == null)
                    throw new ApplicationException("Il n'y a pas de sortie.");
                return output;
            }
            set
            {
                this.output = value;
                OutputImage.Source = output == null ? null : ToBitmapImage(output);
                OutputDim.Text = "Sortie : " + (output == null ? null : output.Width + "x" + output.Height);
            }
        }
        internal bool HasInput { get { return input != null; } }
        internal bool HasOutput { get { return output != null; } }
        private BitMap input = null, output = null;
        private readonly MyMenu menu;

        internal readonly ResizeOptions resizeOptions;
        internal readonly RotateOptions rotateOptions;
        internal readonly EffectOptions effectOptions;
        internal readonly ConvolutionOptions convolutionOptions;
        internal readonly HistogramOptions histogramOptions;
        internal readonly FractalOptions fractalOptions;
        internal readonly BitmapHideOptions bitmapHideOptions;
        internal readonly BitmapRecoverOptions bitmapRecoverOptions;
        internal readonly QRCodeCreateOptions qrcodeCreateOptions;
        internal readonly QRCodeReadOptions qrcodeReadOptions;

        //Constructeurs
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.ColumnDefinitions[0].Width = new(UI.MyMenu.MENU_SIZE);
            OutputAsInput.Click += delegate { Apply(); };
            
            resizeOptions = new(this);
            rotateOptions = new(this);
            effectOptions = new(this);
            convolutionOptions = new(this);
            histogramOptions = new(this);
            bitmapHideOptions = new(this);
            bitmapRecoverOptions = new(this);
            qrcodeCreateOptions = new(this);
            qrcodeReadOptions = new(this);
            fractalOptions = new(this);

            menu = new(this);
            menu.RefreshContent();
        }
        //Methodes
        internal void Apply()
        {
            if (output != null)
            {
                InputImage.Source = OutputImage.Source;
                OutputImage.Source = null;
                input = output;
                OutputDim.Text = "Sortie : ";
                output = null;
                InputDim.Text = "Entrée : " + input.Width + "x" + input.Height;
            }
        }
        internal void LoadInput()
        {
            BitMap loaded = LoadImage();
            if (loaded != null)
                Input = loaded;
        }
        internal void SetOptions(Options option)
        {
            Options.Content = option;
        }
        internal void SaveInput()
        {
            SaveImage(Input);
        }
    }
}

using Bitmap;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using MyProject.ui;

namespace MyProject.UI
{
    class MyMenu
    {
        //Constantes
        public const int MENU_SIZE = 200;
        public const int IMAGE_SIZE = 40;
        public const int PADDING = 10;

        //Variables
        private readonly MainWindow mainWindow;
        private readonly Menu mymenu;
        private readonly ScrollViewer scroll;

        //Constructeurs
        public MyMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            this.scroll = mainWindow.MyMenu;
            this.mymenu = new();
            this.mymenu.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
            scroll.Content = mymenu;
        }

        //Methodes
        public void RefreshContent()
        {
            mymenu.Items.Clear();
            AddItem("Ouvrir", "bitmap_open.bmp", mainWindow.LoadInput);
            AddItem("Enregistrer", "bitmap_save.bmp", mainWindow.SaveInput);
            AddItem("Redimensioner", "bitmap_resize.bmp", mainWindow.resizeOptions);
            AddItem("Pivoter", "bitmap_rotate.bmp", mainWindow.rotateOptions);
            AddItem("Effets", "bitmap_effect.bmp", mainWindow.effectOptions);
            AddItem("Filtres", "bitmap_convolution.bmp", mainWindow.convolutionOptions);
            AddItem("Histogramme", "bitmap_histogram.bmp", mainWindow.histogramOptions);
            AddItem("Fractale", "fractal.bmp", mainWindow.fractalOptions);
            AddItem("Cacher image", "bitmap_hide.bmp", mainWindow.bitmapHideOptions);
            AddItem("Retrouver image", "bitmap_recover.bmp", mainWindow.bitmapRecoverOptions);
            AddItem("Créer QR code", "qrcode_create.bmp", mainWindow.qrcodeCreateOptions);
            AddItem("Lire QR code", "qrcode_read.bmp", mainWindow.qrcodeReadOptions);
        }
        private void AddItem(string name, string filename, Options options)
        {
            AddItem(name, filename, delegate { mainWindow.SetOptions(options); });
        }
        private void AddItem(string name, string filename, Action action)
        {
            Grid grid = new();
            grid.Height = IMAGE_SIZE + 2*PADDING;
            grid.Width = MENU_SIZE;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Background = new SolidColorBrush(System.Windows.Media.Colors.White);

            ColumnDefinition cd1 = new();
            cd1.Width = new(IMAGE_SIZE + 2 * PADDING);
            grid.ColumnDefinitions.Add(cd1);
            grid.ColumnDefinitions.Add(new());

            RowDefinition rd = new();
            rd.Height = new(grid.Height);
            grid.RowDefinitions.Add(rd);

            Image image = new();
            image.Width = IMAGE_SIZE;
            image.Height = IMAGE_SIZE;
            image.Source = MainWindow.ToBitmapImage(filename);
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);

            TextBlock text = new();
            text.Text = name;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetRow(text, 0);
            Grid.SetColumn(text, 1);

            grid.Children.Add(text);
            grid.MouseLeftButtonDown += delegate
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            mymenu.Items.Add(grid);
        }
    }
}
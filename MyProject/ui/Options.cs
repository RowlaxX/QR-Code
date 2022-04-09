using System;
using System.Windows;
using System.Windows.Controls;

namespace MyProject.ui
{
    class Options : Grid
    {
        //Variables
        public bool HidePreview { get; set; } = false;
        public MainWindow MainWindow { get; private set; }
        public Label Title { get; private set; } = new Label();

        private readonly StackPanel[,] panels;

        //Constructeurs
        public Options(MainWindow mainWindow, int row, int column)
        {
            if (mainWindow == null)
                throw new ArgumentNullException(nameof(mainWindow));
            if (row <= 0)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column <= 0)
                throw new ArgumentOutOfRangeException(nameof(column));
                
            for (int i = 0; i < row; i++)
                RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < column; j++)
                ColumnDefinitions.Add(new ColumnDefinition());

            MainWindow = mainWindow;
            panels = new StackPanel[row, column];
        }
        //Methodes statiques
        protected static RoutedEventHandler Delegate(Action action)
        {
            return delegate 
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        //Methodes
        private StackPanel CreatePanel(int row, int column)
        {
            StackPanel sp = new();
            SetRow(sp, row + 1);
            SetColumn(sp, column);
            panels[row, column] = sp;

            sp.Orientation = Orientation.Vertical;
            sp.Margin = new(20, 10, 20, 10);
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.VerticalAlignment = VerticalAlignment.Center;

            Children.Add(sp);
            return sp;
        }
        public StackPanel GetPanel(int row, int column)
        {
            if (panels[row, column] == null)
                return CreatePanel(row, column);
            return panels[row, column];
        }
        public void Add(Control e, int row, int column)
        {
            StackPanel panel = GetPanel(row, column);
            e.Margin = new(10, 3, 10, 3);
            e.HorizontalAlignment = HorizontalAlignment.Left;
            e.VerticalAlignment = VerticalAlignment.Center;
            e.HorizontalContentAlignment = HorizontalAlignment.Center;
            e.VerticalContentAlignment = VerticalAlignment.Center;
            panel.Children.Add(e);
        }
        public void Add(string text, int row, int column)
        {
            Label label = new();
            label.Content = text;
            Add(label, row, column);
        }
    }
}

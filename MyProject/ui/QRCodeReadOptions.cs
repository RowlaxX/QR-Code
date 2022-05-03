using QRCodes;
using QRCodes.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyProject.ui
{
    class QRCodeReadOptions : Options
    {
        //Variables
        private readonly Label info = new();
        private readonly Button read = new();
        private readonly TextBox payload = new();

        //Constructeurs
        public QRCodeReadOptions(MainWindow mainWindow) : base(mainWindow, 1, 3)
        {
            read.Content = "Lire l'entrée";
            payload.IsReadOnly = true;

            read.Height = 60;
            read.Click += Delegate(Read);
            payload.Height = 100;
            info.Height = 100;

            Add(read, 0, 0);
            Add("Les photos fonctionnent aussi.", 0, 0);
            Add(payload, 0, 2);
            Add(info, 0, 1);

            read.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            payload.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            info.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }
        //Methodes
        private void Read()
        {
            QRCodeReader reader = new(MainWindow.Input);
            QRCode qr = reader.Read();

            StringBuilder sb = new();
            sb.AppendLine("Version : " + qr.Version);
            sb.AppendLine("Encodage : " + qr.Informations.EncodingType);
            sb.AppendLine("Masque : " + qr.AppliedMask.Type);
            sb.AppendLine("EC : " + qr.Informations.ErrorCorrection.Level);
            sb.AppendLine("Penalité : " + qr.Penalty);

            info.Content = sb.ToString();
            payload.Text = qr.Payload.Content;
        }
    }
}

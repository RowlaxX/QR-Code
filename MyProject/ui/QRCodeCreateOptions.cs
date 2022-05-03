using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCodes;
using System.Windows.Controls;
using System.IO;

namespace MyProject.ui
{
    class QRCodeCreateOptions : Options
    {
        //Variables
        private readonly ComboBox encoding = new();
        private readonly ComboBox ecLevel = new();
        private readonly ComboBox mask = new();
        private readonly Slider version = new();
        private readonly TextBox versiontb = new();
        private readonly Button create = new();
        private readonly TextBox payload = new();
        private readonly Label info = new();
        private readonly CheckBox autoversion = new();

        public Payload Payload { get {
                if (AutoEncoding)
                    return Payloads.Encode(payload.Text);
                return Payloads.Encode(payload.Text, Encoding); } }
        public bool AutoVersion { get { return autoversion.IsChecked.Value; } }
        public int Version { get { return (int)version.Value; } }
        public ErrorCorrection.Levels ECLevel { get
            {
                if (ecLevel.SelectedIndex == 0)
                    return ErrorCorrection.Levels.LOW;
                else if (ecLevel.SelectedIndex == 1)
                    return ErrorCorrection.Levels.MEDIUM;
                else if (ecLevel.SelectedIndex == 2)
                    return ErrorCorrection.Levels.QUARTIL;
                else
                    return ErrorCorrection.Levels.HIGH;
            } }
        public QRCodes.Encoding.Types Encoding { get
            {
                if (encoding.SelectedIndex == 0)
                    return 0;
                if (encoding.SelectedIndex == 1)
                    return QRCodes.Encoding.Types.Alphanumeric;
                if (encoding.SelectedIndex == 2)
                    return QRCodes.Encoding.Types.Numeric;
                if (encoding.SelectedIndex == 3)
                    return QRCodes.Encoding.Types.Binary;
                else
                    return QRCodes.Encoding.Types.Kanji;
            } }
        public bool AutoEncoding
        {
            get { return encoding.SelectedIndex == 0; }
        }
        public bool AutoMask { get { return mask.SelectedIndex == 0; } }
        public int Mask { get { return mask.SelectedIndex - 1; } }

        //Constructeurs
        public QRCodeCreateOptions(MainWindow mainWindow) : base(mainWindow, 1, 3)
        {
            version.Minimum = 1;
            version.Maximum = 40;
            version.ValueChanged += delegate { UpdateVersionInput(); };
            version.Value = 1;

            payload.Height = 50;

            versiontb.Text = "1";
            versiontb.Width = 50;
            versiontb.TextChanged += delegate { UpdateSlider(); };

            info.Height = 100;
            
            autoversion.Content = "Version automatique ?";
            autoversion.IsChecked = true;
            autoversion.Click += Delegate(UpdateAutoVersion);

            create.Height = 60;
            create.Content = "Créer";
            create.Click += Delegate(Create);

            encoding.Items.Add("Auto");
            encoding.Items.Add("Alphanumerique");
            encoding.Items.Add("Numerique");
            encoding.Items.Add("Binaire");
            encoding.Items.Add("Kanji");
            encoding.SelectedIndex = 0;

            ecLevel.Items.Add("Faible (7%)");
            ecLevel.Items.Add("Moyenne (15%)");
            ecLevel.Items.Add("Quartil (25%)");
            ecLevel.Items.Add("Haute (30%)");
            ecLevel.SelectedIndex = 0;

            mask.Items.Add("Auto");
            for (int i = 0; i < 8; i++)
                mask.Items.Add("Masque " + i);
            mask.SelectedIndex = 0;

            Add("Payload : ", 0, 0);
            Add(payload, 0, 0);
            Add("Version : ", 0, 0);
            Add(autoversion, 0, 0);
            Add(version, 0, 0);
            Add(versiontb, 0, 0);

            Add("Correction d'erreur : ", 0, 1);
            Add(ecLevel, 0, 1);
            Add("Encodage : ", 0, 1);
            Add(encoding, 0, 1);
            Add("Masque : ", 0, 1);
            Add(mask, 0, 1);

            Add(create, 0, 2);
            Add(info, 0, 2);

            version.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            payload.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            payload.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            create.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            UpdateAutoVersion();
        }
        //Methodes
        private void UpdateVersionInput()
        {
            versiontb.Text = Version.ToString();
        }
        private void UpdateSlider()
        {
            string toParse = versiontb.Text;

            StringBuilder sb = new(toParse.Length);
            foreach (char c in toParse)
                if (c >= '0' && c <= '9')
                    sb.Append(c);

            if (sb.Length == 0)
                return;

            string parsed = sb.ToString();
            int value = int.Parse(parsed);

            if (value > 40)
                value = 40;

            versiontb.Text = value.ToString();
            version.Value = value;
        }
        private void UpdateAutoVersion()
        {
            if (AutoVersion)
            {
                version.Visibility = System.Windows.Visibility.Hidden;
                versiontb.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                version.Visibility = System.Windows.Visibility.Visible;
                versiontb.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void Create()
        {
            QRCode.Builder builder = QRCode.NewBuilder();

            if (AutoVersion)
                builder.AutoVersion();
            else
                builder.SetVersion(Version);

            if (AutoMask)
                builder.AutoMask();
            else
                builder.SetMask(Mask);

            builder.SetErrorCorrectionLevel(ECLevel);
            builder.SetPayload(Payload);

            QRCode qr = builder.Build();
            StringBuilder sb = new();
            sb.AppendLine("Version : " + qr.Version);
            sb.AppendLine("Encodage : " + qr.Informations.EncodingType);
            sb.AppendLine("Masque : " + (qr.IsMasked() ? qr.AppliedMask.Type : "aucun"));
            sb.AppendLine("EC : " + qr.Informations.ErrorCorrection.Level);
            sb.AppendLine("Penalité : " + qr.Penalty);

            info.Content = sb.ToString();
            MainWindow.Output = qr.ToBitMap().Scale(10);
        }
    }
}

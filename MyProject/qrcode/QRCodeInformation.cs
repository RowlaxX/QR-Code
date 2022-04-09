using System;

namespace QRCodes
{
    class QRCodeInformation
    {
        //Methodes statiques
        public static int GetLengthArea(int version, Encoding.Types encodingType)
        {
            if (version <= 9)
            {
                if (encodingType == Encoding.Types.Numeric)
                    return 10;
                else if (encodingType == Encoding.Types.Alphanumeric)
                    return 9;
                else
                    return 8;
            }
            else if (version <= 26)
            {
                if (encodingType == Encoding.Types.Numeric)
                    return 12;
                else if (encodingType == Encoding.Types.Alphanumeric)
                    return 11;
                else if (encodingType == Encoding.Types.Binary)
                    return 16;
                else
                    return 10;
            }
            else
            {
                if (encodingType == Encoding.Types.Numeric)
                    return 14;
                else if (encodingType == Encoding.Types.Alphanumeric)
                    return 13;
                else if (encodingType == Encoding.Types.Binary)
                    return 16;
                else
                    return 12;
            }
        }

        //Variables
        public int Version { get; private set; }
        public int Size { get; private set; }
        public Mask Mask { get; private set; }
        public Payload Payload { get; private set; }
        public ErrorCorrection.Entry ErrorCorrection { get; private set; }

        //Constructeurs
        public QRCodeInformation(int version, Payload payload, Mask mask, ErrorCorrection.Levels ecLevel)
        {
            if (version < 1 || version > 40)
                throw new ArgumentOutOfRangeException(nameof(version));
            this.Payload = payload ?? throw new ArgumentNullException(nameof(payload));

            this.ErrorCorrection = QRCodes.ErrorCorrection.GetEntry(version, ecLevel);

            if (ContentArea < Payload.BitCount)
                throw new FormatException("this payload is too big for this version & error correction. Payload size=" + payload.BitCount + ". QRCode V:" + version + "-EC:" + ecLevel + "-E:" + EncodingType + " size=" + ContentArea);

            this.Version = version;
            this.Size = Version * 4 + 17;
            this.Mask = mask;
        }
        public QRCodeInformation(int version)
        {
            if (version < 1 || version > 40)
                throw new ArgumentOutOfRangeException(nameof(version));

            this.Version = version;
            this.Size = Version * 4 + 17;
            this.Mask = null;
            this.ErrorCorrection = null;
            this.Payload = null;
        }

        //Propriete générale
        public Encoding.Types EncodingType { get { return Payload.Encoding; } }
        public bool AutoMask { get { return Mask == null; } }

        //Proprietes aires
        public int LengthArea { get { return GetLengthArea(Version, EncodingType); } }
        public int ContentArea { get { return ErrorCorrection.TotalDataCodewords * 8 - 4 - 4 - LengthArea; } }

        //Proprietes autre
        public int AlignmentPatternPositionLength
        {
            get
            {
                if (Version < 2)
                    throw new ApplicationException("Version 1 do not have alignment patterns.");

                return 2 + Version / 7;
            }
        }
        public int[] AlignmentPatternPosition
        {
            get
            {
                int[] position = new int[AlignmentPatternPositionLength];
                int diff = (Size - 13) / (position.Length - 1);

                for (int i = 0; i < position.Length; i++)
                    position[i] = 6 + i * diff;

                return position;
            }
        }
    }
}
using QRCodes.PayloadTypes;
using MyProject;
using System;

namespace QRCodes
{
    class Payloads
    {
        //Methodes statiques
        public static Payload Decode(bool[] message, int qrCodeVersion)
        {
            Encoding.Types encType = Encoding.FromInt((int)Utils.ReadEndiannessBits(message, 0, 4));
            int lengthArea = QRCodeInformation.GetLengthArea(qrCodeVersion, encType);
            int length = (int)Utils.ReadEndiannessBits(message, 4, lengthArea);

            if (encType == Encoding.Types.Numeric)
                return NumericPayload.Decode(message, 4 + lengthArea, length);
            if (encType == Encoding.Types.Alphanumeric)
                return AlphanumericPayload.Decode(message, 4 + lengthArea, length);
            if (encType == Encoding.Types.Binary)
                return BinaryPayload.Decode(message, 4 + lengthArea, length);
            if (encType == Encoding.Types.Kanji)
                return KanjiPayload.Decode(message, 4 + lengthArea, length);

            throw new ApplicationException("Unknow message encoding.");
        }
        public static Payload Encode(string message)
        {
            if (NumericPayload.IsValid(message))
                return NumericPayload.Encode(message);
            if (AlphanumericPayload.IsValid(message))
                return AlphanumericPayload.Encode(message);
            if (BinaryPayload.IsValid(message))
                return BinaryPayload.Encode(message);
            if (KanjiPayload.IsValid(message))
                return KanjiPayload.Encode(message);

            throw new ApplicationException("Unknow message encoding.");
        }
        public static Payload Encode(string message, Encoding.Types type)
        {
            if (type == Encoding.Types.Numeric)
                return NumericPayload.Encode(message);
            if (type == Encoding.Types.Alphanumeric)
                return AlphanumericPayload.Encode(message);
            if (type == Encoding.Types.Binary)
                return BinaryPayload.Encode(message);
            if (type == Encoding.Types.Kanji)
                return KanjiPayload.Encode(message);

            throw new ApplicationException("Unknow message encoding.");

        }
    }
}

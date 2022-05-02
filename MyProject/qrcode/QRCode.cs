using MyProject;
using System;
using ReedSolomon;
using Bitmap;
using QRCodes.Reader;
using System.Text;
using System.IO;

namespace QRCodes
{
    class QRCode
    {
        //Methodes statiques
        public static Builder NewBuilder()
        {
            return new Builder();
        }
        public static QRCode Create(Payload payload, ErrorCorrection.Levels ecLevel)
        {
            return NewBuilder()
                    .AutoVersion()
                    .AutoMask()
                    .SetPayload(payload)
                    .SetErrorCorrectionLevel(ecLevel)
                    .Build();
        }
        public static QRCode Create(Payload payload)
        {
            return Create(payload, ErrorCorrection.Levels.LOW);
        }
        public static QRCode Create(string message)
        {
            return Create(Payloads.Encode(message));
        }
        public static QRCode Create(string message, ErrorCorrection.Levels ecLevel)
        {
            return Create(Payloads.Encode(message), ecLevel);
        }
        public static QRCode Read(bool[,] area)
        {
            if (area == null)
                throw new ArgumentNullException(nameof(area));

            //Reading from matrice
            int version = ReadVersion(area);
            Format.ECMInfo ecmInfo = ReadECM(area);
            ErrorCorrection.Levels ecLevel = ecmInfo.ECLevel;
            Mask mask = ecmInfo.Mask;            
            QRCode qrcode = new(version);
            Payload payload = ReadPayload(area, qrcode.DataModuleIterator(), ecmInfo);

            //Editing QR Code
            qrcode.Informations = new QRCodeInformation(version, payload, mask, ecLevel);
            WritePayloadData(qrcode);

            //Locking
            foreach (Module m in qrcode.modules)
                m.Lock();

            //Finalizing
            qrcode.Mask(mask);
            return qrcode;
        }
        public static QRCode Read(BitMap image)
        {
            return new QRCodeReader(image).Read();
        }

        //Builder
        public class Builder
        {
            //Variables
            private int version = 0;
            private Mask mask = null;
            private Payload payload = null;
            private ErrorCorrection.Levels errorCorrectionLevel = ErrorCorrection.Levels.LOW;

            //Constructeurs
            internal Builder() { }

            //Methodes
            public QRCode Build()
            {
                if (version == 0)
                    for (int i = 1; i <= 40; i++)
                        try
                        {
                            return new QRCode(i, payload, mask, errorCorrectionLevel);
                        }
                        catch (FormatException e)
                        {
                            if (i == 40)
                                throw new FormatException(e.Message);
                        }
                
                return new QRCode(version, payload, mask, errorCorrectionLevel);
            }
            public Builder AutoMask()
            {
                this.mask = null;
                return this;
            }
            public Builder SetMask(int mask)
            {
                this.mask = new Mask(mask);
                return this;
            }
            public Builder SetMask(Mask mask)
            {
                this.mask = mask;
                return this;
            }
            public Builder SetVersion(int version)
            {
                this.version = version;
                return this;
            }
            public Builder AutoVersion()
            {
                this.version = 0;
                return this;
            }
            public Builder SetErrorCorrectionLevel(ErrorCorrection.Levels ecLevel)
            {
                this.errorCorrectionLevel = ecLevel;
                return this;
            }
            public Builder SetPayload(Payload payload)
            {
                this.payload = payload;
                return this;
            }
            public Builder SetPayload(string message)
            {
                this.payload = Payloads.Encode(message);
                return this;
            }
        }

        //Attributs
        public QRCodeInformation Informations { get; private set; }
        public int Version { get { return Informations.Version; } }
        public int Size { get { return Informations.Size; } }
        public Payload Payload { get { return Informations.Payload; } }
        public Mask AppliedMask { get; private set; }
        public int Penalty { get; private set; }

        private readonly Module[,] modules;

        //Constructeurs
        internal QRCode(int version, Payload payload, Mask mask, ErrorCorrection.Levels ecLevel) : this(new QRCodeInformation(version, payload, mask, ecLevel)) {}
        internal QRCode(QRCodeInformation infos)
        {
            if (infos == null)
                throw new ArgumentNullException(nameof(infos));

            this.Informations = infos;
            this.modules = new Module[Size, Size];

            //Writing modules
            WriteDefaultModules();
            WritePayloadData(this);

            //Applying mask
            if (infos.AutoMask)
                ApplyBestMask();
            else
                Mask(infos.Mask);

            //Locking
            foreach (Module m in modules)
                m.Lock();
        }
        private QRCode(int version)
        {
            this.Informations = new QRCodeInformation(version);
            this.modules = new Module[Size, Size];
            WriteDefaultModules();
        }
        private QRCode(QRCode another)
        {
            this.Informations = another.Informations;
            this.AppliedMask = another.AppliedMask;
            this.Penalty = another.Penalty;

            this.modules = new Module[another.Size, another.Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    this.modules[i, j] = another.modules[i, j].Clone();
        }

        //Read method
        private static int ReadVersion(bool[,] area)
        {
            if (area.GetLength(0) != area.GetLength(1))
                throw new ArgumentException("area is not a squared matrice.");

            int size = area.GetLength(0);
            int version = size - 17;
            if (version % 4 != 0)
                throw new ArgumentException("area do not have a correct size.");
            version /= 4;

            if (version <= 2)
                return version;

            bool[] v1 = new bool[18];
            bool[] v2 = new bool[18];

            int i, j;
            for (int k = 0; k < 18; k++)
            {
                i = k % 3;
                j = k / 3;
                v1[k] = area[size - 9 - i, 5 - j];
                v2[k] = area[5 - j, size - 9 - i];
            }

            int w1 = (int)Utils.ReadEndiannessBits(v1);
            int w2 = (int)Utils.ReadEndiannessBits(v2);
            int version2 = Format.VersionFrom(w1, w2);

            if (version != version2)
                throw new ApplicationException("Incoherrent versions");

            return version;
        }
        private static Format.ECMInfo ReadECM(bool[,] area)
        {
            int size = area.GetLength(0);
            bool[] ecm1 = new bool[15];
            bool[] ecm2 = new bool[15];

            for (int i = 0; i <= 6; i++)
            {
                ecm1[i] = area[8, i == 6 ? 7 : i];
                ecm2[i] = area[size - 1 - i, 8];
            }

            for (int i = 7; i < 15; i++)
            {
                ecm1[i] = area[15 - (i >= 9 ? i + 1 : i), 8];
                ecm2[i] = area[8, size - 15 + i];
            }

            return Format.ECMInfoFrom((short)Utils.ReadEndiannessBits(ecm1), (short)Utils.ReadEndiannessBits(ecm2));
        }
        private static Payload ReadPayload(bool[,] area, ModuleIterator iterator, Format.ECMInfo ecmInfo)
        {
            int version = (area.GetLength(0) - 17) / 4;
            ErrorCorrection.Entry ecEntry = ErrorCorrection.GetEntry(version, ecmInfo.ECLevel);

            byte[][] dataBlocks = ReadPayloadDataBlocks(area, iterator, ecmInfo.Mask, ecEntry);
            byte[][] ecBlocks = ReadPayloadEcBlocks(area, iterator, ecmInfo.Mask, ecEntry);
            bool[] message = CorrectPayloadMessage(dataBlocks, ecBlocks);
            return Payloads.Decode(message, version);
        }
        private static bool[] CorrectPayloadMessage(byte[][] dataBlocks, byte[][] ecBlocks)
        {
            byte[][] corrected = new byte[dataBlocks.Length][];
            int byteCount = 0;

            ReedSolomonDecoder decoder = new(GenericGF.QR_CODE_FIELD_256);

            for (int i = 0; i < corrected.Length; i++)
            {
                corrected[i] = decoder.DecodeEx(dataBlocks[i], ecBlocks[i]);
                byteCount += corrected[i].Length;
            }

            bool[] message = new bool[byteCount * 8];
            int writed = 0;
            for (int i = 0; i < corrected.Length; i++)
                for (int j = 0; j < corrected[i].Length; j++)
                {
                    Utils.WriteEndiannessBits(corrected[i][j], message, writed * 8, 8);
                    writed++;
                }

            return message;
        }
        private static byte[][] ReadPayloadEcBlocks(bool[,] area, ModuleIterator iterator, Mask mask, ErrorCorrection.Entry entry)
        {
            return ReadPayloadBlocks(area, iterator, mask, entry.Group1blocks, entry.Group2blocks, entry.EcCodewordsPerBlock, entry.EcCodewordsPerBlock);
        }
        private static byte[][] ReadPayloadDataBlocks(bool[,] area, ModuleIterator iterator, Mask mask, ErrorCorrection.Entry entry)
        {
            return ReadPayloadBlocks(area, iterator, mask, entry.Group1blocks, entry.Group2blocks, entry.CodewordsPerGroup1Block, entry.CodewordsPerGroup2Block);
        }
        private static byte[][] ReadPayloadBlocks(bool[,] area, ModuleIterator iterator, Mask mask, int g1b, int g2b, int cg1b, int cg2b)
        {
            byte[][] data = new byte[g1b + g2b][];
            int maxIteration = (g1b + g2b) * Math.Max(cg1b, cg2b);

            for (int i = 0; i < data.Length; i++)
                data[i] = new byte[i < g1b ? cg1b : cg2b];

            int block;
            int col;

            for (int i = 0; i < maxIteration; i++)
            {
                block = i % data.Length;
                col = i / data.Length;

                if (col >= data[block].Length)
                    continue;

                data[block][col] = ReadNextCodeword(area, iterator, mask);
            }

            return data;
        }
        private static byte ReadNextCodeword(bool[,] area, ModuleIterator iterator, Mask mask)
        {
            return (byte)Utils.ReadEndiannessBits(Read(area, iterator, mask, 8));
        }
        private static bool[] Read(bool[,] area, ModuleIterator iterator, Mask mask, int count)
        {
            bool[] array = new bool[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = area[iterator.Y, iterator.X];
                if (mask.Apply(iterator.Y, iterator.X))//Unmasking before reading
                    array[i] = !array[i];
                iterator.Next();
            }
            return array;
        }

        //Penalty methods
        private static int CalculatePenalty(QRCode qrcode)
        {
            return Penalty1(qrcode) + Penalty2(qrcode) + Penalty3(qrcode) + Penalty4(qrcode);
        }
        private static int Penalty1(QRCode qrcode)
        {
            int penalty = 0;
            int size = qrcode.Size;

            //Condition 1
            int consecutiv;
            Module.Status last, current;

            //Horizontal
            for (int i = 0; i < size; i++)
            {
                consecutiv = 1;
                last = qrcode.GetModule(i, 0).State;

                for (int j = 1; j < size; j++)
                {
                    current = qrcode.GetModule(i, j).State;

                    if (current == last)
                        consecutiv++;
                    else
                    {
                        if (consecutiv >= 5)
                            penalty += consecutiv - 2;
                        last = current;
                        consecutiv = 1;
                    }
                }

                if (consecutiv >= 5)
                    penalty += consecutiv - 2;
            }

            //Vertical
            for (int j = 0; j < size; j++)
            {
                consecutiv = 1;
                last = qrcode.GetModule(0, j).State;

                for (int i = 1; i < size; i++)
                {
                    current = qrcode.GetModule(i, j).State;

                    if (current == last)
                        consecutiv++;
                    else
                    {
                        if (consecutiv >= 5)
                            penalty += consecutiv - 2;

                        last = current;
                        consecutiv = 1;
                    }
                }

                if (consecutiv >= 5)
                    penalty += consecutiv - 2;
            }

            return penalty;
        }
        private static int Penalty2(QRCode qrcode)
        {
            int dest = qrcode.Size - 1;
            int penalty = 0;

            Module.Status s0, s1, s2, s3;
            for (int i = 0; i < dest; i++)
                for (int j = 0; j < dest; j++)
                {
                    s0 = qrcode.GetModule(i + 0, j + 0).State;
                    s1 = qrcode.GetModule(i + 1, j + 0).State;
                    s2 = qrcode.GetModule(i + 0, j + 1).State;
                    s3 = qrcode.GetModule(i + 1, j + 1).State;
                    if (s0 == s1 && s1 == s2 && s2 == s3)
                        penalty += 3;
                }

            return penalty;
        }
        private static int Penalty3(QRCode qrcode)
        {
            Module.Status w = Module.Status.White;
            Module.Status b = Module.Status.Black;

            Module.Status[] pattern1 = { b, w, b, b, b, w, b, w, w, w, w };
            Module.Status[] pattern2 = { w, w, w, w, b, w, b, b, b, w, b };

            return (PatternCount(qrcode, pattern1) + PatternCount(qrcode, pattern2)) * 40; ;
        }
        private static int Penalty4(QRCode qrcode)
        {
            int darkModules = 0;
            int whiteModules = 0;

            foreach (Module module in qrcode.modules)
                if (module.IsBlack) darkModules++;
                else if (module.IsWhite) whiteModules++;

            int percent = (int)((darkModules*100.0) / (double)(darkModules + whiteModules));
            int previous = percent - percent % 5;
            int next = previous + 5;

            previous = Math.Abs(previous - 50) / 5;
            next = Math.Abs(next - 50) / 5;

            return Math.Min(previous, next) * 10;
        }
        private static int PatternCount(QRCode qrcode, Module.Status[] pattern)
        {
            int size = qrcode.Size;
            int dest = size - pattern.Length;
            int count = 0;

            //Condition 1
            int k;

            for (int i = 0 ; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //Horizontal
                    if (j < dest)
                    {
                        for (k = 0; k < pattern.Length; k++)
                            if (qrcode.GetModule(i, j + k).State != pattern[k])
                                break;

                        if (k == pattern.Length)
                            count++;
                    }
                    
                    //Vertical
                    if (i < dest)
                    {
                        for (k = 0; k < pattern.Length; k++)
                            if (qrcode.GetModule(i + k, j).State != pattern[k])
                                break;

                        if (k == pattern.Length)
                            count++;
                    }
                }
            }

            return count;
        }

        //Write methods
        private void WriteDefaultModules()
        {
            int size = Size;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    SetModule(i, j, Module.Types.Data);

            WriteFinderPatterns(this);
            WriteSeparators(this);
            WriteVersionInformation(this);
            WriteTiming(this);
            WriteAlignments(this);
            WriteDarkModule(this);
            WriteFormatInformation(this);
        }
        private static void WriteFinderPatterns(QRCode qrcode)
        {
            WriteFinderPattern(qrcode, 0, 0);
            WriteFinderPattern(qrcode, qrcode.Size - 7, 0);
            WriteFinderPattern(qrcode, 0, qrcode.Size - 7);
        }
        private static void WriteFinderPattern(QRCode qrcode, int y, int x)
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                    if (i == 0 || j == 0 || i == 6 || j == 6)
                        qrcode.SetModule(i + y, j + x, Module.Types.FinderPattern, Module.Status.Black);
                    else if (i >= 2 && i <= 4 && j >= 2 && j <= 4)
                        qrcode.SetModule(i + y, j + x, Module.Types.FinderPattern, Module.Status.Black);
                    else
                        qrcode.SetModule(i + y, j + x, Module.Types.FinderPattern, Module.Status.White);
        }
        private static void WriteSeparators(QRCode qrcode)
        {
            int size = qrcode.Size;
            for (int i = 0; i < 8; i++)
            {
                //Top left
                qrcode.SetModule(7, i, Module.Types.Separators, Module.Status.White);
                qrcode.SetModule(i, 7, Module.Types.Separators, Module.Status.White);
                //Top Right
                qrcode.SetModule(7, size - 8 + i, Module.Types.Separators, Module.Status.White);
                qrcode.SetModule(i, size - 8, Module.Types.Separators, Module.Status.White);
                //Bottom left
                qrcode.SetModule(size - 8, i, Module.Types.Separators, Module.Status.White);
                qrcode.SetModule(size - 8 + i, 7, Module.Types.Separators, Module.Status.White);
            }
        }
        private static void WriteVersionInformation(QRCode qrcode)
        {

            if (qrcode.Version < 7)
                return;

            int size = qrcode.Size;
            bool[] data = Utils.GetEndiannessBits(Format.GetVersionInformation((byte)qrcode.Version), 18);
            Module.Status state;

            int i, j;
            for (int index = 0; index < data.Length; index++)
            {
                i = index % 3;
                j = index / 3;
                state = data[index] ? Module.Status.Black : Module.Status.White;

                //First (left)
                qrcode.SetModule(size - 9 - i, 5 - j, Module.Types.VersionInformation, state);
                //Second (right)
                qrcode.SetModule(5 - j, size - 9 - i, Module.Types.VersionInformation, state);
            }
        }
        private static void WriteFormatInformation(QRCode qrcode)
        {
            int size = qrcode.Size;
            ErrorCorrection.Levels ecLevel = qrcode.Informations.ErrorCorrection.Level;
            Mask mask = qrcode.AppliedMask;
            Module.Status[] data = new Module.Status[15]!;

            if (mask == null)
                for (int i = 0; i < 15; i++)
                    data[i] = Module.Status.Undefined;
            else {
                bool[] bit = Utils.GetEndiannessBits(Format.GetFormatInformation(ecLevel, mask), 15);
                for (int i = 0; i < 15; i++)
                    data[i] = bit[i] ? Module.Status.Black : Module.Status.White;
            }
 
            for (int i = 0; i <= 6; i++)
            {
                qrcode.SetModule(8, i == 6 ? 7 : i, Module.Types.FormatInformation, data[i]);
                qrcode.SetModule(size - 1 - i , 8, Module.Types.FormatInformation, data[i]);
            }

            for (int i = 7; i < 15; i++)
            {
                qrcode.SetModule(15 - (i >= 9 ? i + 1 : i), 8, Module.Types.FormatInformation, data[i]);
                qrcode.SetModule(8, size - 15 + i, Module.Types.FormatInformation, data[i]);
            }
        }
        private static void WriteTiming(QRCode qrcode)
        {
            int size = qrcode.Size;
            for (int i = 8; i < size - 8; i++)
            {
                qrcode.SetModule(6, i, Module.Types.TimingPattern, i % 2 == 1 ? Module.Status.White : Module.Status.Black);
                qrcode.SetModule(size - 1 - i, 6, Module.Types.TimingPattern, i % 2 == 1 ? Module.Status.White : Module.Status.Black);
            }
        }
        private static void WriteAlignments(QRCode qrcode)
        {
            if (qrcode.Version < 2)
                return;

            int[] positions = qrcode.Informations.AlignmentPatternPosition;
            for (int i = 0; i < positions.Length; i++)
                for (int j = 0; j < positions.Length; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (i == 0 && j == positions.Length - 1)
                        continue;
                    if (j == 0 && i == positions.Length - 1)
                        continue;
                    WriteAlignment(qrcode, positions[i], positions[j]);
                }
        }
        private static void WriteAlignment(QRCode qrcode, int y, int x)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (i == 0 || j == 0 || i == 4 || j == 4)
                        qrcode.SetModule(i + y - 2, j + x - 2, Module.Types.AlignmentPattern, Module.Status.Black);
                    else if (i == 2 && j == 2)
                        qrcode.SetModule(i + y - 2, j + x - 2, Module.Types.AlignmentPattern, Module.Status.Black);
                    else
                        qrcode.SetModule(i + y - 2, j + x - 2, Module.Types.AlignmentPattern, Module.Status.White);
        }
        private static void WriteDarkModule(QRCode qrcode)
        {
            qrcode.SetModule(qrcode.Size - 8, 8, Module.Types.DarkModule, Module.Status.Black);
        }
        private static void WritePayloadData(QRCode qrcode)
        {
            byte[][] blockData = GeneratePayloadBlockData(qrcode.Informations);
            byte[][] ecData = GeneratePayloadEcData(qrcode.Informations, blockData);

            ModuleIterator iterator = qrcode.DataModuleIterator();
            Write(iterator, blockData);
            Write(iterator, ecData);

            while (iterator.HasNext())//End padding
            {
                iterator.Current.State = Module.Status.White;
                iterator.Next();
            }
        }
        private static bool[] GeneratePayloadRawData(QRCodeInformation informations)
        {
            Payload payload = informations.Payload;
            bool[] data = new bool[informations.ErrorCorrection.TotalDataCodewords * 8];
            int writed = 0; 

            writed += Utils.WriteEndiannessBits(Encoding.ToInt(informations.EncodingType), data, 0, 4);
            writed += Utils.WriteEndiannessBits(payload.Length, data, writed, informations.LengthArea);
            writed += Utils.Write(payload.Data, data, writed);
            writed += Utils.Write(new bool[4], data, writed);

            int padding = (8 - writed % 8) % 8;
            writed += Utils.Write(new bool[padding], data, writed);

            int codewordsRemaining = informations.ErrorCorrection.TotalDataCodewords - writed / 8;
            for (int i = 0; i < codewordsRemaining; i++)
                if (i % 2 == 0)
                    writed += Utils.WriteEndiannessBits(236, data, writed, 8);
                else
                    writed += Utils.WriteEndiannessBits(17, data, writed, 8);

            return data;
        }
        private static byte[][] GeneratePayloadBlockData(QRCodeInformation information)
        {
            ErrorCorrection.Entry entry = information.ErrorCorrection;

            bool[] rawData = GeneratePayloadRawData(information);
            byte[][] blockData = new byte[entry.Group1blocks + entry.Group2blocks][];

            int readed = 0;
            for (int i = 0; i < blockData.Length; i++)
            {
                blockData[i] = new byte[i < entry.Group1blocks ? entry.CodewordsPerGroup1Block : entry.CodewordsPerGroup2Block];
                for (int j = 0; j < blockData[i].Length; j++)
                {
                    blockData[i][j] = (byte)Utils.ReadEndiannessBits(rawData, readed, 8);
                    readed += 8;
                }
            }

            return blockData;
        }
        private static byte[][] GeneratePayloadEcData(QRCodeInformation information, byte[][] blockData)
        {
            byte[][] ecData = new byte[blockData.Length][];
            int ecCodewordPerBlock = information.ErrorCorrection.EcCodewordsPerBlock;
            int[] toEncode;
            ReedSolomonEncoder encoder = new(GenericGF.QR_CODE_FIELD_256);

            for (int i = 0; i < ecData.Length; i++)
            {
                toEncode = new int[ecCodewordPerBlock + blockData[i].Length];
                for (int j = 0; j < blockData[i].Length; j++)
                    toEncode[j] = blockData[i][j] & 0xFF;

                encoder.Encode(toEncode, ecCodewordPerBlock);
                ecData[i] = new byte[ecCodewordPerBlock];
                for (int j = 0; j < ecCodewordPerBlock; j++)
                    ecData[i][j] = (byte)toEncode[blockData[i].Length + j];
            }

            return ecData;
        }
        private static void Write(ModuleIterator iterator, byte[][] array)
        {
            int maxLength = 0;
            
            //Getting maxLength
            for (int i = 0; i < array.Length; i++)
                if (array[i].Length > maxLength)
                    maxLength = array[i].Length;

            //Writing
            for (int i = 0; i < maxLength; i++)
                for (int j = 0; j < array.Length; j++)
                    if (i < array[j].Length)
                        Write(iterator, array[j][i]);
        }
        private static void Write(ModuleIterator iterator, byte b)
        {
            foreach (bool e in Utils.GetEndiannessBits(b, 8))
            {
                iterator.Current.State = e ? Module.Status.Black : Module.Status.White;
                iterator.Next();
            }
        }

        //Mask method
        private void ForceMask(Mask mask)
        {
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            
            int size = Size;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (modules[i, j].IsData && mask.Apply(i, j))
                        if (modules[i, j].Locked)
                        {
                            modules[i, j] = modules[i, j].Clone(false);
                            modules[i, j].Switch();
                            modules[i, j].Lock();
                        }
                        else
                            modules[i, j].Switch();
        }
        private void Unmask()
        {
            if (!IsMasked())
                throw new ApplicationException("This QR code is already unmasked.");

            ForceMask(AppliedMask);
            Penalty = 0;
            AppliedMask = null;
            WriteFormatInformation(this);
        }
        private void Mask(Mask mask)
        {
            if (IsMasked())
                throw new ApplicationException("This QR Code is already masked.");

            ForceMask(mask);
            Penalty = CalculatePenalty(this);
            AppliedMask = mask;
            WriteFormatInformation(this);
        }
        private void ApplyBestMask()
        {
            if (IsMasked())
                throw new ApplicationException("this QR Code is already masked.");

            int bestPenalty = int.MaxValue;
            Mask bestMask = null;

            for (int i = 0b000; i <= 0b111; i++)
            {
                Mask(new Mask(i));

                if (Penalty < bestPenalty)
                {
                    bestPenalty = Penalty;
                    bestMask = AppliedMask;
                }

                Unmask();
            }

            Mask(bestMask);
        }

        //Methodes
        public QRCode Clone()
        {
            return new QRCode(this);
        }
        private void SetModule(int y, int x, Module.Types type, Module.Status state)
        {
            modules[y, x] = new Module(type, state);
        }
        private void SetModule(int y, int x, Module.Types type)
        {
            modules[y, x] = new Module(type);
        }
        public Module GetModule(int y, int x)
        {
            return modules[y, x];
        }
        public BitMap ToBitMap(int margin)
        {
            int size = Size;
            BitMap image = new(size + 2 * margin, size + 2 * margin);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    image.SetPixel(i + margin, j + margin, modules[i, j].Color);
            return image;
        }
        public BitMap ToBitMap()
        {
            return ToBitMap(Size / 10);
        }
        public ModuleIterator DataModuleIterator()
        {
            return new ModuleIterator(this);
        }
        public bool IsMasked()
        {
            return AppliedMask != null;
        }
    }
}
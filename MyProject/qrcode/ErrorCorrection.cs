using System;

namespace QRCodes
{
    public class ErrorCorrection
    {
        //Enum
        public enum Levels
        {
            LOW,
            MEDIUM,
            QUARTIL,
            HIGH
        }

        //Methodes
        public static Levels FromInt(int data)
        {
            if (data == 0b01)
                return Levels.LOW;
            if (data == 0b00)
                return Levels.MEDIUM;
            if (data == 0b11)
                return Levels.QUARTIL;
            if (data == 0b10)
                return Levels.HIGH;
            throw new ArgumentException("unknow error correction.");
        }
        public static int ToInt(Levels e)
        {
            if (e == Levels.LOW)
                return 0b01;
            if (e == Levels.MEDIUM)
                return 0b00;
            if (e == Levels.QUARTIL)
                return 0b11;
            if (e == Levels.HIGH)
                return 0b10;
            throw new ArgumentException("unknow error correction.");
        }
        public static int Percent(Levels e)
        {
            if (e == Levels.LOW)
                return 7;
            if (e == Levels.MEDIUM)
                return 15;
            if (e == Levels.QUARTIL)
                return 25;
            if (e == Levels.HIGH)
                return 30;
            throw new ArgumentException("unknow error correction.");
        }
        public static Levels FromPercent(int data)
        {
            if (data == 7)
                return Levels.LOW;
            if (data == 15)
                return Levels.MEDIUM;
            if (data == 25)
                return Levels.QUARTIL;
            if (data == 30)
                return Levels.HIGH;
            throw new ArgumentException("unknow error correction.");
        }

        //Entry
        internal static Entry GetEntry(int version, Levels level)
        {
            if (level == Levels.LOW)
                return LowEntries[version - 1];
            if (level == Levels.MEDIUM)
                return MediumEntries[version - 1];
            if (level == Levels.QUARTIL)
                return QuartilEntries[version - 1];
            if (level == Levels.HIGH)
                return HighEntries[version - 1];
            throw new ArgumentException("unknow level " + level);
        }

        internal class Entry
        {
            public int Version { get; private set; }
            public Levels Level { get; private set; }
            public int EcCodewordsPerBlock { get; private set; }
            public int Group1blocks { get; private set; }
            public int Group2blocks { get; private set; }
            public int CodewordsPerGroup1Block { get; private set; }
            public int CodewordsPerGroup2Block { get; private set; }
            public int TotalDataCodewords { get; private set; }

            internal Entry(int version, Levels level, int ecCodewordsPerBlock, int group1blocks, int codewordsPerGroup1Block, int group2blocks, int codewordsPerGroup2Block)
            {
                this.Version = version;
                this.Level = level;
                this.EcCodewordsPerBlock = ecCodewordsPerBlock;
                this.Group1blocks = group1blocks;
                this.Group2blocks = group2blocks;
                this.CodewordsPerGroup1Block = codewordsPerGroup1Block;
                this.CodewordsPerGroup2Block = codewordsPerGroup2Block;
                this.TotalDataCodewords = group1blocks * codewordsPerGroup1Block + group2blocks * codewordsPerGroup2Block;
            }

            internal Entry(int version, Levels level, int ecCodewordsPerBlock, int group1blocks, int codewordsPerGroup1Block) : this(version, level, ecCodewordsPerBlock, group1blocks, codewordsPerGroup1Block, 0, 0) { }
        }

        private readonly static Entry[] LowEntries = new Entry[]
        {
            new Entry(1, Levels.LOW, 7, 1, 19),
            new Entry(2, Levels.LOW, 10, 1, 34),
            new Entry(3, Levels.LOW, 15, 1, 55),
            new Entry(4, Levels.LOW, 20, 1, 80),
            new Entry(5, Levels.LOW, 26, 1, 108),
            new Entry(6, Levels.LOW, 18, 2, 68),
            new Entry(7, Levels.LOW, 20, 2, 78),
            new Entry(8, Levels.LOW, 24, 2, 97),
            new Entry(9, Levels.LOW, 30, 2, 116),
            new Entry(10, Levels.LOW, 18, 2, 68, 2, 69),
            new Entry(11, Levels.LOW, 20, 4, 81),
            new Entry(12, Levels.LOW, 24, 2, 92, 2, 93),
            new Entry(13, Levels.LOW, 26, 4, 107),
            new Entry(14, Levels.LOW, 30, 3, 115, 1, 116),
            new Entry(15, Levels.LOW, 22, 5, 87, 1, 88),
            new Entry(16, Levels.LOW, 24, 5, 98, 1, 99),
            new Entry(17, Levels.LOW, 28, 1, 107, 5, 108),
            new Entry(18, Levels.LOW, 30, 5, 120, 1, 121),
            new Entry(19, Levels.LOW, 28, 3, 113, 4, 114),
            new Entry(20, Levels.LOW, 28, 3, 107, 5, 108),
            new Entry(21, Levels.LOW, 28, 4, 116, 4, 117),
            new Entry(22, Levels.LOW, 28, 2, 111, 7, 112),
            new Entry(23, Levels.LOW, 30, 4, 121, 5, 122),
            new Entry(24, Levels.LOW, 30, 6, 117, 4, 118),
            new Entry(25, Levels.LOW, 26, 8, 106, 4, 107),
            new Entry(26, Levels.LOW, 28, 10, 114, 2, 115),
            new Entry(27, Levels.LOW, 30, 8, 122, 4, 123),
            new Entry(28, Levels.LOW, 30, 3, 117, 10, 118),
            new Entry(29, Levels.LOW, 30, 7, 116, 7, 117),
            new Entry(30, Levels.LOW, 30, 5, 115, 10, 116),
            new Entry(31, Levels.LOW, 30, 13, 115, 3, 116),
            new Entry(32, Levels.LOW, 30, 17, 115),
            new Entry(33, Levels.LOW, 30, 17, 115, 1, 116),
            new Entry(34, Levels.LOW, 30, 13, 115, 6, 116),
            new Entry(35, Levels.LOW, 30, 12, 121, 7, 122),
            new Entry(36, Levels.LOW, 30, 6, 121, 14, 122),
            new Entry(37, Levels.LOW, 30, 17, 122, 4, 123),
            new Entry(38, Levels.LOW, 30, 4, 122, 18, 123),
            new Entry(39, Levels.LOW, 30, 20, 117, 4, 118),
            new Entry(40, Levels.LOW, 30, 19, 118, 6, 119),
        };

        private readonly static Entry[] MediumEntries = new Entry[]
        {
            new Entry(1, Levels.MEDIUM, 10, 1, 16),
            new Entry(2, Levels.MEDIUM, 16, 1, 28),
            new Entry(3, Levels.MEDIUM, 26, 1, 44),
            new Entry(4, Levels.MEDIUM, 18, 2, 32),
            new Entry(5, Levels.MEDIUM, 24, 2, 43),
            new Entry(6, Levels.MEDIUM, 16, 4, 27),
            new Entry(7, Levels.MEDIUM, 18, 4, 31),
            new Entry(8, Levels.MEDIUM, 22, 2, 38, 2, 39),
            new Entry(9, Levels.MEDIUM, 22, 3, 36, 2, 37),
            new Entry(10, Levels.MEDIUM, 26, 4, 43, 1, 44),
            new Entry(11, Levels.MEDIUM, 30, 1, 50, 4, 51),
            new Entry(12, Levels.MEDIUM, 22, 6, 36, 2, 37),
            new Entry(13, Levels.MEDIUM, 22, 8, 37, 1, 38),
            new Entry(14, Levels.MEDIUM, 24, 4, 40, 5, 41),
            new Entry(15, Levels.MEDIUM, 24, 5, 41, 5, 42),
            new Entry(16, Levels.MEDIUM, 28, 7, 45, 3, 46),
            new Entry(17, Levels.MEDIUM, 28, 10, 46, 1, 47),
            new Entry(18, Levels.MEDIUM, 26, 9, 43, 4, 44),
            new Entry(19, Levels.MEDIUM, 26, 3, 44, 11, 45),
            new Entry(20, Levels.MEDIUM, 26, 3, 41, 13, 42),
            new Entry(21, Levels.MEDIUM, 26, 17, 42),
            new Entry(22, Levels.MEDIUM, 28, 17, 46),
            new Entry(23, Levels.MEDIUM, 28, 4, 47, 14, 48),
            new Entry(24, Levels.MEDIUM, 28, 6, 45, 14, 46),
            new Entry(25, Levels.MEDIUM, 28, 8, 47, 13, 48),
            new Entry(26, Levels.MEDIUM, 28, 19, 46, 4, 47),
            new Entry(27, Levels.MEDIUM, 28, 22, 45, 3, 46),
            new Entry(28, Levels.MEDIUM, 28, 3, 45, 23, 46),
            new Entry(29, Levels.MEDIUM, 28, 21, 45, 7, 46),
            new Entry(30, Levels.MEDIUM, 28, 19, 47, 10, 48),
            new Entry(31, Levels.MEDIUM, 28, 2, 46, 29, 47),
            new Entry(32, Levels.MEDIUM, 28, 10, 46, 23, 47),
            new Entry(33, Levels.MEDIUM, 28, 14, 46, 21, 47),
            new Entry(34, Levels.MEDIUM, 28, 14, 46, 23, 47),
            new Entry(35, Levels.MEDIUM, 28, 12, 47, 26, 48),
            new Entry(36, Levels.MEDIUM, 28, 6, 47, 34, 48),
            new Entry(37, Levels.MEDIUM, 28, 29, 46, 14, 47),
            new Entry(38, Levels.MEDIUM, 28, 13, 46, 32, 47),
            new Entry(39, Levels.MEDIUM, 28, 40, 47, 7, 48),
            new Entry(40, Levels.MEDIUM, 28, 18, 47, 31, 48),
        };

        private readonly static Entry[] QuartilEntries = new Entry[]
        {
            new Entry(1, Levels.QUARTIL, 13, 1, 13),
            new Entry(2, Levels.QUARTIL, 22, 1, 22),
            new Entry(3, Levels.QUARTIL, 18, 2, 17),
            new Entry(4, Levels.QUARTIL, 26, 2, 24),
            new Entry(5, Levels.QUARTIL, 18, 2, 15, 2, 16),
            new Entry(6, Levels.QUARTIL, 24, 4, 19),
            new Entry(7, Levels.QUARTIL, 18, 2, 14, 4, 15),
            new Entry(8, Levels.QUARTIL, 22, 4, 18, 2, 19),
            new Entry(9, Levels.QUARTIL, 20, 4, 16, 4, 17),
            new Entry(10, Levels.QUARTIL, 24, 6, 19, 2, 20),
            new Entry(11, Levels.QUARTIL, 28, 4, 22, 4, 23),
            new Entry(12, Levels.QUARTIL, 26, 4, 20, 6, 21),
            new Entry(13, Levels.QUARTIL, 24, 8, 20, 4, 21),
            new Entry(14, Levels.QUARTIL, 20, 11, 16, 5, 17),
            new Entry(15, Levels.QUARTIL, 30, 5, 24, 7, 25),
            new Entry(16, Levels.QUARTIL, 24, 15, 19, 2, 20),
            new Entry(17, Levels.QUARTIL, 28, 1, 22, 15, 23),
            new Entry(18, Levels.QUARTIL, 28, 17, 22, 1, 23),
            new Entry(19, Levels.QUARTIL, 26, 17, 21, 4, 22),
            new Entry(20, Levels.QUARTIL, 30, 15, 24, 5, 25),
            new Entry(21, Levels.QUARTIL, 28, 17, 22, 6, 23),
            new Entry(22, Levels.QUARTIL, 30, 7, 24, 16, 25),
            new Entry(23, Levels.QUARTIL, 30, 11, 24, 14, 25),
            new Entry(24, Levels.QUARTIL, 30, 11, 24, 16, 25),
            new Entry(25, Levels.QUARTIL, 30, 7, 24, 22, 25),
            new Entry(26, Levels.QUARTIL, 28, 28, 22, 6, 23),
            new Entry(27, Levels.QUARTIL, 30, 8, 23, 26, 24),
            new Entry(28, Levels.QUARTIL, 30, 4, 24, 31, 25),
            new Entry(29, Levels.QUARTIL, 30, 1, 23, 37, 24),
            new Entry(30, Levels.QUARTIL, 30, 15, 24, 25, 25),
            new Entry(31, Levels.QUARTIL, 30, 42, 24, 1, 25),
            new Entry(32, Levels.QUARTIL, 30, 10, 24, 35, 25),
            new Entry(33, Levels.QUARTIL, 30, 29, 24, 19, 25),
            new Entry(34, Levels.QUARTIL, 30, 44, 24, 7, 25),
            new Entry(35, Levels.QUARTIL, 30, 39, 24, 14, 25),
            new Entry(36, Levels.QUARTIL, 30, 46, 24, 10, 25),
            new Entry(37, Levels.QUARTIL, 30, 49, 24, 10, 25),
            new Entry(38, Levels.QUARTIL, 30, 48, 24, 14, 25),
            new Entry(39, Levels.QUARTIL, 30, 43, 24, 22, 25),
            new Entry(40, Levels.QUARTIL, 30, 34, 24, 34, 25),
        };

        private readonly static Entry[] HighEntries = new Entry[]
        {
            new Entry(1, Levels.HIGH, 17, 1, 9),
            new Entry(2, Levels.HIGH, 28, 1, 16),
            new Entry(3, Levels.HIGH, 22, 2, 13),
            new Entry(4, Levels.HIGH, 16, 4, 9),
            new Entry(5, Levels.HIGH, 22, 2, 11, 2, 12),
            new Entry(6, Levels.HIGH, 28, 4, 15),
            new Entry(7, Levels.HIGH, 26, 4, 13, 1, 14),
            new Entry(8, Levels.HIGH, 26, 4, 14, 2, 15),
            new Entry(9, Levels.HIGH, 24, 4, 12, 4, 13),
            new Entry(10, Levels.HIGH, 28, 6, 15, 2, 16),
            new Entry(11, Levels.HIGH, 24, 3, 12, 8, 13),
            new Entry(12, Levels.HIGH, 28, 7, 14, 4, 15),
            new Entry(13, Levels.HIGH, 22, 12, 11, 4, 12),
            new Entry(14, Levels.HIGH, 24, 11, 12, 5, 13),
            new Entry(15, Levels.HIGH, 24, 11, 12, 7, 13),
            new Entry(16, Levels.HIGH, 30, 3, 15, 13, 16),
            new Entry(17, Levels.HIGH, 28, 2, 14, 17, 15),
            new Entry(18, Levels.HIGH, 28, 2, 14, 19, 15),
            new Entry(19, Levels.HIGH, 26, 9, 13, 16, 14),
            new Entry(20, Levels.HIGH, 28, 15, 15, 10, 16),
            new Entry(21, Levels.HIGH, 30, 19, 16, 6, 17),
            new Entry(22, Levels.HIGH, 24, 34, 13),
            new Entry(23, Levels.HIGH, 30, 16, 15, 14, 16),
            new Entry(24, Levels.HIGH, 30, 30, 16, 2, 17),
            new Entry(25, Levels.HIGH, 30, 22, 15, 13, 16),
            new Entry(26, Levels.HIGH, 30, 33, 16, 4, 17),
            new Entry(27, Levels.HIGH, 30, 12, 15, 28, 16),
            new Entry(28, Levels.HIGH, 30, 11, 15, 31, 16),
            new Entry(29, Levels.HIGH, 30, 19, 15, 26, 16),
            new Entry(30, Levels.HIGH, 30, 23, 15, 25, 16),
            new Entry(31, Levels.HIGH, 30, 23, 15, 28, 16),
            new Entry(32, Levels.HIGH, 30, 19, 15, 35, 16),
            new Entry(33, Levels.HIGH, 30, 11, 15, 46, 16),
            new Entry(34, Levels.HIGH, 30, 59, 16, 1, 17),
            new Entry(35, Levels.HIGH, 30, 22, 15, 41, 16),
            new Entry(36, Levels.HIGH, 30, 2, 15, 64, 16),
            new Entry(37, Levels.HIGH, 30, 24, 15, 46, 16),
            new Entry(38, Levels.HIGH, 30, 42, 15, 32, 16),
            new Entry(39, Levels.HIGH, 30, 10, 15, 67, 16),
            new Entry(40, Levels.HIGH, 30, 20, 15, 61, 16),
        };
    }
}

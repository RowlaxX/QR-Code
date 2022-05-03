using Bitmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodes.Reader
{
    internal class FinderPatternFinder
    {
        //Variables
        private BitMap binarized;
        private bool scanned = false;

        private List<Shape> patterns = new List<Shape>();
        private List<Shape> potential = new List<Shape>();

        //Constructeurs
        internal FinderPatternFinder(BitMap binarized)
        {
            this.binarized = binarized;
        }

        //Methodes
        public void Scan()
        {
            if (scanned)
                return;

            for (int i = 0; i < binarized.Height; i++)
                ScanLine(i);
        }

        public List<Shape> getFinderPatterns()
        {
            if (!scanned)
                Scan();

            CheckPotential();
            if (patterns.Count != 3)
                throw new ApplicationException(patterns.Count + " finder pattern has been found. Need 3");

            return patterns;
        }

        private void CheckPotential()
        {
            bool correct;
            foreach(Shape pattern in potential)
            {
                correct = true;
                pattern.SetEstimatedSize(7);
                for (int i = 0; i < 7 && correct; i++)
                    for (int j = 0; j < 7 && correct; j++)
                        if (i == 0 || i == 6 || j == 0 || j == 6)
                            correct = pattern.IsBlack(i, j);
                        else if (i >= 2 && i <= 4 && j >= 2 && j <= 4)
                            correct = pattern.IsBlack(i, j);
                        else
                            correct = !pattern.IsBlack(i, j);
                
                if (correct)
                    patterns.Add(pattern);
            }
        }

        private void ScanLine(int line)
        {
            
        }

    }
}

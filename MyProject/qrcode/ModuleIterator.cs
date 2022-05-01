using System;

namespace QRCodes
{
    class ModuleIterator
    {
        //Variables
        private readonly QRCode qrcode;
        private bool IsLeft { 
            get 
            {
                if (X < 6)
                    return X % 2 == 0;
                else if (X == 6)
                    throw new ApplicationException();
                else
                    return X % 2 == 1; 
            }
        }
        public int Count { get; private set; } = 0;
        public int X { get; private set; }
        public int Y { get; private set; }
        public Module Current { get { return qrcode.GetModule(Y, X); } }
        public bool GoingUp { get; private set; } = true;
        public QRCode QRCode { get { return qrcode; } }

        //Constructeurs
        public ModuleIterator(QRCode qrcode)
        {
            this.qrcode = qrcode ?? throw new ArgumentNullException(nameof(qrcode));
            this.X = qrcode.Size - 1;
            this.Y = qrcode.Size - 1;
        }

        //Methodes
        private void GoLeft()
        {
            if (X == 7)
                X -= 1;
            X -= 1;
        }
        private void GoRight()
        {
            if (X == 5)
                X += 1;
            X += 1;
        }
        private bool AtBound(int i)
        {
            if (GoingUp && i == 0)
                return true;
            if (!GoingUp && i == qrcode.Size - 1)
                return true;
            return false;
        }
        public bool HasNext()
        {
            return X >= 0;
        }
        public Module Next()
        {
            try
            {
                return Current;
            }
            finally
            {
                do
                {
                    NextPos();
                } while (HasNext() && Current.Type != Module.Types.Data);
                Count++;
            }
        }
        private void NextPos()
        {
            if (IsLeft)
                if (AtBound(Y))
                {
                    GoLeft();
                    GoingUp = !GoingUp;
                }
                else
                {
                    GoRight();
                    Y += GoingUp ? -1 : 1;
                }
            else
                GoLeft();
        }
    }
}
using System;
using Bitmap;

namespace QRCodes
{
    class Module
    {
        //Enum
        public enum Types
        {
            FinderPattern,
            Separators,
            AlignmentPattern,
            TimingPattern,
            DarkModule,
            FormatInformation,
            VersionInformation,
            Data,
        }
        public enum Status
        {
            Undefined = 0,
            Black = 1,
            White = 2
        }

        //Methodes statiques
        public static bool IsPermanent(Types type)
        {
            return !(type == Types.Data || type == Types.FormatInformation);
        }

        //Attributs
        private Status state = Status.Undefined;
        public Types Type { get; private set; }
        public Status State
        {
            get
            {
                return state;
            }
            set
            {
                if (Permanent)
                    throw new ApplicationException("Cannot modify a permanent module.");
                if (Locked)
                    throw new ApplicationException("This module is locked.");
                this.state = value;
            }
        }
        public bool Permanent { get { return IsPermanent(Type); } }
        public Color Color 
        { 
            get 
            {
                if (state == Status.Undefined)
                    return Type == Types.Data ? Colors.LIGHT_GRAY : Colors.GRAY;
                else if (state == Status.Black)
                    return Colors.BLACK;
                else if (state == Status.White)
                    return Colors.WHITE;
                throw new ApplicationException();//Should not be thrown
            } 
        }
        public bool Locked { get; private set; } = false;
        public bool IsBlack { get { return State == Status.Black; } }
        public bool IsWhite { get { return State == Status.White; } }
        public bool IsData { get { return Type == Types.Data; } }

        //Constructeurs
        public Module(Types type, Status state)
        {
            if (IsPermanent(type) && state == Status.Undefined)
                throw new ArgumentException("Type " + type + " is a permament type so state must be specified.");
            this.Type = type;
            this.state = state;
        }
        public Module(Types type) : this(type, Status.Undefined) { }
        public Module(Module another, bool locked)
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));

            this.Type = another.Type;
            this.state = another.State;
            this.Locked = locked;
        }
        
        //Methodes
        public Module Clone(bool locked)
        {
            return new Module(this, locked);
        }
        public Module Clone()
        {
            return new Module(this, this.Locked);
        }
        public void Lock()
        {
            this.Locked = true;
        }
        public void Switch()
        {
            if (State == Status.White)
                State = Status.Black;
            else if (State == Status.Black)
                State = Status.White;
        }
    }
}
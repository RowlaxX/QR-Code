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
            Enabled = 1,
            Disabled = 2
        }

        //Methodes statiques
        public static bool IsPermanent(Types type)
        {
            return !(type == Types.Data || type == Types.FormatInformation);
        }

        //Attributs
        private Status state = 0;
        public Types Type { get; private set; } = 0;
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
                if (state == 0)
                    return Colors.RED;
                else if (state == Status.Enabled)
                    return Colors.BLACK;
                else if (state == Status.Disabled)
                    return Colors.WHITE;
                throw new ApplicationException();//Should not be thrown
            } 
        }
        public bool Locked { get; private set; } = false;
        public bool IsBlack { get { return State == Status.Enabled; } }
        public bool IsWhite { get { return State == Status.Disabled; } }
        public bool IsData { get { return Type == Types.Data; } }

        //Constructeurs
        public Module(Types type, Status state)
        {
            if (IsPermanent(type) && state == 0)
                throw new ArgumentException("Type " + type + " is a permament type so state must be specified.");
            this.Type = type;
            this.state = state;
        }
        public Module(Types type) : this(type, 0) { }
        public Module() : this(Types.Data, 0) { }
        public Module(Module another, bool locked)
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));

            this.Type = another.Type;
            this.State = another.State;
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
            if (State == Status.Disabled)
                State = Status.Enabled;
            else if (State == Status.Enabled)
                State = Status.Disabled;
        }
    }
}
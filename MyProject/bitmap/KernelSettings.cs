namespace Bitmap
{
    class KernelSettings
    {
        //Border enum
        public enum Borders { Extend, Wrap, Crop }

        //Variales
        public Borders Border { get; set; } = Borders.Extend;
        public bool RedChannel { get; set; } = true;
        public bool BlueChannel { get; set; } = true;
        public bool GreenChannel { get; set; } = true;

        //Methodes
        public KernelSettings Clone()
        {
            KernelSettings copy = new KernelSettings();
            copy.Border = this.Border;
            copy.RedChannel = this.RedChannel;
            copy.BlueChannel = this.BlueChannel;
            copy.GreenChannel = this.GreenChannel;
            return copy;
        }
    }
}

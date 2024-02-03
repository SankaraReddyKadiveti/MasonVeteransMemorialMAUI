using System;

using SkiaSharp;

namespace MasonVeteransMemorial.Transforms
{
    class TouchManipulationInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}

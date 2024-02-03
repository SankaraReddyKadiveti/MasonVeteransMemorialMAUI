using System;
using System.Collections.Generic;
using SkiaSharp;

namespace MasonVeteransMemorial.Controls.SkiaSharp
{
    public static class SkiaSharpCommon
    {
        public static SKColor FromHex(this SKColor color, string hex)
        {
            SKColor colorOut;
            SKColor.TryParse(hex, out colorOut);
            return colorOut;
        }

        public class MapArea
        {
            public float ScaleX { get; set; }
            public float ScaleY { get; set; }
            public float TransX { get; set; }
            public float TransY { get; set; }
        }

        public class BrickPalate
        {
            public int MaxLayerCount { get; set; }
            public int MaxLayerBrickCount { get; set; }
            public int LayerNumber { get; set; }
            public float LayerWidth { get; set; }
            public float WallHeight { get; set; }
            public float StartFromY { get; set; }
            public float StartFrom { get; set; }
            public float DrawTo { get; set; }
            public float NextLayerTop { get; set; }
            public int PreviousLayerBrickCount { get; set; }
            public SKPath Layers { get; set; }
            public SKRect model { get; set; }
            public string BrickText { get; set; }
            public int TotalBrickCount { get; set; }
            public List<BrickDetail> Bricktionary { get; set; }
            public bool IsFirstLayer { get; set; }
            public float CircleRadius { get; set; }
        }

        public class BrickDetail
        {
            public string Name { get; set; }
            public string location { get; set; }
            public int position { get; set; }
        }
    }
}

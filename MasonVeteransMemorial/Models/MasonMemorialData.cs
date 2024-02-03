using System;
using System.Collections.Generic;

namespace MasonVeteransMemorial.Models
{
    public class MasonMemorialData
    {
        public MasonMemorialData()
        {
        }

        public static ICollection<Brick> MasonBricks;
        public static ICollection<SectionQuadrant> SectionGridQuadrants;
        public static float HonorAreaGridHeight = 0;
        public static float HonorAreaGridWidth = 0;
        public static GridItemDimension MasonBrickDimension;
    }
}

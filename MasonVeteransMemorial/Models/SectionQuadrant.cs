using System;
namespace MasonVeteransMemorial.Models
{
    public class SectionQuadrant
    {
        public SectionQuadrant()
        {
        }

        public string Name { get; set; }
        public string Section { get; set; }
        public string Location { get; set; }
        public string Prefix { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Top { get; set; }
        public float Left { get; set; }
        public string ImageSource { get; set; }
        public float OverLapArea { get; set; }
    }
}

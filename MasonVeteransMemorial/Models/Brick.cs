using System;
namespace MasonVeteransMemorial.Models
{
    public class Brick
    {
        public string CompoundKey
        {
            get { return Location + Position.ToString(); }
        }

        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public string Section { get; set; }
        public string Location { get; set; }
        public int Position { get; set; }
        public string Line1st { get; set; }
        public string Line2nd { get; set; }
        public string Line3rd { get; set; }
        public string Comments { get; set; }
    }
}

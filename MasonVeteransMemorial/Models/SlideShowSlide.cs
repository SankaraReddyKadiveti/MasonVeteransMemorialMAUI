using System;

namespace MasonVeteransMemorial.Models
{
    public class SlideShowSlide
    {
        public int SlideId { get; set; }
        public string SlideTitle { get; set; }
        public bool HideImage { get; set; }
        public bool SlideImageContentOnly { get; set; }
        public ImageSource SlideImageContent { get; set; }
        public string SlideTextContent { get; set; }
        public int SortOrder { get; set; }
        public bool Skip { get; set; }
    }
}

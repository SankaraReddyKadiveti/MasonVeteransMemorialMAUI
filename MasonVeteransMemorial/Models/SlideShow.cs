using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace MasonVeteransMemorial.Models
{
    public class SlideShow
    {
        public ObservableCollection<SlideShowSlide> Slides { get; set; }
        public string Title { get; set; }
        public string ShortName { get; set; }
        public string SlideShowIcon { get; set; }
        public bool FirstPageImplementsReadMore { get; set; }

        public SlideShow()
        {
        }
    }
}

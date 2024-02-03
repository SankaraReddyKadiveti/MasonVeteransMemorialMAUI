using System;
using System.Collections.Generic;
using MasonVeteransMemorial.Models;
using System.Collections.ObjectModel;

namespace MasonVeteransMemorial.Data
{
    public class AboutMemorialSlideShow : SlideShow
    {
        public AboutMemorialSlideShow()
        {
            Title = "Veterans Memorial Info";
            ShortName = "About";
            SlideShowIcon = "info";
            FirstPageImplementsReadMore = false;
            Build();
        }

        public void Build()
        {
            if (null == Slides)
                Slides = new ObservableCollection<SlideShowSlide>();
            else
                Slides.Clear();

            var sortOrder = 1;

            Slides.Add(
                new SlideShowSlide
                {
                SlideTextContent = @"The Mason Veterans Memorial was dedicated on Saturday, November 8, 2003. Over 1000 people, including at least 150 veterans, attended the event. The late Neil Armstrong, a Korean War veteran and the first man to walk on the moon, was the guest of honor. He was introduced by Congressman Rob Portman (U.S. House of Representatives, Second District) and spoke briefly about the history of aviation in armed conflicts in the United States.",
                    SlideImageContent = "Veterans_Memorial_015",
                    HideImage = true,
                    SlideTitle = "Veterans Memorial",
                    SortOrder = sortOrder
                }
            );
            sortOrder++;
            Slides.Add(
                new SlideShowSlide
                {
                    SlideTextContent = @"In addition to Mr. Armstrong, Colonel Michael J. Belzil, Commander of the 88th Air Base Wing at Wright-Patterson Air Force also spoke. The significance of the poem by Lonna Kingsbury.",
                    SlideImageContent = "memorial_info_image_2",
                    SortOrder = sortOrder

                }
            );
            sortOrder++;
            Slides.Add(
                new SlideShowSlide
                {
                SlideTextContent = @"In addition to Mr. Armstrong, Colonel Michael J. Belzil, Commander of the 88th Air Base Wing at Wright-Patterson Air Force Base, also spoke. The significance of the memorial design was described through a poem by Lonna Kingsbury. At the head of the memorial, a flag proudly waves above a wall depicting the five branches of the service: Air Force, Army, Coast Guard, Marines, and Navy. At the base of the wall and around the flag are memorial bricks for veterans. These and other bricks to the sides of the memorial were purchased by citizens to help pay for its construction.",
                    SlideImageContent = "memorial_info_image_3",
                    SortOrder = sortOrder
                }
            );
            sortOrder++;
            Slides.Add(
                new SlideShowSlide
                {
                SlideTextContent = @"The main feature of the memorial is a set of 10 pillars representing the 10 major conflicts in American history. The height of each pillar is proportional to the number of casualties in the war. At the end of the row is a low stone representing the hope for peace and the end to casualties in war. ",
                    SlideImageContent = "memorial_info_image_4",
                    SortOrder = sortOrder
                }
            );
            sortOrder++;
            Slides.Add(
                new SlideShowSlide
                {
                SlideTextContent = @" An eternal flame at the lower end of the memorial will burn until all prisoners of war and missing in action are returned to American soil. It stands in front of a wall of tears that represents the sorrows of all those affected by war.",
                    SlideImageContent = "memorial_info_image_5",
                    SortOrder = sortOrder
                }
            );
        }
    }
}

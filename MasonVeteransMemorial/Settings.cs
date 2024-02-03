using System;
namespace MasonVeteransMemorial
{
    public class Settings
    {
        // this is the default static instance you'd use like string text = Settings.Default.SomeSetting;
        public readonly static Settings Default = new Settings();

        public const string MasonHomePageUrl = "https://www.imaginemason.org";

        public const string MasonAboutPageUrl = "https://www.imaginemason.org/about/veterans-memorial";

        public const string MasonDonatePageUrl = "https://www.imaginemason.org/about/veterans-memorial/#donations";

        public const string Mason3DTourUrl = "https://my.matterport.com/show/?m=bF3FWX44iAp";

        public string PlaceHolder { get; set; } // add setting properties as you wish

        public static bool UseMapPage { get; set; }
    }
}

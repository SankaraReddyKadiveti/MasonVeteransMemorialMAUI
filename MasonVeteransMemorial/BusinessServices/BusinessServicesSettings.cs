using System;
namespace MasonVeteransMemorial.BusinessServices
{
    public class BusinessServicesSettings
    {
        // this is the default static instance you'd use like string text = Settings.Default.SomeSetting;
        public readonly static Settings Default = new Settings();

        public const int HonorSectionColumnBrickCount = 33;
        public const int HonorSectionRowBrickCount = 39;
    }
}

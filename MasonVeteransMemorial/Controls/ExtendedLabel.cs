using System;
//using Xamarin.Forms;

namespace MasonVeteransMemorial.Controls
{
    public class ExtendedLabel : Label
    {
        public EventHandler Tapped;
        public ExtendedLabel()
        {
            var tapgesture = new TapGestureRecognizer();
            tapgesture.Tapped += (sender, e) =>
            {
                Tapped?.Invoke(sender, e);
            };
            GestureRecognizers.Add(tapgesture);
        }
    }
}

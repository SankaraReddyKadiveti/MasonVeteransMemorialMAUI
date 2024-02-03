using System;
using Android.Graphics;
using Android.Widget;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;

//[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(MasonVeteransMemorial.Droid.Effects.UnderlineEffect), "UnderlineEffect")]
namespace MasonVeteransMemorial.Droid.Effects
{
    public class UnderlineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var textView = (TextView)Control;
            textView.PaintFlags |= PaintFlags.UnderlineText;
        }

        protected override void OnDetached()
        {
            var textView = (TextView)Control;
            textView.PaintFlags &= ~PaintFlags.UnderlineText;
        }
    }
}
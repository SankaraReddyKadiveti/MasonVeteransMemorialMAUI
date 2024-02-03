using System;
using System.Threading;
//using Xamarin.Forms;

namespace MasonVeteransMemorial.Controls
{
    public class CustomViewCell : ViewCell
    {
        public static BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create("SelectedBackgroundColor", typeof(Color), typeof(CustomViewCell), Color.FromHex("#00000000"), BindingMode.TwoWay);
        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }

        public static BindableProperty IsSelectedProperty = BindableProperty.Create("IsSelected", typeof(bool), typeof(CustomViewCell), false);
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public CustomViewCell()
        {

        }
    }
}

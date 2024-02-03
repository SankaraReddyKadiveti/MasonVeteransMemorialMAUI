using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasonVeteransMemorial.Pages
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();            
            //Browser.OpenAsync(new Uri(Settings.MasonAboutPageUrl), BrowserLaunchMode.SystemPreferred);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Open the MasonAboutPageUrl when the About page appears
            Browser.OpenAsync(new Uri(Settings.MasonAboutPageUrl), BrowserLaunchMode.SystemPreferred);
        }        
               
    }
}
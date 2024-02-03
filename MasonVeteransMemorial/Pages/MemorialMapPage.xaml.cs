using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MasonVeteransMemorial.Models;
using MasonVeteransMemorial.ViewModels;
//using Xamarin.Forms;

namespace MasonVeteransMemorial.Pages
{
    public partial class MemorialMapPage : ContentPage, IMemorialMapViewModelDelegate
    {
        public static readonly BindableProperty MasonBrickProperty = BindableProperty.Create(nameof(MasonBrick), typeof(Brick), typeof(MemorialMapPage), null);
        public Brick MasonBrick
        {
            get => (Brick)GetValue(MasonBrickProperty);
            set => SetValue(MasonBrickProperty, value);
        }

        public MemorialMapViewModel ViewModel => BindingContext as MemorialMapViewModel;

        public MemorialMapPage()
        {
            InitializeComponent();
            Initialize(new Brick());
        }

        public MemorialMapPage(Brick brick)
        {
            InitializeComponent();
            Initialize(brick);
        }

        private void Initialize(Brick brick)
        {
            Title = "Map";
            //Icon = "ic_map";

            BindingContext = new MemorialMapViewModel(brick);
            ViewModel.Delegate = this;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(MasonBrick))
            {
                ViewModel.SelectedBrick = MasonBrick;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.MapBrickCommand.Execute(null);
        }

        protected void OnLogoClicked(object sender, EventArgs e)
        {
            Browser.OpenAsync(new Uri(Settings.MasonHomePageUrl), BrowserLaunchMode.SystemPreferred);
            //Device.OpenUri(new Uri(Settings.MasonHomePageUrl));
        }

        public void OnLoadFailure(string title, string message)
        {

        }

        public void OnLoadSuccess()
        {

        }

        public void OnSearchComplete()
        {

        }
    }
}

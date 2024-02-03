using MasonVeteransMemorial.Controls;
using MasonVeteransMemorial.ViewModels;
using Microsoft.Maui.ApplicationModel;
using System;

namespace MasonVeteransMemorial.Pages;

public partial class MainPage : ContentPage, IMainViewModelViewModelDelegate
{
    public MainViewModel ViewModel => BindingContext as MainViewModel;
    public MainPage()
	{
		InitializeComponent();
        BindingContext = new MainViewModel();
        ViewModel.Delegate = this;

        Title = "Memorial";
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (sender, e) =>
        {
            Browser.OpenAsync(new Uri("https://www.google.com/maps/?q=39.353381,-84.3083203"));
        };

        slAddress.GestureRecognizers.Add(tapGesture);
    }

	protected void OnLogoClicked(object sender, EventArgs e)
	{
        Browser.OpenAsync(new Uri(Settings.MasonHomePageUrl), BrowserLaunchMode.SystemPreferred);
        //Device.OpenUri(new Uri(Settings.MasonHomePageUrl));
    }

    protected void OnHomeClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(MainPage));        
    }

    protected void OnSearchClicked(object sender, EventArgs e)
    {
       
        Shell.Current.GoToAsync(nameof(SearchPage));
        //new MainAppTabContainer();
        //return Task.CompletedTask;

      /*  SearchPage searchPage = new SearchPage();
        Navigation.PushAsync(page: searchPage);*/
    }

    protected void OnAboutClicked(object sender, EventArgs e)
    {
        Browser.OpenAsync(new Uri(Settings.MasonAboutPageUrl), BrowserLaunchMode.SystemPreferred);
        //Launcher.TryOpenAsync(new Uri(Settings.MasonDonatePageUrl));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadCommand.Execute(null);
    }

    public void OnLoadSuccess()
    {       
    }

    public void OnLoadFailure(string title, string message)
    {
    }


    /*
        int count = 0;
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }*/
}


using MasonVeteransMemorial.Controls;
using MasonVeteransMemorial.Pages;

namespace MasonVeteransMemorial;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		Settings.UseMapPage = false;
		
		MainPage = new AppShell();
		//MainPage = new MainAppTabContainer();
		//MainPage = new TabbedPageDemoPage2();
		//MainPage = new AsapPage();

		Routing.RegisterRoute(nameof(MasonVeteransMemorial.Pages.MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(MasonVeteransMemorial.Pages.SearchPage), typeof(SearchPage));
		Routing.RegisterRoute(nameof(MasonVeteransMemorial.Pages.AboutPage), typeof(AboutPage));
		Routing.RegisterRoute(nameof(MasonVeteransMemorial.Pages.MemorialMapPage), typeof(MemorialMapPage));

	}
}

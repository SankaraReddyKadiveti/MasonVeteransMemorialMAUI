using MasonVeteransMemorial.Controls;
using MasonVeteransMemorial.Pages;
using Microsoft.Maui.Controls;

namespace MasonVeteransMemorial;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// new MainAppTabContainer();

		//Routes

		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
		Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
		Routing.RegisterRoute(nameof(MemorialMapPage), typeof(MemorialMapPage));

		//Settings.UseMapPage = true;
	}

   
}

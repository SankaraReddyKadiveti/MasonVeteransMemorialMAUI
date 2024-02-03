using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using MasonVeteransMemorial.Effects;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Hosting;

namespace MasonVeteransMemorial;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMauiCompatibility()
			.ConfigureEssentials()
			//.ConfigureEffects<UnderlineEffect, UnderlineEffect>()
			/*.ConfigureEffects(effects =>
			{
				effects.Add<IUnderlineEffect, UnderlineEffect >();
			})*/
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})


#if DEBUG
		.Logging.AddDebug();

#elif ANDROID || IOS || WINDOWS || MACCATALYST
            .InitPlatform()
#endif
		ConfigureServices(builder);
        return builder.Build();
	}
    private static void ConfigureServices(MauiAppBuilder builder)
    {
		builder.Services.AddTransient<UnderlineEffect>();
        // Register the required services, including EffectsFactory
        //builder.Services.AddMauiControlsHostingServices();
        //builder.ConfigureEffects();
        // Additional service registrations if needed
    }
}

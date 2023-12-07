using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DASApprenticeManagementPOC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .AddAppSettings()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder AddAppSettings(this MauiAppBuilder builder)
        {
            return builder.AddAppSettings("appSettings.json")
#if DEBUG
                    .AddAppSettings("appSettings.debug.json")
#else
                .AddAppSettings("appSettings.release.json")
#endif
                ;
        }

        private static MauiAppBuilder AddAppSettings(this MauiAppBuilder builder, string filename)
        {
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"FCMPushNotificationTest.{filename}");
            {
                if (stream != null)
                {
                    //var config = new ConfigurationBuilder()
                    //    .AddJsonStream(stream)
                    //    .Build();
                    builder.Configuration.AddJsonStream(stream);
                }
            }

            return builder;
        }

    }
}

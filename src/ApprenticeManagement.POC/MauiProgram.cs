using System.Reflection;
using ApprenticeManagement.POC.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;

#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
using Plugin.Firebase.Bundled.Shared;
#elif ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;
using System.Reflection;
#endif 

namespace ApprenticeManagement.POC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .AddAppSettings()
                .RegisterFirebaseServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var serviceUri = builder.Configuration.GetValue<string>("apprenticeManagementServiceBaseUri");
            builder.Services.AddMauiBlazorWebView()
                .Services.AddSingleton<DeviceManagementService>(new DeviceManagementService(serviceUri))
                ;

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<CustomAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(
                    sp => sp.GetRequiredService<CustomAuthenticationStateProvider>())
                .AddSingleton<UserAccounts>()
                .AddScoped<UserAccountAuthenticator>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
#if IOS
                events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) =>
                {
                    CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                    return false;
                }));
#else
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings());
                    CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
                }));
#endif
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(isAuthEnabled: true, isCloudMessagingEnabled: true, isCrashlyticsEnabled: true, isAnalyticsEnabled: true);
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
                .GetManifestResourceStream($"{typeof(MauiProgram).Namespace}.{filename}");
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

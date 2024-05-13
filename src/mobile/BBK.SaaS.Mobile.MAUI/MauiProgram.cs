using Microsoft.Extensions.Configuration;
using System.Reflection;
using BBK.SaaS.Core;
using Plugin.LocalNotification;

namespace BBK.SaaS.Mobile.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            ApplicationBootstrapper.InitializeIfNeeds<SaaSMobileMAUIModule>();

            var app = builder.Build();
            return app;
        }
    }
}
using Microsoft.Extensions.Logging;
using E_Commerce.ViewModels;
using E_Commerce.Views;

namespace E_Commerce
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddTransient<ProductDetailsViewModel>();
            builder.Services.AddTransient<ProductDetailsPage>();

            builder.Services.AddSingleton<CartViewModel>();
            builder.Services.AddSingleton<CartPage>();

            #if DEBUG
    		builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }
}

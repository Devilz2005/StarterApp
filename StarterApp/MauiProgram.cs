using Microsoft.Extensions.Logging;
using StarterApp.ViewModels;
using StarterApp.Database.Data;
using StarterApp.Views;
using System.Diagnostics;
using StarterApp.Services;

namespace StarterApp;

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

        // -------------------------------------------------------------------
        // Set useSharedApi to true to connect to the shared REST API,
        // or false to use the local PostgreSQL database.
        // This is an example of dependency injection — the ViewModels do
        // not change regardless of which implementation is registered here.
        // -------------------------------------------------------------------
        const bool useSharedApi = true;

        if (useSharedApi)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://set09102-api.b-davison.workers.dev/")
            };
            builder.Services.AddSingleton(httpClient);
            builder.Services.AddSingleton<IAuthenticationService, ApiAuthenticationService>();

            // Register the API-based item service so ViewModels can request
            // item data without needing to know how the API calls work.
            builder.Services.AddSingleton<IItemService, ApiItemService>();
        }
        else
        {
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        }

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<UserListPage>();

        builder.Services.AddTransient<UserDetailPage>();
        builder.Services.AddTransient<UserDetailViewModel>();

        // Register the item list page and its ViewModel
        builder.Services.AddSingleton<ItemListViewModel>();
        builder.Services.AddTransient<ItemListPage>();

        // Register the create item page and its ViewModel
        builder.Services.AddSingleton<CreateItemViewModel>();
        builder.Services.AddTransient<CreateItemPage>();

        // Register the item detail page and its ViewModel
        builder.Services.AddSingleton<ItemDetailViewModel>();
        builder.Services.AddTransient<ItemDetailPage>();

        // Register the update item page and its ViewModel
        builder.Services.AddSingleton<UpdateItemViewModel>();
        builder.Services.AddTransient<UpdateItemPage>();

        // Register the rental list page and its ViewModel
        builder.Services.AddSingleton<RentalListViewModel>();
        builder.Services.AddTransient<RentalListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
using StarterApp.Views;

namespace StarterApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register navigation routes so Shell can find pages
        Routing.RegisterRoute(nameof(ItemListPage), typeof(ItemListPage));
        Routing.RegisterRoute(nameof(CreateItemPage), typeof(CreateItemPage));
        Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
        Routing.RegisterRoute(nameof(UpdateItemPage), typeof(UpdateItemPage));

        // Rental requests page
        Routing.RegisterRoute(nameof(RentalListPage), typeof(RentalListPage));

        Routing.RegisterRoute(nameof(UserListPage), typeof(UserListPage));
        Routing.RegisterRoute(nameof(UserDetailPage), typeof(UserDetailPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }
}
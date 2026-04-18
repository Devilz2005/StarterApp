using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ItemListPage : ContentPage
{
    public ItemListPage(ItemListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // The ViewModel is injected via dependency injection,
        // so we don't manually create it (keeps everything consistent)
    }
}
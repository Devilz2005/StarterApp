using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the item every time this page appears again
        if (BindingContext is ItemDetailViewModel viewModel)
        {
            await viewModel.RefreshAsync();
        }
    }
}
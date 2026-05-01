using StarterApp.ViewModels;

namespace StarterApp.Views;

// Page used to display the details of a selected item
public partial class ItemDetailPage : ContentPage
{
    // Constructor receives the ViewModel through dependency injection
    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        // Loads the XAML layout for this page
        InitializeComponent();

        // Connects the ViewModel to the UI so bindings work
        BindingContext = viewModel;
    }

    // This method runs every time the page appears on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the item every time this page appears again
        // This ensures updated data is shown after editing
        if (BindingContext is ItemDetailViewModel viewModel)
        {
            await viewModel.RefreshAsync();
        }
    }
}
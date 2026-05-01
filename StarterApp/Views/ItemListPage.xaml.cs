using StarterApp.ViewModels;

namespace StarterApp.Views;

// Page used to display the list of items
public partial class ItemListPage : ContentPage
{
    // Constructor receives the ViewModel through dependency injection
    public ItemListPage(ItemListViewModel viewModel)
    {
        // Loads the XAML layout for this page
        InitializeComponent();

        // Connects the ViewModel to the UI so data binding works
        BindingContext = viewModel;

        // The ViewModel is injected via dependency injection,
        // so it is not created manually
    }
}
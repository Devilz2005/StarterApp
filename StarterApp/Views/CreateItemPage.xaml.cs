using StarterApp.ViewModels;

namespace StarterApp.Views;

// Page used to create a new item
public partial class CreateItemPage : ContentPage
{
    // Constructor receives the ViewModel from dependency injection
    public CreateItemPage(CreateItemViewModel viewModel)
    {
        // Loads the XAML layout for this page
        InitializeComponent();

        // Connects the ViewModel to the UI so data binding works
        BindingContext = viewModel;

        // ViewModel is injected via DI so it isn't created manually
    }
}
using StarterApp.ViewModels;

namespace StarterApp.Views;

// Page used to update an existing item
public partial class UpdateItemPage : ContentPage
{
    // Constructor receives the ViewModel through dependency injection
    public UpdateItemPage(UpdateItemViewModel viewModel)
    {
        // Loads the XAML layout for this page
        InitializeComponent();

        // Connects the ViewModel to the UI so data binding works
        BindingContext = viewModel;
    }
}
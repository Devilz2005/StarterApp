using StarterApp.ViewModels;

namespace StarterApp.Views;

// Page used to display incoming and outgoing rental requests
public partial class RentalListPage : ContentPage
{
    // Constructor receives the ViewModel through dependency injection
    public RentalListPage(RentalListViewModel viewModel)
    {
        // Loads the XAML layout for this page
        InitializeComponent();

        // Connects the ViewModel to the UI so data binding works
        BindingContext = viewModel;
    }
}
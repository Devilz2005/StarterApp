using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class CreateItemPage : ContentPage
{
    public CreateItemPage(CreateItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // ViewModel is injected via DI so it isn't created it manually
    }
}
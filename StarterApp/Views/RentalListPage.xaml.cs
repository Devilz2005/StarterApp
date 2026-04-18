using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class RentalListPage : ContentPage
{
    public RentalListPage(RentalListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
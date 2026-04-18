using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class UpdateItemPage : ContentPage
{
    public UpdateItemPage(UpdateItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
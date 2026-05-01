// Main dashboard view model for authenticated users

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

// ViewModel for the main dashboard page
// Manages dashboard display, user information, and navigation to other sections
public partial class MainViewModel : BaseViewModel
{
    // Authentication service used to access the logged-in user and handle logout
    private readonly IAuthenticationService _authService;
    
    // Navigation service used to move between pages
    private readonly INavigationService _navigationService;

    // The currently authenticated user
    [ObservableProperty]
    private User? currentUser;

    // Welcome message displayed on the dashboard
    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    // Controls whether admin-only features are visible
    [ObservableProperty]
    private bool isAdmin;

    // Default constructor for design-time support
    public MainViewModel()
    {
        // Default constructor for design time support
        Title = "Dashboard";
    }
    
    // Main constructor with dependency injection
    public MainViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Dashboard";

        LoadUserData();
    }

    // Loads the current user's data and sets up the dashboard
    private void LoadUserData()
    {
        CurrentUser = _authService.CurrentUser;
        IsAdmin = _authService.HasRole("Admin");
        
        if (CurrentUser != null)
        {
            WelcomeMessage = $"Welcome, {CurrentUser.FullName}!";
        }
    }

    // Command triggered when the user logs out
    [RelayCommand]
    private async Task LogoutAsync()
    {
        // Ask the user to confirm before logging out
        var result = await Application.Current.MainPage.DisplayAlert(
            "Logout", 
            "Are you sure you want to logout?", 
            "Yes", 
            "No");

        if (result)
        {
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync("LoginPage");
        }
    }

    // Navigates to the user profile page
    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("TempPage");
    }

    // Navigates to the settings page
    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await _navigationService.NavigateToAsync("TempPage");
    }

    // Navigates to the item list page
    [RelayCommand]
    private async Task NavigateToItemsAsync()
    {
        await _navigationService.NavigateToAsync("ItemListPage");
    }

    // Navigates to the user list page
    [RelayCommand]
    private async Task NavigateToUserListAsync()
    {
        // Only admin users are allowed to access user management
        if (!IsAdmin)
        {
            await Application.Current.MainPage.DisplayAlert("Access Denied", "You don't have permission to access admin features.", "OK");
            return;
        }
        
        await _navigationService.NavigateToAsync("UserListPage");
    }

    // Refreshes the dashboard data
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            IsBusy = true;
            LoadUserData();
            
            // Simulate refresh delay
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            SetError($"Failed to refresh data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
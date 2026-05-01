using System.Net.Http.Headers;
using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

// Handles authentication using the shared API instead of the local database.
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    // Stores the logged-in user while the app is running.
    private User? _currentUser;

    // Stores the current user's roles if the API provides any.
    private readonly List<string> _currentUserRoles = new();

    // Lets the rest of the app know when the user logs in or logs out.
    public event EventHandler<bool>? AuthenticationStateChanged;

    public bool IsAuthenticated => _currentUser != null;
    public User? CurrentUser => _currentUser;
    public List<string> CurrentUserRoles => _currentUserRoles;

    public ApiAuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        try
        {
            // Sends the email and password to the API to request a login token.
            var response = await _httpClient.PostAsJsonAsync("auth/token", new { email, password });

            if (!response.IsSuccessStatusCode)
            {
                var rawError = await response.Content.ReadAsStringAsync();

                ApiErrorResponse? error = null;
                try
                {
                    // Try to read the API's error message in a clean format.
                    error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                }
                catch
                {
                }

                return new AuthenticationResult(
                    false,
                    error?.Message ?? rawError ?? "Login failed");
            }

            // Read the token returned by the API.
            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (token == null || string.IsNullOrWhiteSpace(token.Token))
            {
                var rawToken = await response.Content.ReadAsStringAsync();
                return new AuthenticationResult(false, $"Login failed: invalid token response: {rawToken}");
            }

            // Save the token so future API requests are authenticated.
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Token);

            // Load the logged-in user's profile after login succeeds.
            var meResponse = await _httpClient.GetAsync("users/me");

            if (!meResponse.IsSuccessStatusCode)
            {
                var rawProfileError = await meResponse.Content.ReadAsStringAsync();
                return new AuthenticationResult(false, $"Login failed: could not load profile: {rawProfileError}");
            }

            var profile = await meResponse.Content.ReadFromJsonAsync<UserProfileResponse>();
            if (profile == null)
            {
                var rawProfile = await meResponse.Content.ReadAsStringAsync();
                return new AuthenticationResult(false, $"Login failed: invalid profile response: {rawProfile}");
            }

            // Convert the API profile into the app's User model.
            _currentUser = new User
            {
                Id = profile.Id,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                CreatedAt = profile.CreatedAt,
                IsActive = true
            };

            _currentUserRoles.Clear();

            // Notify the app that the user is now logged in.
            AuthenticationStateChanged?.Invoke(this, true);
            return new AuthenticationResult(true, "Login successful");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Login failed: {ex.Message}");
        }
    }

    public async Task<AuthenticationResult> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            // Sends the new user's details to the API registration endpoint.
            var response = await _httpClient.PostAsJsonAsync("auth/register", new
            {
                firstName,
                lastName,
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
            {
                var rawError = await response.Content.ReadAsStringAsync();

                ApiErrorResponse? error = null;
                try
                {
                    // Try to read the API's validation/error message.
                    error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                }
                catch
                {
                }

                return new AuthenticationResult(
                    false,
                    error?.Message ?? rawError ?? "Registration failed");
            }

            return new AuthenticationResult(true, "Registration successful. Please log in.");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Registration failed: {ex.Message}");
        }
    }

    public Task LogoutAsync()
    {
        // Clear the saved user, roles, and authentication token.
        _currentUser = null;
        _currentUserRoles.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        // Notify the app that the user is now logged out.
        AuthenticationStateChanged?.Invoke(this, false);
        return Task.CompletedTask;
    }

    public bool HasRole(string roleName) =>
        _currentUserRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

    public bool HasAnyRole(params string[] roleNames) =>
        roleNames.Any(HasRole);

    public bool HasAllRoles(params string[] roleNames) =>
        roleNames.All(HasRole);

    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        // Password changes are not implemented for the shared API version.
        return Task.FromResult(false);
    }

    // Matches the token response returned by the login endpoint.
    private record TokenResponse(string Token, DateTime ExpiresAt, int UserId);

    // Matches the profile response returned by users/me.
    private record UserProfileResponse(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        DateTime CreatedAt);

    // Matches the common API error response format.
    private record ApiErrorResponse(string Error, string Message);
}
using System.Net.Http.Headers;
using System.Net.Http.Json;
using StarterApp.Database.Models;

namespace StarterApp.Services;

public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private readonly List<string> _currentUserRoles = new();

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
            var response = await _httpClient.PostAsJsonAsync("auth/token", new { email, password });

            if (!response.IsSuccessStatusCode)
            {
                var rawError = await response.Content.ReadAsStringAsync();

                ApiErrorResponse? error = null;
                try
                {
                    error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                }
                catch
                {
                }

                return new AuthenticationResult(
                    false,
                    error?.Message ?? rawError ?? "Login failed");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (token == null || string.IsNullOrWhiteSpace(token.Token))
            {
                var rawToken = await response.Content.ReadAsStringAsync();
                return new AuthenticationResult(false, $"Login failed: invalid token response: {rawToken}");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Token);

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
        _currentUser = null;
        _currentUserRoles.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = null;
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
        return Task.FromResult(false);
    }

    private record TokenResponse(string Token, DateTime ExpiresAt, int UserId);

    private record UserProfileResponse(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        DateTime CreatedAt);

    private record ApiErrorResponse(string Error, string Message);
}
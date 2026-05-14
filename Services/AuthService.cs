using System.Text.Json;
using E_Commerce.Models;
using E_Commerce.Models.Api;

namespace E_Commerce.Services;

public static class AuthService
{
    private const string AuthKey = "auth_user";
    private const string TokenKey = "auth_token";

    private static User? _currentUser;
    public static User? CurrentUser 
    { 
        get
        {
            if (_currentUser == null)
            {
                var userJson = Preferences.Get(AuthKey, string.Empty);
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);
                }
            }
            return _currentUser;
        }
        private set
        {
            _currentUser = value;
            if (value == null)
            {
                Preferences.Remove(AuthKey);
                SecureStorage.Default.Remove(TokenKey);
            }
            else
            {
                var userJson = JsonSerializer.Serialize(value);
                Preferences.Set(AuthKey, userJson);
                if (!string.IsNullOrEmpty(value.Token))
                {
                    SecureStorage.Default.SetAsync(TokenKey, value.Token).Wait();
                }
            }
        }
    }

    public static bool IsLoggedIn => CurrentUser != null;

    public static void SetCurrentUser(UserDto dto)
    {
        CurrentUser = new User
        {
            FullName = dto.DisplayName,
            Email = dto.Email,
            Token = dto.Token,
            Role = dto.Role,
            Avatar = "👤"
        };
    }

    public static async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    public static void Logout()
    {
        CurrentUser = null;
    }
}

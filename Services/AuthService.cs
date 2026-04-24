using System.Collections.Generic;
using System.Linq;
using E_Commerce.Models;

namespace E_Commerce.Services;

public static class AuthService
{
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, FullName = "Ammar Yasser", Email = "ammar@email.com", Password = "123456", Avatar = "👤" },
        new User { Id = 2, FullName = "Ahmed Mohamed", Email = "ahmed@email.com", Password = "123456", Avatar = "👤" },
        new User { Id = 3, FullName = "Sara Ali", Email = "sara@email.com", Password = "123456", Avatar = "👤" },
        new User { Id = 4, FullName = "Omar Hassan", Email = "omar@email.com", Password = "password", Avatar = "👤" },
        new User { Id = 5, FullName = "Nour Ibrahim", Email = "nour@email.com", Password = "password", Avatar = "👤" },
    };

    public static User? CurrentUser { get; private set; }

    public static bool Login(string email, string password)
    {
        var user = _users.FirstOrDefault(u =>
            u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user != null)
        {
            CurrentUser = user;
            return true;
        }
        return false;
    }

    public static void Logout()
    {
        CurrentUser = null;
    }

    public static List<User> GetAllUsers() => _users;
}

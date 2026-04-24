using System.Windows.Input;
using E_Commerce.Services;

namespace E_Commerce.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        Title = "Login";
        LoginCommand = new Command(OnLogin);
    }

    private async void OnLogin()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Please enter your email";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your password";
            HasError = true;
            return;
        }

        IsBusy = true;

        // Simulate network delay
        await Task.Delay(800);

        bool success = AuthService.Login(Email, Password);

        IsBusy = false;

        if (success)
        {
            if (App.Current is App app)
            {
                app.MainPage = new AppShell();
            }
        }
        else
        {
            ErrorMessage = "Invalid email or password. Try ammar@email.com / 123456";
            HasError = true;
        }
    }
}

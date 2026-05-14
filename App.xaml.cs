namespace E_Commerce
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Check if user is already logged in
            if (Services.AuthService.CurrentUser != null)
            {
                return new Window(new AppShell());
            }

            var loginPage = activationState?.Context?.Services.GetService<Views.LoginPage>();
            
            // If for some reason loginPage is null, fallback to manual instantiation to prevent a hard crash
            if (loginPage == null) 
            {
                var apiService = new Services.ApiService();
                var loginViewModel = new ViewModels.LoginViewModel(apiService);
                loginPage = new Views.LoginPage(loginViewModel);
            }

            return new Window(loginPage);
    }
    }
}
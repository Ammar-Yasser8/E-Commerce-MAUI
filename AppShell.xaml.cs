using E_Commerce.Views;

namespace E_Commerce
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
        }
    }
}

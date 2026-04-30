using E_Commerce.Views;

namespace E_Commerce
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
            Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
            Routing.RegisterRoute(nameof(AdminProductsPage), typeof(AdminProductsPage));
            Routing.RegisterRoute(nameof(AdminCategoriesPage), typeof(AdminCategoriesPage));
            Routing.RegisterRoute(nameof(AdminOrdersPage), typeof(AdminOrdersPage));
            Routing.RegisterRoute(nameof(ProductEditPage), typeof(ProductEditPage));
            Routing.RegisterRoute(nameof(CategoryEditPage), typeof(CategoryEditPage));
        }
    }
}

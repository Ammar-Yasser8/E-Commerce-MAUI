using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class AdminProductsPage : ContentPage
{
	public AdminProductsPage(AdminProductsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}

using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class AdminCategoriesPage : ContentPage
{
	public AdminCategoriesPage(AdminCategoriesViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}

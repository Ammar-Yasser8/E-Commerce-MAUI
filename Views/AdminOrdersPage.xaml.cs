using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class AdminOrdersPage : ContentPage
{
	public AdminOrdersPage(AdminOrdersViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}

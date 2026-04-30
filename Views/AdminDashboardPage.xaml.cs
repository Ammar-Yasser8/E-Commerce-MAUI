using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class AdminDashboardPage : ContentPage
{
	public AdminDashboardPage(AdminDashboardViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}

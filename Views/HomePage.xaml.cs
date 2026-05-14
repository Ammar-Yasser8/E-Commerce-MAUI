using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is HomeViewModel vm)
		{
			await vm.InitializeAsync();
		}
	}
}

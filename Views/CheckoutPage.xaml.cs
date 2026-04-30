using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class CheckoutPage : ContentPage
{
	public CheckoutPage(CheckoutViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}

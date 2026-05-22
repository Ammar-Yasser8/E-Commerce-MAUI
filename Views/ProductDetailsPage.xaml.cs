using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ProductDetailsViewModel _viewModel;

    public ProductDetailsPage(ProductDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        MessagingCenter.Subscribe<ProductDetailsViewModel>(this, "ItemAddedToCart", async (sender) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                CartPopup.IsVisible = true;
                CartPopup.Opacity = 0;
                CartPopup.TranslationY = -20;

                await Task.WhenAll(
                    CartPopup.FadeTo(1, 250),
                    CartPopup.TranslateTo(0, 0, 250, Easing.CubicOut)
                );

                await Task.Delay(2000);

                await Task.WhenAll(
                    CartPopup.FadeTo(0, 250),
                    CartPopup.TranslateTo(0, -20, 250, Easing.CubicIn)
                );

                CartPopup.IsVisible = false;
            });
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
    }
}

using E_Commerce.ViewModels;

namespace E_Commerce.Views;

public partial class CategoryEditPage : ContentPage
{
	public CategoryEditPage(CategoryEditViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<CategoryEditViewModel, (bool Success, string Message)>(this, "CategorySaved", (sender, args) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                NotifyPopup.BackgroundColor = args.Success ? Color.FromArgb("#28A745") : Color.FromArgb("#FF3B30");
                NotifyIcon.Text = args.Success ? "✓" : "❌";
                NotifyText.Text = args.Message;

                NotifyPopup.IsVisible = true;
                NotifyPopup.Opacity = 0;
                NotifyPopup.TranslationY = -20;

                await Task.WhenAll(
                    NotifyPopup.FadeTo(1, 250),
                    NotifyPopup.TranslateTo(0, 0, 250, Easing.CubicOut)
                );

                await Task.Delay(2000);

                await Task.WhenAll(
                    NotifyPopup.FadeTo(0, 250),
                    NotifyPopup.TranslateTo(0, -20, 250, Easing.CubicIn)
                );

                NotifyPopup.IsVisible = false;
            });
        });
	}
}

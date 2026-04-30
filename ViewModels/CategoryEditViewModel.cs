using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using E_Commerce.Models.Api;
using E_Commerce.Services;
using Microsoft.Maui.Media;

namespace E_Commerce.ViewModels;

[QueryProperty(nameof(Category), "Category")]
public class CategoryEditViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private byte[]? _imageBytes;
    private string _imageName = string.Empty;

    private ProductCategory? _category;
    public ProductCategory? Category
    {
        get => _category;
        set
        {
            SetProperty(ref _category, value);
            if (value != null)
            {
                IsEditMode = true;
                Title = "Edit Category";
                Name = value.Name;
            }
            else
            {
                IsEditMode = false;
                Title = "Create Category";
            }
        }
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _pictureUrl = string.Empty;
    public string PictureUrl
    {
        get => _pictureUrl;
        set => SetProperty(ref _pictureUrl, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand GoBackCommand { get; }

    public CategoryEditViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Save Category";

        SaveCommand = new Command(async () => await SaveCategoryAsync());
        PickImageCommand = new Command(async () => await OnPickImageAsync());
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private async Task OnPickImageAsync()
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync();
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    _imageBytes = memoryStream.ToArray();
                }
                _imageName = result.FileName;
                PictureUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            if (App.Current?.MainPage != null)
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
        }
    }

    private async Task SaveCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            if (App.Current?.MainPage != null)
                await App.Current.MainPage.DisplayAlert("Error", "Name is required.", "OK");
            return;
        }

        IsBusy = true;
        bool success;

        try
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Name), "Name");

            if (_imageBytes != null)
            {
                var imageContent = new ByteArrayContent(_imageBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "Picture", _imageName);
            }

            if (IsEditMode && Category != null)
            {
                success = await _apiService.UpdateCategoryAsync(Category.Id, content);
            }
            else
            {
                success = await _apiService.CreateCategoryAsync(content);
            }
        }
        catch (Exception ex)
        {
            success = false;
            if (App.Current?.MainPage != null)
                await App.Current.MainPage.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
        }

        IsBusy = false;

        MessagingCenter.Send(this, "CategorySaved", (success, success ? $"Category {(IsEditMode ? "updated" : "created")}!" : "Failed to save category."));

        if (success)
        {
            await Task.Delay(2200);
            await Shell.Current.GoToAsync("..");
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using E_Commerce.Models.Api;
using E_Commerce.Services;
using Microsoft.Maui.Media;

namespace E_Commerce.ViewModels;

[QueryProperty(nameof(Product), "Product")]
public class ProductEditViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private byte[]? _imageBytes;
    private string _imageName = string.Empty;

    public System.Collections.ObjectModel.ObservableCollection<ProductCategory> CategoriesList { get; } = new();

    private ProductCategory? _selectedCategory;
    public ProductCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            if (value != null)
            {
                CategoryId = value.Id;
            }
        }
    }

    private ProductToReverseDto? _product;
    public ProductToReverseDto? Product
    {
        get => _product;
        set
        {
            SetProperty(ref _product, value);
            if (value != null)
            {
                IsEditMode = true;
                Title = "Edit Product";
                Name = value.Name;
                Description = value.Description;
                Price = value.Price;
                PictureUrl = value.PictureUrl;
                CategoryId = value.CategoryId;
                BrandId = value.BrandId;

                if (CategoriesList.Count > 0)
                {
                    SelectedCategory = CategoriesList.FirstOrDefault(c => c.Id == CategoryId);
                }
            }
            else
            {
                IsEditMode = false;
                Title = "Create Product";
                BrandId = 1; // default brand id
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

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    private string _pictureUrl = string.Empty;
    public string PictureUrl
    {
        get => _pictureUrl;
        set => SetProperty(ref _pictureUrl, value);
    }

    private int _categoryId;
    public int CategoryId
    {
        get => _categoryId;
        set => SetProperty(ref _categoryId, value);
    }

    private int _brandId;
    public int BrandId
    {
        get => _brandId;
        set => SetProperty(ref _brandId, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand GoBackCommand { get; }

    public ProductEditViewModel(ApiService apiService)
    {
        _apiService = apiService;
        Title = "Save Product";

        SaveCommand = new Command(async () => await SaveProductAsync());
        PickImageCommand = new Command(async () => await OnPickImageAsync());
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        Task.Run(LoadCategoriesAsync);
    }

    private async Task LoadCategoriesAsync()
    {
        var cats = await _apiService.GetCategoriesAsync();
        if (cats != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CategoriesList.Clear();
                foreach (var c in cats)
                {
                    CategoriesList.Add(c);
                }
                
                if (CategoryId != 0)
                {
                    SelectedCategory = CategoriesList.FirstOrDefault(c => c.Id == CategoryId);
                }
            });
        }
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

    private async Task SaveProductAsync()
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
            content.Add(new StringContent(Description), "Description");
            content.Add(new StringContent(Price.ToString()), "Price");
            content.Add(new StringContent(CategoryId == 0 ? "1" : CategoryId.ToString()), "CategoryId");

            int finalBrandId = BrandId == 0 ? 1 : BrandId;
            content.Add(new StringContent(finalBrandId.ToString()), "BrandId");

            if (_imageBytes != null)
            {
                var imageContent = new ByteArrayContent(_imageBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "Picture", _imageName);
            }

            if (IsEditMode && Product != null)
            {
                success = await _apiService.UpdateProductAsync(Product.Id, content);
            }
            else
            {
                success = await _apiService.CreateProductAsync(content);
            }
        }
        catch (Exception ex)
        {
            success = false;
            if (App.Current?.MainPage != null)
                await App.Current.MainPage.DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
        }

        IsBusy = false;

        MessagingCenter.Send(this, "ProductSaved", (success, success ? $"Product {(IsEditMode ? "updated" : "created")}!" : "Failed to save product."));

        if (success)
        {
            await Task.Delay(2200);
            await Shell.Current.GoToAsync("..");
        }
    }
}

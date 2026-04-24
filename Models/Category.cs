using E_Commerce.ViewModels;

namespace E_Commerce.Models;

public class Category : BaseViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}

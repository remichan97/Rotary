using CommunityToolkit.Mvvm.ComponentModel;

namespace Rotary.App.Models;

public partial class HeaderRow : ObservableObject
{
    [ObservableProperty]
    public partial string Key { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Value { get; set; } = string.Empty;
}

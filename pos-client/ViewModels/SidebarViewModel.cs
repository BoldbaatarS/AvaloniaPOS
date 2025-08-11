using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace RestaurantPOS.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private double sidebarWidth = 200;

    private bool _isExpanded = true;

    [RelayCommand]
    private async Task ToggleSidebar()
    {
        if (_isExpanded)
        {
            // Хураах
            for (double w = SidebarWidth; w > 50; w -= 10)
            {
                SidebarWidth = w;
                await Task.Delay(10);
            }
        }
        else
        {
            // Дэлгэх
            for (double w = SidebarWidth; w < 200; w += 10)
            {
                SidebarWidth = w;
                await Task.Delay(10);
            }
        }

        _isExpanded = !_isExpanded;
    }
}
